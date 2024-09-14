using System.Data;
using System.Reflection;

namespace DatabaseSharp.Models
{
	public class DatabaseResultRow
	{
		private readonly DataRow _row;

		public DatabaseResultRow(DataRow row)
		{
			_row = row;
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
				if (prop.GetCustomAttribute<DatabaseSharpAttribute>() is DatabaseSharpAttribute overrideName)
					columnName = overrideName.ColumnName;
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

			return Convert.ChangeType(getObj, type, System.Globalization.CultureInfo.InvariantCulture);
		}

		/// <summary>
		/// Converts a value from the datatable to a ordinary type or null
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="columnName"></param>
		/// <returns></returns>
		/// <exception cref="ArgumentOutOfRangeException"></exception>
		public T? GetValueOrNull<T>(string columnName) where T : struct
		{
			object getObj = GetObjectValueFromDataTable(columnName);

			if (getObj == null || DBNull.Value.Equals(getObj))
				return null;

			if (typeof(T) == typeof(bool))
			{
				if (getObj.ToString() == "1")
					getObj = "true";
				else if (getObj.ToString() == "0")
					getObj = "false";
			}
			else if (getObj is DateTime dateTime)
			{
				if (typeof(T) == typeof(DateTime))
					return (T)(object)dateTime;
				getObj = dateTime.ToString("yyyy-MM-ddTHH:mm:ss.fffZ");
			}

			return (T)Convert.ChangeType(getObj.ToString(), typeof(T), System.Globalization.CultureInfo.InvariantCulture);
		}

		private object GetObjectValueFromDataTable(string columnName)
		{
			if (!_row.Table.Columns.Contains(columnName))
				throw new Exception($"Table contains no column called '{columnName}'");
			return _row[columnName];
		}
	}
}
