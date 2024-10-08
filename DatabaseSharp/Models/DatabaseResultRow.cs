﻿using DatabaseSharp.Serializers;
using System.Data;
using System.Reflection;

namespace DatabaseSharp.Models
{
	public class DatabaseResultRow
	{
		/// <summary>
		/// Set of optional property serializers
		/// </summary>
		public Dictionary<string, IDatabaseSerializer> Serializers { get; }

		private readonly DataRow _row;

		public DatabaseResultRow(DataRow row, Dictionary<string, IDatabaseSerializer> serializers)
		{
			_row = row;
			Serializers = serializers;
		}

		/// <summary>
		/// Attempt to deserialize the row into a class object
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		public T Fill<T>() where T : class, new()
		{
			var instance = new T();
			if (instance == null)
				throw new Exception("Could not create an empty instance of the class!");

			var props = instance.GetType().GetProperties();
			foreach (var prop in props)
			{
				if (prop.GetCustomAttribute<DatabaseSharpIgnoreAttribute>() is DatabaseSharpIgnoreAttribute ignoreData)
					if (ignoreData.IgnoreAsFill)
						continue;

				var columnName = prop.Name;
				if (prop.GetCustomAttribute<DatabaseSharpAttribute>() is DatabaseSharpAttribute overrideAttribute)
				{
					if (overrideAttribute.ColumnName != null)
						columnName = overrideAttribute.ColumnName;

					if (overrideAttribute.Serializer != null)
					{
						var serializer = Serializers[overrideAttribute.Serializer];
						var value = GetValue(columnName, typeof(string));
						prop.SetValue(instance, serializer.Deserialise(value, prop.PropertyType));
						continue;
					}
				}
				var underlying = Nullable.GetUnderlyingType(prop.PropertyType);
				if (underlying != null)
					prop.SetValue(instance, GetValueOrNull(columnName, underlying));
				else
					prop.SetValue(instance, GetValue(columnName, prop.PropertyType));
			}

			return instance;
		}

		/// <summary>
		/// Converts a value from the datatable to a ordinary type
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="columnName"></param>
		/// <returns></returns>
		/// <exception cref="ArgumentOutOfRangeException"></exception>
		/// <exception cref="ArgumentNullException"></exception>
		public T GetValue<T>(string columnName) where T : IConvertible => GetValue(columnName, typeof(T));

		private dynamic GetValue(string columnName, Type type)
		{
			object getObj = GetObjectValueFromDataTable(columnName);

			if (getObj == null)
				throw new ArgumentNullException("Result from the datatable is null!");
			if (type == typeof(bool))
			{
				if (getObj.ToString() == "1")
					getObj = "true";
				else if (getObj.ToString() == "0")
					getObj = "false";
			}
			else if (getObj is DateTime dateTime)
			{
				if (type == typeof(DateTime))
					return dateTime;
				getObj = dateTime.ToString("yyyy-MM-ddTHH:mm:ss.fffZ");
			}
			else if (type == typeof(Guid))
				return Convert.ChangeType(getObj, type, System.Globalization.CultureInfo.InvariantCulture);

			return Convert.ChangeType(getObj.ToString(), type, System.Globalization.CultureInfo.InvariantCulture);
		}

		/// <summary>
		/// Converts a value from the datatable to a ordinary type or null
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="columnName"></param>
		/// <returns></returns>
		/// <exception cref="ArgumentOutOfRangeException"></exception>
		public T? GetValueOrNull<T>(string columnName) where T : struct => GetValueOrNull(columnName, typeof(T));

		private dynamic? GetValueOrNull(string columnName, Type type)
		{
			object getObj = GetObjectValueFromDataTable(columnName);

			if (getObj == null || DBNull.Value.Equals(getObj))
				return null;

			if (type == typeof(bool))
			{
				if (getObj.ToString() == "1")
					getObj = "true";
				else if (getObj.ToString() == "0")
					getObj = "false";
			}
			else if (getObj is DateTime dateTime)
			{
				if (type == typeof(DateTime))
					return dateTime;
				getObj = dateTime.ToString("yyyy-MM-ddTHH:mm:ss.fffZ");
			}
			else if (type == typeof(Guid))
				return Convert.ChangeType(getObj, type, System.Globalization.CultureInfo.InvariantCulture);

			return Convert.ChangeType(getObj.ToString(), type, System.Globalization.CultureInfo.InvariantCulture);
		}

		private object GetObjectValueFromDataTable(string columnName)
		{
			if (!_row.Table.Columns.Contains(columnName))
				throw new Exception($"Table contains no column called '{columnName}'");
			return _row[columnName];
		}
	}
}
