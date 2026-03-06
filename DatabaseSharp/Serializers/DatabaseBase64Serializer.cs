using DatabaseSharp.Attributes;
using DatabaseSharp.Models;
using System.Buffers.Text;
using System.Reflection;
using System.Text.Json;

namespace DatabaseSharp.Serializers
{
	/// <summary>
	/// This is a serialiser that will convert a given property into a base64 string and back
	/// </summary>
	public class DatabaseBase64Serializer : IDatabaseSerializer
	{
		/// <summary>
		/// Name of the Base64 serializer
		/// </summary>
		public const string SerializerName = "BASE64";
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
			var item = Base64Decode(value);
			return item;
		}

		public ISQLParameter Serialize(object item, PropertyInfo propInfo)
		{
			var itemValue = propInfo.GetValue(item);
			if (itemValue is string itemValueStr)
			{
				var value = Base64Encode(itemValueStr);
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
			throw new Exception("The Base64Serializer can only serialize strings!");
		}

		public static string Base64Encode(string plainText)
		{
			var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
			return System.Convert.ToBase64String(plainTextBytes);
		}

		public static string Base64Decode(string base64EncodedData)
		{
			var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
			return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
		}
	}
}
