using DatabaseSharp.Models;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;

namespace DatabaseSharp
{
	/// <summary>
	/// A client to execute STPs from
	/// </summary>
	public class DBClient : IDBClient
	{
		/// <summary>
		/// Connection string to the database
		/// </summary>
		public string ConnectionString { get; set; }

		/// <summary>
		/// Main constructor
		/// </summary>
		/// <param name="connectionString"></param>
		public DBClient(string connectionString)
		{
			ConnectionString = connectionString;
		}

		/// <summary>
		/// Execute a STP with a set of parameters (if any).
		/// </summary>
		/// <param name="procedureName"></param>
		/// <param name="parameters"></param>
		/// <returns></returns>
		public async Task<DatabaseResult> ExecuteAsync(string procedureName, List<ISQLParameter>? parameters = null)
		{
			DataSet dt = new DataSet() { Locale = CultureInfo.InvariantCulture };
			using (SqlConnection sqlConn = new SqlConnection(ConnectionString))
			{
				using (SqlCommand sqlCmd = new SqlCommand(procedureName, sqlConn))
				{
					sqlCmd.CommandType = CommandType.StoredProcedure;
					if (parameters != null)
					{
						foreach (ISQLParameter s in parameters)
						{
							switch (s)
							{
								case SQLParam p:
									sqlCmd.Parameters.AddWithValue(s.Name, p.Value);
									break;
								case SQLListParam p:
									var added = sqlCmd.Parameters.AddWithValue(s.Name, p.GenerateParameter());
									added.SqlDbType = SqlDbType.Structured;
									break;
							}
						}
					}
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
