using DatabaseSharp.Serializers;
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
		/// Set of optional property serializers
		/// </summary>
		public Dictionary<string, IDatabaseSerializer> Serializers { get; }

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
		public DatabaseResultTable this[int index] => new DatabaseResultTable(_dataSet.Tables[index], this, Serializers);

		/// <summary>
		/// Main constructor
		/// </summary>
		/// <param name="dataSet"></param>
		/// <param name="serializers"></param>
		public DatabaseResult(DataSet dataSet, Dictionary<string, IDatabaseSerializer> serializers)
		{
			_dataSet = dataSet;
			Serializers = serializers;
		}

		/// <summary>
		/// Main constructor
		/// </summary>
		/// <param name="dataSet"></param>
		public DatabaseResult(DataSet dataSet)
		{
			_dataSet = dataSet;
			Serializers = new Dictionary<string, IDatabaseSerializer>()
			{
				{ DatabaseDefaultSerializer.SerializerName, new DatabaseDefaultSerializer() }
			};
		}

		/// <summary>
		/// Simply return the data as a <seealso cref="DataSet"/> instance
		/// </summary>
		/// <returns></returns>
		public DataSet ToDataset() => _dataSet;

		public IEnumerator<DatabaseResultTable> GetEnumerator() => new DatabaseResultEnumerator(_dataSet, this, Serializers);

		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

		internal class DatabaseResultEnumerator : IEnumerator<DatabaseResultTable>
		{
			private readonly DataSet _dataset;
			private readonly DatabaseResult _parent;
			private int _index;
			private readonly Dictionary<string, IDatabaseSerializer> _serializers;

			public DatabaseResultEnumerator(DataSet dataset, DatabaseResult parent, Dictionary<string, IDatabaseSerializer> serializers)
			{
				_dataset = dataset;
				_parent = parent;
				_index = -1;
				_serializers = serializers;
			}

			public DatabaseResultTable Current => new DatabaseResultTable(_dataset.Tables[_index], _parent, _serializers);

			object IEnumerator.Current => new DatabaseResultTable(_dataset.Tables[_index], _parent, _serializers);

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
