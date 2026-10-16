using DatabaseSharp.Models;
using MySqlConnector;
using System.Data;
using System.Globalization;

namespace DatabaseSharp.MySQL
{
	/// <summary>
	/// A client to execute STPs on a MySQL server
	/// </summary>
	public class MySQLClient : BaseDBClient
	{
		/// <summary>
		/// Main constructor
		/// </summary>
		/// <param name="connectionString"></param>
		public MySQLClient(string connectionString) : base(connectionString)
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
			using (var sqlConn = new MySqlConnection(ConnectionString))
			{
				using (var sqlCmd = new MySqlCommand(procedureName, sqlConn))
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
							else
								throw new Exception("Invalid SQL parameter!");
						}
					}

					if (token != null)
					{
						await sqlConn.OpenAsync(token.Value);
						if (token.Value.IsCancellationRequested)
							return new DatabaseResult(new DataSet());
						using (var sqlAdapter = new MySqlDataAdapter(sqlCmd))
						{
							await Task.Run(() => sqlAdapter.Fill(dt), token.Value);
						}
						if (token.Value.IsCancellationRequested)
							return new DatabaseResult(new DataSet());
					}
					else
					{
						await sqlConn.OpenAsync();
						using (var sqlAdapter = new MySqlDataAdapter(sqlCmd))
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
			using (var sqlConn = new MySqlConnection(ConnectionString))
			{
				using (var sqlCmd = new MySqlCommand(freeSql, sqlConn))
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
					using (var sqlAdapter = new MySqlDataAdapter(sqlCmd))
					{
						await Task.Run(() => sqlAdapter.Fill(dt));
					}
				}
			}
			return new DatabaseResult(dt, Serializers);
		}
	}
}
