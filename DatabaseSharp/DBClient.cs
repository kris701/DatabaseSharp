using DatabaseSharp.Models;
using DatabaseSharp.Serializers;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Reflection;

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
		/// Set of optional property serializers
		/// </summary>
		public Dictionary<string, IDatabaseSerializer> Serializers { get; }

		/// <summary>
		/// Main constructor
		/// </summary>
		/// <param name="connectionString"></param>
		public DBClient(string connectionString)
		{
			ConnectionString = connectionString;
			Serializers = new Dictionary<string, IDatabaseSerializer>()
			{
				{ DatabaseJsonSerializer.SerializerName, new DatabaseJsonSerializer() },
				{ DatabaseEnumSerializer.SerializerName, new DatabaseEnumSerializer() },
			};
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
			return new DatabaseResult(dt, Serializers);
		}

		/// <summary>
		/// Execute a STP with a object that will be turned into parameters
		/// </summary>
		/// <param name="procedureName"></param>
		/// <param name="item"></param>
		/// <returns></returns>
		public async Task<DatabaseResult> ExecuteAsync(string procedureName, object item) => await ExecuteAsync(procedureName, GenerateParametersFromObject(item));

		/// <summary>
		/// Automatically generate STP parameters based on a given object
		/// </summary>
		/// <param name="item"></param>
		/// <returns></returns>
		public List<ISQLParameter>? GenerateParametersFromObject(object item)
		{
			var parameters = new List<ISQLParameter>();
			if (item != null)
			{
				var props = item.GetType().GetProperties();
				foreach (var prop in props)
				{
					if (prop.GetCustomAttribute<DatabaseSharpIgnoreAttribute>() is DatabaseSharpIgnoreAttribute ignoreData)
						if (ignoreData.IgnoreAsParameter)
							continue;

					var value = prop.GetValue(item);

					var typeName = "";
					var columnName = "";
					var parameterName = prop.Name;
					if (prop.GetCustomAttribute<DatabaseSharpAttribute>() is DatabaseSharpAttribute overrideName)
					{
						if (overrideName.ParameterName != null)
							parameterName = overrideName.ParameterName;
						if (overrideName.TypeName != null)
							typeName = overrideName.TypeName;
						if (overrideName.ColumnName != null)
							columnName = overrideName.ColumnName;
						if (overrideName.Serializer != null)
						{
							var serializer = Serializers[overrideName.Serializer];
							value = serializer.Serialize(value, prop.PropertyType);
						}
					}

					if (value is List<dynamic> lst)
						parameters.Add(new SQLListParam<dynamic>(parameterName, lst, columnName, typeName));
					else
						parameters.Add(new SQLParam(parameterName, value));
				}
			}
			if (parameters.Count == 0)
				return null;
			return parameters;
		}
	}
}
