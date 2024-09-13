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
							var type = s.GetType();
							if (type == typeof(SQLParam))
							{
								var p = (SQLParam)s;
								sqlCmd.Parameters.AddWithValue(s.Name, p.Value);
							}
							else if (type.IsAssignableTo(typeof(IListHandler)))
							{
								var p = (IListHandler)s;
								var values = p.CreateDataTable();
								if (values.Rows.Count > 0)
								{
									var added = sqlCmd.Parameters.AddWithValue(s.Name, values);
									added.SqlDbType = SqlDbType.Structured;
									added.TypeName = p.DatabaseTypeName;
								}
								else
									sqlCmd.Parameters.AddWithValue(s.Name, null);
							}
							else
								throw new Exception("Invalid SQL parameter!");
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
