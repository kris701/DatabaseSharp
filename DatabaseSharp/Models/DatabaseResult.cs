using System.Data;

namespace DatabaseSharp.Models
{
	/// <summary>
	/// Result object from executing a STP
	/// </summary>
	public class DatabaseResult
	{
		/// <summary>
		/// Resulting dataset
		/// </summary>
		public DataSet DataSet { get; }
		/// <summary>
		/// A bool indicating if there are no tables in the dataset
		/// </summary>
		public bool IsEmpty { get => DataSet.Tables.Count == 0; }

		/// <summary>
		/// Main constructor
		/// </summary>
		/// <param name="dataSet"></param>
		public DatabaseResult(DataSet dataSet)
		{
			DataSet = dataSet;
		}

		/// <summary>
		/// Gets the amount of rows that is contained in a table
		/// </summary>
		/// <param name="table"></param>
		/// <returns></returns>
		/// <exception cref="ArgumentOutOfRangeException"></exception>
		public int RowCount(int table = 0)
		{
			if (DataSet.Tables.Count < table)
				throw new ArgumentOutOfRangeException($"Dataset only has {DataSet.Tables.Count} tables, but index '{table}' was requested!");
			return DataSet.Tables[table].Rows.Count;
		}

		/// <summary>
		/// Boolean check if a given column exists.
		/// </summary>
		/// <param name="column"></param>
		/// <param name="table"></param>
		/// <returns></returns>
		/// <exception cref="ArgumentOutOfRangeException"></exception>
		public bool ContainsColumn(string column, int table = 0)
		{
			if (DataSet.Tables.Count < table)
				throw new ArgumentOutOfRangeException($"Dataset only has {DataSet.Tables.Count} tables, but index '{table}' was requested!");
			return DataSet.Tables[table].Columns.Contains(column);
		}

		/// <summary>
		/// Converts a value from the datatable to a ordinary type
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="column"></param>
		/// <param name="rowID"></param>
		/// <param name="table"></param>
		/// <returns></returns>
		/// <exception cref="ArgumentOutOfRangeException"></exception>
		/// <exception cref="ArgumentNullException"></exception>
		public T GetValue<T>(string column, int rowID = 0, int table = 0) where T : IConvertible
		{
			if (DataSet.Tables.Count < table)
				throw new ArgumentOutOfRangeException($"Dataset only has {DataSet.Tables.Count} tables, but index '{table}' was requested!");
			object getObj = GetObjectValueFromDataTable(column, rowID, DataSet.Tables[table]);

			if (getObj == null)
				throw new ArgumentNullException("Result from the datatable is null!");
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

		/// <summary>
		/// Converts a value from the datatable to a structured type
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="column"></param>
		/// <param name="rowID"></param>
		/// <param name="table"></param>
		/// <returns></returns>
		/// <exception cref="ArgumentOutOfRangeException"></exception>
		/// <exception cref="ArgumentNullException"></exception>
		public T GetStructuredValue<T>(string column, int rowID = 0, int table = 0)
		{
			if (DataSet.Tables.Count < table)
				throw new ArgumentOutOfRangeException($"Dataset only has {DataSet.Tables.Count} tables, but index '{table}' was requested!");
			object getObj = GetObjectValueFromDataTable(column, rowID, DataSet.Tables[table]);

			return (T)Convert.ChangeType(getObj, typeof(T), System.Globalization.CultureInfo.InvariantCulture);
		}

		/// <summary>
		/// Converts a value from the datatable to a ordinary type or null
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="column"></param>
		/// <param name="rowID"></param>
		/// <param name="table"></param>
		/// <returns></returns>
		/// <exception cref="ArgumentOutOfRangeException"></exception>
		public T? GetValueOrNull<T>(string column, int rowID = 0, int table = 0) where T : struct
		{
			if (DataSet.Tables.Count < table)
				throw new ArgumentOutOfRangeException($"Dataset only has {DataSet.Tables.Count} tables, but index '{table}' was requested!");
			object getObj = GetObjectValueFromDataTable(column, rowID, DataSet.Tables[table]);

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

		private object GetObjectValueFromDataTable(string column, int rowID, DataTable table)
		{
			if (table.Rows.Count > 0)
			{
				if (table.Rows[rowID].Table.Columns.Contains(column))
					return table.Rows[rowID][column];
				else
					throw new Exception($"Table contains no column called '{column}'");
			}
			return null;
		}
	}
}
