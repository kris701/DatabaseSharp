using DatabaseSharp.Models;

namespace DatabaseSharp.Tools
{
	/// <summary>
	/// Base implementation model for a list for a model serialization/deserialization.
	/// </summary>
	/// <typeparam name="TIn"></typeparam>
	/// <typeparam name="TOut"></typeparam>
	/// <typeparam name="TEmpty"></typeparam>
	/// <param name="client"></param>
	/// <param name="targetSTP"></param>
	public class BaseListSerializerModel<TIn, TOut, TEmpty>(IDBClient client, string targetSTP)
		where TIn : notnull, new()
		where TOut : class, new()
		where TEmpty : class, new()
	{
		/// <summary>
		/// Name of the STP
		/// </summary>
		public string TargetSTP { get; set; } = targetSTP;

		/// <summary>
		/// Additional action to run before communicating with the database
		/// </summary>
		public Func<TIn, Task>? PreExecute = null;
		/// <summary>
		/// Additional action to run after communicating with the database
		/// </summary>
		public Func<TIn, List<TOut>, Task>? PostExecute = null;
		/// <summary>
		/// Override for how to use the Fill deserialization method.
		/// If none given, the default DatabaseSharp one is used.
		/// </summary>
		public Func<DatabaseResult, TIn, Task<List<TOut>>>? FillOverride = null;

		private readonly IDBClient _dbClient = client;

		/// <summary>
		/// Execute the STP
		/// </summary>
		/// <param name="input"></param>
		/// <returns></returns>
		public async Task<List<TOut>> ExecuteAsync(TIn input)
		{
			if (PreExecute != null)
				await PreExecute(input);
			var result = await _dbClient.ExecuteAsync(TargetSTP, input);
			if (typeof(TOut) == typeof(TEmpty))
			{
				if (PostExecute != null)
					await PostExecute(input, new());
				return new();
			}
			if (result.Count == 0)
				return new();
			if (result[0].Count == 0)
				return new();
			if (FillOverride != null)
			{
				var model = await FillOverride(result, input);
				if (PostExecute != null)
					await PostExecute(input, model);
				return model;
			}
			else
			{
				var model = result[0].FillAll<TOut>();
				if (PostExecute != null)
					await PostExecute(input, model);
				return model;
			}
		}
	}
}
