﻿using DatabaseSharp.Models;
using System.Data;
using System.Data.SqlClient;

namespace DatabaseSharp
{
	/// <summary>
	/// A client to execute STPs from
	/// </summary>
	public class DBClient
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
		public async Task<DatabaseResult> Execute(string procedureName, List<SQLParam>? parameters = null)
		{
			DataSet dt = new DataSet();
			using (SqlConnection sqlConn = new SqlConnection(ConnectionString))
			{
				using (SqlCommand sqlCmd = new SqlCommand(procedureName, sqlConn))
				{
					sqlCmd.CommandType = CommandType.StoredProcedure;
					if (parameters != null)
						foreach (SQLParam s in parameters)
							sqlCmd.Parameters.AddWithValue(s.Name, s.Value);
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
