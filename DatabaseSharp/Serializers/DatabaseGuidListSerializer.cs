using DatabaseSharp.Attributes;
using DatabaseSharp.Models;
using System.Collections;
using System.Reflection;
using System.Text.Json;

namespace DatabaseSharp.Serializers
{
	/// <summary>
	/// This is a serializer that supports converting a concatinated string in the database to a list type in C#
	/// </summary>
	public class DatabaseGuidListSerializer : IDatabaseSerializer
	{
		/// <summary>
		/// Name of the Guid list serializer
		/// </summary>
		public const string SerializerName = "GUDL";

		public Type? OverrideDatabaseType { get; } = typeof(Guid);
		public char Seperator { get; set; } = ';';
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
			var item = ReturnAsStringOrNull(value);
			return item;
		}

		private dynamic? ReturnAsStringOrNull(object? value)
		{
			if (value == null)
				return null;
			if (value is string sValue)
			{
				var list = new List<Guid>();
				var split = sValue.Split(Seperator).ToList();
				foreach (var item in split)
					list.Add(new Guid(item));
				return list;
			}
			return new List<Guid>();
		}

		public ISQLParameter Serialize(object item, PropertyInfo propInfo)
		{
			var value = propInfo.GetValue(item);
			var parameterName = propInfo.Name;
			if (propInfo.GetCustomAttribute<DatabaseSharpAttribute>() is DatabaseSharpAttribute overrideName)
			{
				if (overrideName.ParameterName != null)
					parameterName = overrideName.ParameterName;
			}

			if (value == null)
				return new SQLParam(parameterName, null);
			if (value is IList lst)
			{
				var asStr = "";
				foreach(var lstItem in lst)
					asStr += lstItem.ToString() + Seperator;
				if (asStr.Length > 0)
					asStr = asStr.Substring(0, asStr.Length - 1);
				return new SQLParam(parameterName, asStr);
			}
			else
				throw new Exception("Property should be a string list!");
		}
	}
}
