using DatabaseSharp.Attributes;
using DatabaseSharp.Models;
using System.Reflection;
using System.Text.Json;

namespace DatabaseSharp.Serializers
{
	/// <summary>
	/// This is a serialiser that will convert a given property into a JSON string in the database.
	/// Do note, you cannot give list parameters to this, it simply converts the List object to JSON aswell.
	/// </summary>
	public class DatabaseJsonSerializer : IDatabaseSerializer
	{
		/// <summary>
		/// Name of the JSON serializer
		/// </summary>
		public const string SerializerName = "JSON";
		public Type? OverrideDatabaseType { get; } = typeof(string);

		public dynamic? Deserialise(PropertyInfo propInfo, DatabaseResultRow row, DatabaseResult source)
		{
			var columnName = propInfo.Name;
			var underlying = Nullable.GetUnderlyingType(propInfo.PropertyType);
			if (propInfo.GetCustomAttribute<DatabaseSharpAttribute>() is DatabaseSharpAttribute overrideAttribute)
			{
				if (overrideAttribute.ColumnName != null)
					columnName = overrideAttribute.ColumnName;
			}
			var value = row.GetValue<string>(columnName);
			if (value == null || value == "")
				return null;
			var item = JsonSerializer.Deserialize(value, propInfo.PropertyType);
			return item;
		}

		public ISQLParameter Serialize(object item, PropertyInfo propInfo)
		{
			var itemValue = propInfo.GetValue(item);
			var value = JsonSerializer.Serialize(itemValue);
			var parameterName = propInfo.Name;
			if (propInfo.GetCustomAttribute<DatabaseSharpAttribute>() is DatabaseSharpAttribute overrideName)
			{
				if (overrideName.ParameterName != null)
					parameterName = overrideName.ParameterName;
			}
			if (itemValue == null)
				return new SQLParam(parameterName, null);
			return new SQLParam(parameterName, value);
		}
	}
}
