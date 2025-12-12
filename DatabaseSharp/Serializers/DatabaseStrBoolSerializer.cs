using DatabaseSharp.Attributes;
using DatabaseSharp.Models;
using System.Collections;
using System.Reflection;

namespace DatabaseSharp.Serializers
{
	/// <summary>
	/// This is a serializer that converts strings in the form of "Y" and "N" to boolean values for the database.
	/// </summary>
	public class DatabaseStrBoolSerializer : IDatabaseSerializer
	{
		/// <summary>
		/// Name of the JSON serializer
		/// </summary>
		public const string SerializerName = "STRB";

		public Type? OverrideDatabaseType { get; } = typeof(bool);
		public dynamic? Deserialise(PropertyInfo propInfo, DatabaseResultRow row, DatabaseResult source)
		{
			var columnName = propInfo.Name;
			var underlying = Nullable.GetUnderlyingType(propInfo.PropertyType);
			if (propInfo.GetCustomAttribute<DatabaseSharpAttribute>() is DatabaseSharpAttribute overrideAttribute)
			{
				if (overrideAttribute.ColumnName != null)
					columnName = overrideAttribute.ColumnName;
				if (overrideAttribute.FillTable != -1)
				{
					if (propInfo.PropertyType.GenericTypeArguments.Length > 0)
					{
						var argProp = propInfo.PropertyType.GenericTypeArguments[0];
						var argUnderlying = Nullable.GetUnderlyingType(argProp);
						if (argUnderlying != null)
							return ReturnAsStringListOrNull(source[overrideAttribute.FillTable].GetAllValuesOrNull<bool>(columnName));
						else
							return ReturnAsStringListOrNull(source[overrideAttribute.FillTable].GetAllValues<bool>(columnName));
					}
					else
					{
						if (underlying != null)
							return ReturnAsStringOrNull(source[overrideAttribute.FillTable][0].GetValueOrNull<bool>(columnName));
						else
							return ReturnAsStringOrNull(source[overrideAttribute.FillTable][0].GetValue<bool>(columnName));
					}
				}
			}
			if (underlying != null)
				return ReturnAsStringOrNull(row.GetValueOrNull<bool>(columnName));
			else
				return ReturnAsStringOrNull(row.GetValue<bool>(columnName));
		}

		private dynamic? ReturnAsStringListOrNull(IList values)
		{
			if (values == null)
				return null;
			var newValues = new List<string>();
			foreach (var value in values)
				newValues.Add(ReturnAsStringOrNull(value));
			return newValues;
		}

		private dynamic? ReturnAsStringOrNull(object? value)
		{
			if (value == null)
				return null;
			if (value is bool bValue && bValue)
				return "Y";
			return "N";
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

			if (value is IList lst)
			{
				var valueList = new List<bool>();
				foreach (var val in lst)
					if (val is string lsVal)
						valueList.Add(lsVal.ToLower() == "y");
				return new SQLListParam(parameterName, valueList);
			}
			if (value == null)
				return new SQLParam(parameterName, null);
			if (value is string sVal)
			{
				if (sVal.ToLower() == "y")
					return new SQLParam(parameterName, true);
				else
					return new SQLParam(parameterName, false);
			}
			throw new Exception("Unknown property type for serializer!");
		}
	}
}
