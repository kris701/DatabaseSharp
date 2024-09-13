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
		public T Fill<T>() 
			where T : class, new()
		{
			var instance = new T();
			if (instance == null)
				throw new Exception("Could not create an empty instance of the class!");

			var props = instance.GetType().GetProperties();
			foreach(var prop in props)
			{
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
		/// <param name="column"></param>
		/// <returns></returns>
		/// <exception cref="ArgumentOutOfRangeException"></exception>
		/// <exception cref="ArgumentNullException"></exception>
		public T GetValue<T>(string column) where T : IConvertible => GetValue(column, typeof(T));

		private dynamic GetValue(string column, Type type)
		{
			object getObj = GetObjectValueFromDataTable(column);

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
					return (object)dateTime;
				getObj = dateTime.ToString("yyyy-MM-ddTHH:mm:ss.fffZ");
			}

			return Convert.ChangeType(getObj, type, System.Globalization.CultureInfo.InvariantCulture);
		}

		/// <summary>
		/// Converts a value from the datatable to a ordinary type or null
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="column"></param>
		/// <returns></returns>
		/// <exception cref="ArgumentOutOfRangeException"></exception>
		public T? GetValueOrNull<T>(string column) where T : struct
		{
			object getObj = GetObjectValueFromDataTable(column);

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

		private object GetObjectValueFromDataTable(string column)
		{
			if (!_row.Table.Columns.Contains(column))
				throw new Exception($"Table contains no column called '{column}'");
			return _row[column];
		}
	}
}
