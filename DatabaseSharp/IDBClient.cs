using DatabaseSharp.Models;

namespace DatabaseSharp
{
	/// <summary>
	/// General interface for the database client
	/// </summary>
	public interface IDBClient
	{
		/// <summary>
		/// Execute a STP with a set of parameters (if any).
		/// </summary>
		/// <param name="procedureName"></param>
		/// <param name="parameters"></param>
		/// <returns></returns>
		public Task<DatabaseResult> ExecuteAsync(string procedureName, List<ISQLParameter>? parameters = null);
	}
}
