using DatabaseSharp.Models;
using System.Data;
using System.Data.SqlClient;

namespace DatabaseSharp
{
	public class DBClient
	{
		public string ConnectionString { get; set; }

		public DBClient(string connectionString)
		{
			ConnectionString = connectionString;
		}

		public async Task<DatabaseResult> Execute(string procedureName, List<SQLParam> parameters = null)
		{
			DataSet dt = new DataSet();
			using (SqlConnection sqlConn = new SqlConnection(ConnectionString))
			{
				using (SqlCommand sqlCmd = new SqlCommand(procedureName, sqlConn))
				{
					sqlCmd.CommandType = CommandType.StoredProcedure;
					if (parameters != null)
						foreach (SQLParam s in parameters)
							sqlCmd.Parameters.AddWithValue(s.ParamName, s.Value);
					await sqlConn.OpenAsync();
					using (SqlDataAdapter sqlAdapter = new SqlDataAdapter(sqlCmd))
					{
						await Task.Run(() => sqlAdapter.Fill(dt));
					}
				}
			}
			return new DatabaseResult(dt);
		}

	}
}
