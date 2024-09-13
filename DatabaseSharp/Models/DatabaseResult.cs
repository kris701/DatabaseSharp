using System.Collections;
using System.Data;

namespace DatabaseSharp.Models
{
	/// <summary>
	/// Result object from executing a STP
	/// </summary>
	public class DatabaseResult : IEnumerable<DatabaseResultTable>
	{
		private readonly DataSet _dataSet;
		/// <summary>
		/// A bool indicating if there are no tables in the dataset
		/// </summary>
		public bool IsEmpty { get => _dataSet.Tables.Count == 0; }
		/// <summary>
		/// Number of tables in the dataset
		/// </summary>
		public int Count => _dataSet.Tables.Count;

		/// <summary>
		/// Get a table from the dataset
		/// </summary>
		/// <param name="index"></param>
		/// <returns></returns>
		public DatabaseResultTable this[int index] => new DatabaseResultTable(_dataSet.Tables[index]);

		/// <summary>
		/// Main constructor
		/// </summary>
		/// <param name="dataSet"></param>
		public DatabaseResult(DataSet dataSet)
		{
			_dataSet = dataSet;
		}

		public IEnumerator<DatabaseResultTable> GetEnumerator() => new DatabaseResultEnumerator(_dataSet);

		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

		internal class DatabaseResultEnumerator : IEnumerator<DatabaseResultTable>
		{
			private readonly DataSet _dataset;
			private int _index;

			public DatabaseResultEnumerator(DataSet dataset)
			{
				_dataset = dataset;
				_index = -1;
			}

			public DatabaseResultTable Current => new DatabaseResultTable(_dataset.Tables[_index]);

			object IEnumerator.Current => new DatabaseResultTable(_dataset.Tables[_index]);

			public void Dispose()
			{
				_dataset.Dispose();
			}

			public bool MoveNext()
			{
				_index++;
				if (_index >= _dataset.Tables.Count)
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
