using DatabaseSharp.Models;
using DatabaseSharp.Serializers;
using System.Collections;
using System.Reflection;

namespace DatabaseSharp.Helpers
{
	public static class ParameterHelpers
	{
		/// <summary>
		/// Automatically generate STP parameters based on a given object
		/// </summary>
		/// <param name="item"></param>
		/// <returns></returns>
		public static List<ISQLParameter>? GenerateParametersFromObject(object item, Dictionary<string, IDatabaseSerializer> serializers)
		{
			var parameters = new List<ISQLParameter>();
			if (item != null)
			{
				var props = item.GetType().GetProperties();
				foreach (var prop in props)
				{
					var parameter = GenerateParameterFromProperty(item, prop, serializers);
					if (parameter != null)
						parameters.Add(parameter);
				}
			}

			if (parameters.Count == 0)
				return null;
			return parameters;
		}

		public static ISQLParameter? GenerateParameterFromProperty(object item, PropertyInfo prop, Dictionary<string, IDatabaseSerializer> serializers)
		{
			if (prop.GetCustomAttribute<DatabaseSharpIgnoreAttribute>() is DatabaseSharpIgnoreAttribute ignoreData)
				if (ignoreData.IgnoreAsParameter)
					return null;

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
					if (value != null)
					{
						var serializer = serializers[overrideName.Serializer];
						value = serializer.Serialize(value, prop.PropertyType);
					}
				}
			}

			if (value is IList lst)
				return new SQLListParam(parameterName, lst, columnName, typeName);
			else
				return new SQLParam(parameterName, value);
		}
	}
}
