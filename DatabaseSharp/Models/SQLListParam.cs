using DatabaseSharp.Helpers;
using DatabaseSharp.Serializers;
using System.Collections;
using System.Data;
using System.Reflection;

namespace DatabaseSharp.Models
{
	/// <summary>
	/// A list parameter given to a STP for a SQL database
	/// </summary>
	public class SQLListParam : ISQLParameter, IListHandler
	{
		/// <summary>
		/// Name of the parameter
		/// </summary>
		public string Name { get; set; }
		/// <summary>
		/// Name of the column to put the data into
		/// </summary>
		public string TableColumnName { get; set; }
		/// <summary>
		/// Name of the type to send it as
		/// </summary>
		public string DatabaseTypeName { get; set; }
		/// <summary>
		/// Value of the parameter
		/// </summary>
		public IList Values { get; set; }

		/// <summary>
		/// Main constructor
		/// </summary>
		/// <param name="name"></param>
		/// <param name="values"></param>
		/// <param name="tableColumnName"></param>
		/// <param name="databaseTypeName"></param>
		public SQLListParam(string name, IList values, string tableColumnName = "", string databaseTypeName = "")
		{
			Name = name;
			Values = values;
			TableColumnName = tableColumnName;
			DatabaseTypeName = databaseTypeName;
		}

		/// <summary>
		/// Generate a datatable from this object
		/// </summary>
		/// <returns></returns>
		public DataTable CreateDataTable(Dictionary<string, IDatabaseSerializer> serializers)
		{
			DataTable table = new DataTable();
			if (Values.Count == 0)
				return table;
			var type = Values[0].GetType();
			if (type.IsPrimitive || type == typeof(Guid) || type == typeof(string))
			{
				table.Columns.Add(TableColumnName, type);
				foreach (var value in Values)
					table.Rows.Add(value);
				return table;
			}
			else
			{
				var props = Values[0].GetType().GetProperties().ToList();
				props.RemoveAll(prop => 
					prop.GetCustomAttribute<DatabaseSharpIgnoreAttribute>() is DatabaseSharpIgnoreAttribute ignoreData &&
					ignoreData.IgnoreAsParameter);
				if (props.Count == 0)
					return table;

				foreach (var prop in props)
				{
					var propName = prop.Name;
					var propType = GetActualType(prop.PropertyType);
					if (prop.GetCustomAttribute<DatabaseSharpAttribute>() is DatabaseSharpAttribute overrides)
					{
						if (overrides.ParameterName != null)
							propName = overrides.ParameterName;
						if (overrides.Serializer != null)
						{
							var serializer = serializers[overrides.Serializer];
							propType = serializer.DatabaseType;
						}
					}

					table.Columns.Add(propName, propType);
				}

				foreach (var value in Values)
				{
					var valueParameters = ParameterHelpers.GenerateParametersFromObject(value, serializers);
					if (valueParameters == null)
						continue;
					var row = table.NewRow();
					foreach (var valueParam in valueParameters)
					{
						if (valueParam is SQLParam actual)
						{
							if (actual.Value == null)
								row[actual.Name] = DBNull.Value;
							else
								row[actual.Name] = actual.Value;
						}
					}
					table.Rows.Add(row);
				}

				return table;
			}
		}

		private Type GetActualType(Type type)
		{
			if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
				return Nullable.GetUnderlyingType(type);
			return type;
		}

		public override int GetHashCode()
		{
			return HashCode.Combine(Name, Values);
		}

		public override bool Equals(object? obj)
		{
			if (obj is SQLParam other)
			{
				if (Name != other.Name) return false;
				if (Values != other.Value) return false;
				return true;
			}
			return false;
		}
	}
}
