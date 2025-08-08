using DatabaseSharp.Attributes;
using DatabaseSharp.Models;
using System.Collections;
using System.Reflection;

namespace DatabaseSharp.Serializers
{
	/// <summary>
	/// This is a serializer that converts enums into integers in the database, and back.
	/// List parameters are supported as well as FillTable arguments.
	/// </summary>
	public class DatabaseEnumSerializer : IDatabaseSerializer
	{
		/// <summary>
		/// Name of the JSON serializer
		/// </summary>
		public const string SerializerName = "ENUM";

		public Type? OverrideDatabaseType { get; } = typeof(int);
		public dynamic? Deserialise(PropertyInfo propInfo, DatabaseResultRow row, DatabaseResult source)
		{
			var enumType = propInfo.PropertyType;
			if (propInfo.PropertyType.GenericTypeArguments.Length > 0)
				enumType = propInfo.PropertyType.GenericTypeArguments[0];

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
							return ReturnAsEnumListOrNull(source[overrideAttribute.FillTable].GetAllValuesOrNull<int>(columnName), enumType);
						else
							return ReturnAsEnumListOrNull(source[overrideAttribute.FillTable].GetAllValues<int>(columnName), enumType);
					}
					else
					{
						if (underlying != null)
							return ReturnAsEnumOrNull(source[overrideAttribute.FillTable][0].GetValueOrNull<int>(columnName), enumType);
						else
							return ReturnAsEnumOrNull(source[overrideAttribute.FillTable][0].GetValue<int>(columnName), enumType);
					}
				}
			}
			if (underlying != null)
				return ReturnAsEnumOrNull(row.GetValueOrNull<int>(columnName), enumType);
			else
				return ReturnAsEnumOrNull(row.GetValue<int>(columnName), enumType);
		}

		private dynamic? ReturnAsEnumListOrNull(IList values, Type enumType)
		{
			if (values == null)
				return null;
			Type genericListType = typeof(List<>).MakeGenericType(enumType);
			var newValues = (IList)Activator.CreateInstance(genericListType);
			foreach (var value in values)
				newValues.Add(ReturnAsEnumOrNull(value, enumType));
			return newValues;
		}

		private dynamic? ReturnAsEnumOrNull(object? value, Type enumType)
		{
			if (value == null)
				return null;
			return Enum.ToObject(enumType, value);
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
				var valueList = new List<int>();
				foreach (var val in lst)
					valueList.Add(Convert.ToInt32(val));
				return new SQLListParam(parameterName, valueList);
			}
			if (value == null)
				return new SQLParam(parameterName, null);
			if (Enum.IsDefined(propInfo.PropertyType, value))
				return new SQLParam(parameterName, Convert.ToInt32(value));
			throw new Exception("Unknown enum property to serialize!");
		}
	}
}
