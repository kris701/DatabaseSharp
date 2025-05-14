using DatabaseSharp.Serializers;
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
		public T Fill<T>(DatabaseResult? source = null) where T : class, new()
		{
			var instance = new T();
			if (instance == null)
				throw new Exception("Could not create an empty instance of the class!");
			return Fill(instance.GetType(), source);
		}

		/// <summary>
		/// Attempt to deserialize the row into a class object
		/// </summary>
		/// <returns></returns>
		public dynamic Fill(Type asType, DatabaseResult? source = null)
		{
			var instance = Activator.CreateInstance(asType);
			if (instance == null)
				throw new Exception("Could not create an empty instance of the class!");

			var props = instance.GetType().GetProperties();
			foreach (var prop in props)
			{
				if (prop.GetCustomAttribute<DatabaseSharpIgnoreAttribute>() is DatabaseSharpIgnoreAttribute ignoreData)
					if (ignoreData.IgnoreAsFill)
						continue;

				var columnName = prop.Name;
				var underlying = Nullable.GetUnderlyingType(prop.PropertyType);
				if (prop.GetCustomAttribute<DatabaseSharpAttribute>() is DatabaseSharpAttribute overrideAttribute)
				{
					if (overrideAttribute.ColumnName != null)
						columnName = overrideAttribute.ColumnName;
					if (overrideAttribute.FillTable != -1)
					{
						if (source == null)
							throw new Exception("Cannot use a fill table reference with no source table!");
						if (prop.PropertyType.GenericTypeArguments.Length == 0)
							throw new Exception("Expected FillTable property to be a list!");

						var argProp = prop.PropertyType.GenericTypeArguments[0];
						var argUnderlying = Nullable.GetUnderlyingType(argProp);
						if (overrideAttribute.Serializer != null)
							prop.SetValue(instance, DeserializeItem(argProp, argUnderlying, overrideAttribute.Serializer, columnName));
						else
						{
							if (argProp.IsPrimitive ||
								argProp == typeof(string) ||
								argProp == typeof(DateTime) ||
								argProp == typeof(TimeSpan) ||
								argProp == typeof(Guid))
							{
								if (argUnderlying != null)
									prop.SetValue(instance, source[overrideAttribute.FillTable].GetAllValuesOrNull(argUnderlying, columnName));
								else
									prop.SetValue(instance, source[overrideAttribute.FillTable].GetAllValues(argProp, columnName));
							}
							else
								prop.SetValue(instance, source[overrideAttribute.FillTable].FillAll(argProp));
						}
						continue;
					}

					if (overrideAttribute.Serializer != null)
					{
						prop.SetValue(instance, DeserializeItem(prop.PropertyType, underlying, overrideAttribute.Serializer, columnName));
						continue;
					}
				}
				if (underlying != null)
					prop.SetValue(instance, GetValueOrNull(underlying, columnName));
				else
					prop.SetValue(instance, GetValue(prop.PropertyType, columnName));
			}

			return instance;
		}

		private dynamic? DeserializeItem(Type type, Type? underlyingType, string serializerName, string columnName)
		{
			if (underlyingType != null)
			{
				var serializer = Serializers[serializerName];
				var value = GetValueOrNull(typeof(string), columnName);
				if (value == null)
					return null;
				else
					return serializer.Deserialise(value, underlyingType);
			}
			else
			{
				var serializer = Serializers[serializerName];
				var value = GetValueOrNull(typeof(string), columnName);
				if (value == null)
					return null;
				else
					return serializer.Deserialise(value, type);
			}
		}

		/// <summary>
		/// Converts a value from the datatable to a ordinary type
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="columnName"></param>
		/// <returns></returns>
		/// <exception cref="ArgumentOutOfRangeException"></exception>
		/// <exception cref="ArgumentNullException"></exception>
		public T GetValue<T>(string columnName) where T : IConvertible => GetValue(typeof(T), columnName);

		public dynamic GetValue(Type asType, string columnName)
		{
			object getObj = GetObjectValueFromDataTable(columnName);

			if (getObj == null)
				throw new ArgumentNullException("Result from the datatable is null!");
			if (asType == typeof(bool))
			{
				if (getObj.ToString() == "1")
					return true;
				if (getObj.ToString() == "0")
					return false;
			}
			else if (getObj is DateTime dateTime)
			{
				// Simply always assume the data saved is in UTC time
				dateTime = DateTime.SpecifyKind(dateTime, DateTimeKind.Utc);
				if (asType == typeof(DateTime))
					return dateTime;
				getObj = dateTime.ToString("yyyy-MM-ddTHH:mm:ss.fffZ");
			}
			else if (asType == typeof(TimeSpan))
				return TimeSpan.Parse(getObj.ToString());
			else if (asType == typeof(Guid))
				return Convert.ChangeType(getObj, asType, System.Globalization.CultureInfo.InvariantCulture);

			return Convert.ChangeType(getObj.ToString(), asType, System.Globalization.CultureInfo.InvariantCulture);
		}

		/// <summary>
		/// Converts a value from the datatable to a ordinary type or null
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="columnName"></param>
		/// <returns></returns>
		/// <exception cref="ArgumentOutOfRangeException"></exception>
		public T? GetValueOrNull<T>(string columnName) where T : struct => GetValueOrNull(typeof(T), columnName);

		public dynamic? GetValueOrNull(Type asType, string columnName)
		{
			object getObj = GetObjectValueFromDataTable(columnName);

			if (getObj == null || DBNull.Value.Equals(getObj))
				return null;

			if (asType == typeof(bool))
			{
				if (getObj.ToString() == "1")
					return true;
				if (getObj.ToString() == "0")
					return false;
			}
			else if (getObj is DateTime dateTime)
			{
				// Simply always assume the data saved is in UTC time
				dateTime = DateTime.SpecifyKind(dateTime, DateTimeKind.Utc);
				if (asType == typeof(DateTime))
					return dateTime;
				getObj = dateTime.ToString("yyyy-MM-ddTHH:mm:ss.fffZ");
			}
			else if (asType == typeof(TimeSpan))
				return TimeSpan.Parse(getObj.ToString());
			else if (asType == typeof(Guid))
				return Convert.ChangeType(getObj, asType, System.Globalization.CultureInfo.InvariantCulture);

			return Convert.ChangeType(getObj.ToString(), asType, System.Globalization.CultureInfo.InvariantCulture);
		}

		private object GetObjectValueFromDataTable(string columnName)
		{
			if (!_row.Table.Columns.Contains(columnName))
				throw new Exception($"Table contains no column called '{columnName}'");
			return _row[columnName];
		}
	}
}
