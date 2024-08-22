using System.Data;

namespace DatabaseSharp.Models
{
	public class DatabaseResult
	{
		public DataSet DataSet { get; }
		public bool IsEmpty { get => DataSet.Tables.Count == 0; }

		public DatabaseResult(DataSet dataSet)
		{
			DataSet = dataSet;
		}

		public int RowCount(int table = 0)
		{
			if (DataSet.Tables.Count < table)
				throw new Exception($"Dataset only has {DataSet.Tables.Count} tables, but index '{table}' was requested!");
			return DataSet.Tables[table].Rows.Count;
		}

		public bool ContainsColumn(string column, int table = 0)
		{
			if (DataSet.Tables.Count < table)
				throw new Exception($"Dataset only has {DataSet.Tables.Count} tables, but index '{table}' was requested!");
			return DataSet.Tables[table].Columns.Contains(column);
		}

		public T GetValue<T>(string column, int rowID = 0, int table = 0) where T : IConvertible
		{
			if (DataSet.Tables.Count < table)
				throw new Exception($"Dataset only has {DataSet.Tables.Count} tables, but index '{table}' was requested!");
			object getObj = GetObjectValueFromDataTable(column, rowID, DataSet.Tables[table]);

			if (getObj == null)
				throw new ArgumentNullException("Result from the datatable is null!");

			return (T)Convert.ChangeType(getObj, typeof(T), System.Globalization.CultureInfo.InvariantCulture);
		}

		public T? GetValueOrNull<T>(string column, int rowID = 0, int table = 0) where T : struct
		{
			if (DataSet.Tables.Count < table)
				throw new Exception($"Dataset only has {DataSet.Tables.Count} tables, but index '{table}' was requested!");
			object getObj = GetObjectValueFromDataTable(column, rowID, DataSet.Tables[table]);

			if (getObj == null || DBNull.Value.Equals(getObj))
				return null;
			else
				return (T)Convert.ChangeType(getObj, typeof(T), System.Globalization.CultureInfo.InvariantCulture);
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
