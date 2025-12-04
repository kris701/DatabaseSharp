using DatabaseSharp.Attributes;
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
		private readonly DatabaseResult _parent;

		public DatabaseResultRow(DataRow row, DatabaseResult parent, Dictionary<string, IDatabaseSerializer> serializers)
		{
			_row = row;
			_parent = parent;
			Serializers = serializers;
		}

		/// <summary>
		/// Attempt to deserialize the row into a class object
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		public T Fill<T>() where T : class, new() => Fill(typeof(T));

		/// <summary>
		/// Attempt to deserialize the row into a class object
		/// </summary>
		/// <returns></returns>
		public dynamic Fill(Type asType)
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

				var serializerName = DatabaseDefaultSerializer.SerializerName;
				if (prop.GetCustomAttribute<DatabaseSharpAttribute>() is DatabaseSharpAttribute overrideAttribute)
				{
					if (overrideAttribute.Serializer != null)
						serializerName = overrideAttribute.Serializer;
				}
				var serializer = Serializers[serializerName];
				prop.SetValue(instance, serializer.Deserialise(prop, this, _parent));
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
			else if (asType == typeof(double))
				return double.Parse(getObj.ToString());

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
			else if (asType == typeof(double))
				return double.Parse(getObj.ToString());

			return Convert.ChangeType(getObj.ToString(), asType, System.Globalization.CultureInfo.InvariantCulture);
		}

		private object GetObjectValueFromDataTable(string columnName)
		{
			if (!_row.Table.Columns.Contains(columnName))
				throw new Exception($"Table contains no column called '{columnName}'");
			return _row[columnName];
		}

		/// <summary>
		/// Simply return the data as a <seealso cref="DataRow"/> instance
		/// </summary>
		/// <returns></returns>
		public DataRow ToDataTable() => _row;
	}
}
