using DatabaseSharp.Helpers;
using DatabaseSharp.Models;
using DatabaseSharp.Serializers;

namespace DatabaseSharp
{
	/// <summary>
	/// Base implementation for a DatabaseSharp client
	/// </summary>
	public abstract class BaseDBClient : IDBClient
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
		/// Main constructor
		/// </summary>
		/// <param name="connectionString"></param>
		public BaseDBClient(string connectionString)
		{
			ConnectionString = connectionString;
			Serializers = new Dictionary<string, IDatabaseSerializer>()
			{
				{ DatabaseJsonSerializer.SerializerName, new DatabaseJsonSerializer() },
				{ DatabaseEnumSerializer.SerializerName, new DatabaseEnumSerializer() },
				{ DatabaseStrBoolSerializer.SerializerName, new DatabaseStrBoolSerializer() },
				{ DatabaseBase64Serializer.SerializerName, new DatabaseBase64Serializer() },
				{ DatabaseStrListSerializer.SerializerName, new DatabaseStrListSerializer() },
				{ DatabaseGuidListSerializer.SerializerName, new DatabaseGuidListSerializer() },
				{ DatabaseDefaultSerializer.SerializerName, new DatabaseDefaultSerializer() },
			};
		}

		/// <summary>
		/// Execute a STP with a set of parameters (if any).
		/// </summary>
		/// <param name="procedureName"></param>
		/// <param name="parameters"></param>
		/// <param name="token"></param>
		/// <returns></returns>
		public abstract Task<DatabaseResult> ExecuteAsync(string procedureName, List<ISQLParameter>? parameters = null, CancellationToken? token = null);

		/// <summary>
		/// Execute a STP with a object that will be turned into parameters
		/// </summary>
		/// <param name="procedureName"></param>
		/// <param name="item"></param>
		/// <param name="token"></param>
		/// <returns></returns>
		public async Task<DatabaseResult> ExecuteAsync(string procedureName, object item, CancellationToken? token = null) => await ExecuteAsync(procedureName, ParameterHelpers.GenerateParametersFromObject(item, Serializers), token);

		/// <summary>
		/// Execute some SQL query.
		/// </summary>
		/// <param name="freeSql"></param>
		/// <param name="token"></param>
		/// <returns></returns>
		public abstract Task<DatabaseResult> ExecuteFreeAsync(string freeSql, CancellationToken? token = null);
	}
}
