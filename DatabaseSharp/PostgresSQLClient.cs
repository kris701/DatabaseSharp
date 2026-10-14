using DatabaseSharp.Helpers;
using DatabaseSharp.Models;
using DatabaseSharp.Serializers;
using Npgsql;
using System.Data;
using System.Globalization;

namespace DatabaseSharp
{
	/// <summary>
	/// A client to execute STPs on a Postgres SQL server
	/// </summary>
	public class PostgresSQLClient : IDBClient
	{
		/// <summary>
		/// Connection string to the database
		/// </summary>
		public string ConnectionString { get; set; }

		/// <summary>
		/// Set of optional property serializers
		/// </summary>
		public Dictionary<string, IDatabaseSerializer> Serializers { get; }

		/// <summary>
		/// Main constructor
		/// </summary>
		/// <param name="connectionString"></param>
		public PostgresSQLClient(string connectionString)
		{
			ConnectionString = connectionString;
			Serializers = new Dictionary<string, IDatabaseSerializer>()
			{
				{ DatabaseJsonSerializer.SerializerName, new DatabaseJsonSerializer() },
				{ DatabaseEnumSerializer.SerializerName, new DatabaseEnumSerializer() },
				{ DatabaseStrBoolSerializer.SerializerName, new DatabaseStrBoolSerializer() },
				{ DatabaseBase64Serializer.SerializerName, new DatabaseBase64Serializer() },
				{ DatabaseStrListSerializer.SerializerName, new DatabaseStrListSerializer() },
				{ DatabaseGuidListSerializer.SerializerName, new DatabaseGuidListSerializer() },
				{ DatabaseDefaultSerializer.SerializerName, new DatabaseDefaultSerializer() },
			};
		}

		/// <summary>
		/// Execute a STP with a set of parameters (if any).
		/// </summary>
		/// <param name="procedureName"></param>
		/// <param name="parameters"></param>
		/// <param name="token"></param>
		/// <returns></returns>
		public async Task<DatabaseResult> ExecuteAsync(string procedureName, List<ISQLParameter>? parameters = null, CancellationToken? token = null)
		{
			DataSet dt = new DataSet() { Locale = CultureInfo.InvariantCulture };
			using (var sqlConn = new NpgsqlConnection(ConnectionString))
			{
				using (var sqlCmd = new NpgsqlCommand(procedureName, sqlConn))
				{
					sqlCmd.CommandTimeout = sqlConn.ConnectionTimeout;
					sqlCmd.CommandType = CommandType.StoredProcedure;
					if (parameters != null)
					{
						foreach (ISQLParameter s in parameters)
						{
							var type = s.GetType();
							if (type == typeof(SQLParam))
							{
								var p = (SQLParam)s;
								var added = sqlCmd.Parameters.AddWithValue(s.Name, p.Value);
								if (p.Value is byte[])
									added.DbType = DbType.Binary;
							}
							else if (type.IsAssignableTo(typeof(IListHandler)))
							{
								var p = (IListHandler)s;
								var values = p.CreateDataTable(Serializers);
								if (values.Rows.Count > 0)
								{
									var added = sqlCmd.Parameters.AddWithValue(s.Name, values);
									added.DbType = DbType.Object;
								}
								else
									sqlCmd.Parameters.AddWithValue(s.Name, null);
							}
							else
								throw new Exception("Invalid SQL parameter!");
						}
					}

					if (token != null)
					{
						await sqlConn.OpenAsync(token.Value);
						if (token.Value.IsCancellationRequested)
							return new DatabaseResult(new DataSet());
						using (var sqlAdapter = new NpgsqlDataAdapter(sqlCmd))
						{
							await Task.Run(() => sqlAdapter.Fill(dt), token.Value);
						}
						if (token.Value.IsCancellationRequested)
							return new DatabaseResult(new DataSet());
					}
					else
					{
						await sqlConn.OpenAsync();
						using (var sqlAdapter = new NpgsqlDataAdapter(sqlCmd))
						{
							await Task.Run(() => sqlAdapter.Fill(dt));
						}
					}
				}
			}
			return new DatabaseResult(dt, Serializers);
		}

		/// <summary>
		/// Execute a STP with a object that will be turned into parameters
		/// </summary>
		/// <param name="procedureName"></param>
		/// <param name="item"></param>
		/// <param name="token"></param>
		/// <returns></returns>
		public async Task<DatabaseResult> ExecuteAsync(string procedureName, object item, CancellationToken? token = null) => await ExecuteAsync(procedureName, ParameterHelpers.GenerateParametersFromObject(item, Serializers), token);

		/// <summary>
		/// Execute some SQL query.
		/// </summary>
		/// <param name="freeSql"></param>
		/// <param name="token"></param>
		/// <returns></returns>
		public async Task<DatabaseResult> ExecuteFreeAsync(string freeSql, CancellationToken? token = null)
		{
			DataSet dt = new DataSet() { Locale = CultureInfo.InvariantCulture };
			using (var sqlConn = new NpgsqlConnection(ConnectionString))
			{
				using (var sqlCmd = new NpgsqlCommand(freeSql, sqlConn))
				{
					sqlCmd.CommandTimeout = sqlConn.ConnectionTimeout;
					sqlCmd.CommandType = CommandType.Text;
					if (token != null)
					{
						await sqlConn.OpenAsync(token.Value);
						if (token.Value.IsCancellationRequested)
							return new DatabaseResult(new DataSet());
					}
					else
						await sqlConn.OpenAsync();
					using (var sqlAdapter = new NpgsqlDataAdapter(sqlCmd))
					{
						await Task.Run(() => sqlAdapter.Fill(dt));
					}
				}
			}
			return new DatabaseResult(dt, Serializers);
		}
	}
}
