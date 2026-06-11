using DatabaseSharp.Models;
using DatabaseSharp.Serializers;

namespace DatabaseSharp
{
	/// <summary>
	/// General interface for the database client
	/// </summary>
	public interface IDBClient
	{
		/// <summary>
		/// Connection string to the database
		/// </summary>
		public string ConnectionString { get; set; }

		/// <summary>
		/// Set of optional property serializers
		/// </summary>
		public Dictionary<string, IDatabaseSerializer> Serializers { get; }

		/// <summary>
		/// Execute a STP with a set of parameters (if any).
		/// </summary>
		/// <param name="procedureName"></param>
		/// <param name="parameters"></param>
		/// <param name="token"></param>
		/// <returns></returns>
		public Task<DatabaseResult> ExecuteAsync(string procedureName, List<ISQLParameter>? parameters = null, CancellationToken? token = null);

		/// <summary>
		/// Execute a STP with a object that will be turned into parameters
		/// </summary>
		/// <param name="procedureName"></param>
		/// <param name="item"></param>
		/// <param name="token"></param>
		/// <returns></returns>
		public Task<DatabaseResult> ExecuteAsync(string procedureName, object item, CancellationToken? token = null);

		/// <summary>
		/// Execute some SQL query.
		/// </summary>
		/// <param name="freeSql"></param>
		/// <param name="token"></param>
		/// <returns></returns>
		public Task<DatabaseResult> ExecuteFreeAsync(string freeSql, CancellationToken? token = null);
	}
}
