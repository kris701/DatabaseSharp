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

		/// <summary>
		/// Execute a STP with a object that will be turned into parameters
		/// </summary>
		/// <param name="procedureName"></param>
		/// <param name="item"></param>
		/// <returns></returns>
		public Task<DatabaseResult> ExecuteAsync(string procedureName, object item);

		/// <summary>
		/// Automatically generate STP parameters based on a given object
		/// </summary>
		/// <param name="item"></param>
		/// <returns></returns>
		public List<ISQLParameter>? GenerateParametersFromObject(object item);
	}
}
