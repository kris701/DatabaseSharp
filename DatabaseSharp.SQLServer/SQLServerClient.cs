using DatabaseSharp.Models;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Globalization;

namespace DatabaseSharp.SQLServer
{
	/// <summary>
	/// A client to execute STPs on a SQL Server database
	/// </summary>
	public class SQLServerClient : BaseDBClient
	{
		/// <summary>
		/// Main constructor
		/// </summary>
		/// <param name="connectionString"></param>
		public SQLServerClient(string connectionString) : base(connectionString)
		{
		}

		/// <summary>
		/// Execute a STP with a set of parameters (if any).
		/// </summary>
		/// <param name="procedureName"></param>
		/// <param name="parameters"></param>
		/// <param name="token"></param>
		/// <returns></returns>
		public override async Task<DatabaseResult> ExecuteAsync(string procedureName, List<ISQLParameter>? parameters = null, CancellationToken? token = null)
		{
			DataSet dt = new DataSet() { Locale = CultureInfo.InvariantCulture };
			using (var sqlConn = new SqlConnection(ConnectionString))
			{
				using (var sqlCmd = new SqlCommand(procedureName, sqlConn))
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
									added.SqlDbType = SqlDbType.VarBinary;
							}
							else if (type.IsAssignableTo(typeof(IListHandler)))
							{
								var p = (IListHandler)s;
								var values = p.CreateDataTable(Serializers);
								if (values.Rows.Count > 0)
								{
									var added = sqlCmd.Parameters.AddWithValue(s.Name, values);
									added.SqlDbType = SqlDbType.Structured;
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
						using (var sqlAdapter = new SqlDataAdapter(sqlCmd))
						{
							await Task.Run(() => sqlAdapter.Fill(dt), token.Value);
						}
						if (token.Value.IsCancellationRequested)
							return new DatabaseResult(new DataSet());
					}
					else
					{
						await sqlConn.OpenAsync();
						using (var sqlAdapter = new SqlDataAdapter(sqlCmd))
						{
							await Task.Run(() => sqlAdapter.Fill(dt));
						}
					}
				}
			}
			return new DatabaseResult(dt, Serializers);
		}

		/// <summary>
		/// Execute some SQL query.
		/// </summary>
		/// <param name="freeSql"></param>
		/// <param name="token"></param>
		/// <returns></returns>
		public override async Task<DatabaseResult> ExecuteFreeAsync(string freeSql, CancellationToken? token = null)
		{
			DataSet dt = new DataSet() { Locale = CultureInfo.InvariantCulture };
			using (var sqlConn = new SqlConnection(ConnectionString))
			{
				using (var sqlCmd = new SqlCommand(freeSql, sqlConn))
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
					using (var sqlAdapter = new SqlDataAdapter(sqlCmd))
					{
						await Task.Run(() => sqlAdapter.Fill(dt));
					}
				}
			}
			return new DatabaseResult(dt, Serializers);
		}
	}
}
