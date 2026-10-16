using DatabaseSharp.Models;
using Npgsql;
using System.Data;
using System.Globalization;

namespace DatabaseSharp.PostgreSQL
{
	/// <summary>
	/// A client to execute STPs on a Postgres SQL server
	/// </summary>
	public class PostgreSQLClient : BaseDBClient
	{
		/// <summary>
		/// Main constructor
		/// </summary>
		/// <param name="connectionString"></param>
		public PostgreSQLClient(string connectionString) : base(connectionString)
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
			using (var sqlConn = new NpgsqlConnection(ConnectionString))
			{
				using (var sqlCmd = new NpgsqlCommand($"SELECT * FROM {procedureName}({BuildArgString(parameters)})", sqlConn))
				{
					sqlCmd.CommandTimeout = sqlConn.ConnectionTimeout;

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
		/// Execute some SQL query.
		/// </summary>
		/// <param name="freeSql"></param>
		/// <param name="token"></param>
		/// <returns></returns>
		public override async Task<DatabaseResult> ExecuteFreeAsync(string freeSql, CancellationToken? token = null)
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

		private string BuildArgString(List<ISQLParameter>? parameters)
		{
			if (parameters == null || parameters.Count == 0)
				return "";
			var args = "";
			foreach (var arg in parameters)
				args += $"'{GetParamValue(arg)}',";
			return args.Substring(0, args.LastIndexOf(','));
		}

		private object? GetParamValue(ISQLParameter parameter)
		{
			switch (parameter)
			{
				case SQLParam p:
					return p.Value;
				case SQLListParam p:
					throw new Exception("List parameters are not supported with PostgreSQL functions!");
				default:
					throw new Exception("Unknown parameter type!");
			}
		}
	}
}
