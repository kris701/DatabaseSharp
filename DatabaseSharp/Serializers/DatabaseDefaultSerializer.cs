using DatabaseSharp.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace DatabaseSharp.Serializers
{
	/// <summary>
	/// Default serializer for DatabaseSharp
	/// </summary>
	public class DatabaseDefaultSerializer : IDatabaseSerializer
	{
		/// <summary>
		/// Name of the JSON serializer
		/// </summary>
		public const string SerializerName = "DEFAULT";
		public Type? OverrideDatabaseType { get; } = null;

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
						if (argProp.IsPrimitive ||
							argProp == typeof(string) ||
							argProp == typeof(DateTime) ||
							argProp == typeof(TimeSpan) ||
							argProp == typeof(Guid))
						{
							if (argUnderlying != null)
								return source[overrideAttribute.FillTable].GetAllValuesOrNull(argUnderlying, columnName);
							else
								return source[overrideAttribute.FillTable].GetAllValues(argProp, columnName);
						}
						else
							return source[overrideAttribute.FillTable].FillAll(argProp);
					}
					else
					{
						if (propInfo.PropertyType.IsPrimitive ||
							propInfo.PropertyType == typeof(string) ||
							propInfo.PropertyType == typeof(DateTime) ||
							propInfo.PropertyType == typeof(TimeSpan) ||
							propInfo.PropertyType == typeof(Guid))
						{
							if (underlying != null)
								return source[overrideAttribute.FillTable][0].GetValueOrNull(underlying, columnName);
							else
								return source[overrideAttribute.FillTable][0].GetValue(propInfo.PropertyType, columnName);
						}
						else
							return source[overrideAttribute.FillTable][0].Fill(propInfo.PropertyType);
					}
				}
			}
			if (underlying != null)
				return row.GetValueOrNull(underlying, columnName);
			else
				return row.GetValue(propInfo.PropertyType, columnName);
		}

		public ISQLParameter Serialize(object item, PropertyInfo propInfo)
		{
			var value = propInfo.GetValue(item);

			var columnName = "";
			var parameterName = propInfo.Name;
			if (propInfo.GetCustomAttribute<DatabaseSharpAttribute>() is DatabaseSharpAttribute overrideName)
			{
				if (overrideName.ParameterName != null)
					parameterName = overrideName.ParameterName;
				if (overrideName.ColumnName != null)
					columnName = overrideName.ColumnName;
			}

			if (value is IList lst)
				return new SQLListParam(parameterName, lst, columnName);
			else
				return new SQLParam(parameterName, value);
		}
	}
}
