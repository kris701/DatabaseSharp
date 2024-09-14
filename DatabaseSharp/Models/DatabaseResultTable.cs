using DatabaseSharp.Serializers;
using System.Collections;
using System.Data;

namespace DatabaseSharp.Models
{
	/// <summary>
	/// Datatable result
	/// </summary>
	public class DatabaseResultTable : IEnumerable<DatabaseResultRow>
	{
		/// <summary>
		/// Set of optional property serializers
		/// </summary>
		public Dictionary<string, IDatabaseSerializer> Serializers { get; }

		/// <summary>
		/// Number of rows in the table
		/// </summary>
		public int Count => _table.Rows.Count;
		/// <summary>
		/// Number of rows in the table
		/// </summary>
		public List<string> Columns { get; }
		/// <summary>
		/// Get a row from the datatable
		/// </summary>
		/// <param name="index"></param>
		/// <returns></returns>
		public DatabaseResultRow this[int index] => new DatabaseResultRow(_table.Rows[index], Serializers);

		private readonly DataTable _table;

		/// <summary>
		/// Main constructor
		/// </summary>
		/// <param name="table"></param>
		/// <param name="serializers"></param>
		public DatabaseResultTable(DataTable table, Dictionary<string, IDatabaseSerializer> serializers)
		{
			_table = table;
			Serializers = serializers;
			Columns = new List<string>();
			foreach (DataColumn col in _table.Columns)
				Columns.Add(col.ColumnName);
		}

		/// <summary>
		/// Boolean check if a given column exists.
		/// </summary>
		/// <param name="column"></param>
		/// <returns></returns>
		/// <exception cref="ArgumentOutOfRangeException"></exception>
		public bool ContainsColumn(string column)
		{
			return _table.Columns.Contains(column);
		}

		/// <summary>
		/// Create a list of all the rows in the table
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		public List<T> FillAll<T>() where T : class, new()
		{
			var result = new List<T>();
			foreach (var row in this)
				result.Add(row.Fill<T>());
			return result;
		}

		/// <summary>
		/// Get a column value across all rows in the table
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="columnName"></param>
		/// <returns></returns>
		public List<T> GetAllValues<T>(string columnName) where T : IConvertible
		{
			var result = new List<T>();
			foreach (var row in this)
				result.Add(row.GetValue<T>(columnName));
			return result;
		}

		/// <summary>
		/// Get a value (or null) across all rows in the table
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="columnName"></param>
		/// <returns></returns>
		public List<T?> GetAllValuesOrNull<T>(string columnName) where T : struct
		{
			var result = new List<T?>();
			foreach (var row in this)
				result.Add(row.GetValueOrNull<T>(columnName));
			return result;
		}

		public IEnumerator<DatabaseResultRow> GetEnumerator() => new DatabaseResultTableEnumerator(_table, Serializers);

		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

		internal class DatabaseResultTableEnumerator : IEnumerator<DatabaseResultRow>
		{
			private readonly DataTable _table;
			private int _index;
			private Dictionary<string, IDatabaseSerializer> _serializers;

			public DatabaseResultTableEnumerator(DataTable table, Dictionary<string, IDatabaseSerializer> serializers)
			{
				_table = table;
				_index = -1;
				_serializers = serializers;
			}

			public DatabaseResultRow Current => new DatabaseResultRow(_table.Rows[_index], _serializers);

			object IEnumerator.Current => new DatabaseResultRow(_table.Rows[_index], _serializers);

			public void Dispose()
			{
				_table.Dispose();
			}

			public bool MoveNext()
			{
				_index++;
				if (_index >= _table.Rows.Count)
					return false;
				return true;
			}

			public void Reset()
			{
				_index = 0;
			}
		}
	}
}
