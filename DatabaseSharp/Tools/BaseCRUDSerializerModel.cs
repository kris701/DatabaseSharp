using DatabaseSharp.Models;

namespace DatabaseSharp.Tools
{
	/// <summary>
	/// Base implementation model for an entire CRUD interface to the database
	/// </summary>
	/// <typeparam name="TAdd"></typeparam>
	/// <typeparam name="TGetAllModel"></typeparam>
	/// <typeparam name="TModel"></typeparam>
	/// <typeparam name="TListModel"></typeparam>
	/// <typeparam name="TGetModel"></typeparam>
	/// <typeparam name="TDelete"></typeparam>
	/// <typeparam name="TEmpty"></typeparam>
	/// <param name="dbClient"></param>
	/// <param name="addSTP"></param>
	/// <param name="updateSTP"></param>
	/// <param name="getSTP"></param>
	/// <param name="getAllSTP"></param>
	/// <param name="deleteSTP"></param>
	public abstract class BaseCRUDSerializerModel<TAdd, TGetAllModel, TModel, TListModel, TGetModel, TDelete, TEmpty>(
			IDBClient dbClient,
			string addSTP,
			string updateSTP,
			string getSTP,
			string getAllSTP,
			string deleteSTP)
		where TAdd : class, new()
		where TGetAllModel : class, new()
		where TModel : class, new()
		where TListModel : class, new()
		where TGetModel : class, new()
		where TDelete : class, new()
		where TEmpty : class, new()
	{
		internal readonly IDBClient _dbClient = dbClient;

		/// <summary>
		/// Name of the add STP
		/// </summary>
		public string TargetAddSTP { get; } = addSTP;
		/// <summary>
		/// Name of the update STP
		/// </summary>
		public string TargetUpdateSTP { get; } = updateSTP;
		/// <summary>
		/// Name of the get STP
		/// </summary>
		public string TargetGetSTP { get; } = getSTP;
		/// <summary>
		/// Name of the get all STP
		/// </summary>
		public string TargetGetAllSTP { get; } = getAllSTP;
		/// <summary>
		/// Name of the delete STP
		/// </summary>
		public string TargetDeleteSTP { get; } = deleteSTP;

		/// <summary>
		/// Additional action to run before communicating with the database
		/// </summary>
		public Func<TAdd, Task>? AddPreExecute = null;
		/// <summary>
		/// Additional action to run after communicating with the database
		/// </summary>
		public Func<TAdd, TModel, Task>? AddPostExecute = null;
		/// <summary>
		/// Override for how to use the Fill deserialization method.
		/// If none given, the default DatabaseSharp one is used.
		/// </summary>
		public Func<DatabaseResult, TAdd, Task<TModel>>? AddFillOverride = null;
		/// <summary>
		/// Method to execute the Add STP
		/// </summary>
		/// <param name="input"></param>
		/// <returns></returns>
		public async Task<TModel> AddAsync(TAdd input) => await ExecuteSingleAsync(input, TargetAddSTP, AddPreExecute, AddPostExecute, AddFillOverride);

		/// <summary>
		/// Additional action to run before communicating with the database
		/// </summary>
		public Func<TModel, Task>? UpdatePreExecute = null;
		/// <summary>
		/// Additional action to run after communicating with the database
		/// </summary>
		public Func<TModel, TModel, Task>? UpdatePostExecute = null;
		/// <summary>
		/// Override for how to use the Fill deserialization method.
		/// If none given, the default DatabaseSharp one is used.
		/// </summary>
		public Func<DatabaseResult, TModel, Task<TModel>>? UpdateFillOverride = null;
		/// <summary>
		/// Method to execute the Update STP
		/// </summary>
		/// <param name="input"></param>
		/// <returns></returns>
		public async Task<TModel> UpdateAsync(TModel input) => await ExecuteSingleAsync(input, TargetUpdateSTP, UpdatePreExecute, UpdatePostExecute, UpdateFillOverride);

		/// <summary>
		/// Additional action to run before communicating with the database
		/// </summary>
		public Func<TGetModel, Task>? GetPreExecute = null;
		/// <summary>
		/// Additional action to run after communicating with the database
		/// </summary>
		public Func<TGetModel, TModel, Task>? GetPostExecute = null;
		/// <summary>
		/// Override for how to use the Fill deserialization method.
		/// If none given, the default DatabaseSharp one is used.
		/// </summary>
		public Func<DatabaseResult, TGetModel, Task<TModel>>? GetFillOverride = null;
		/// <summary>
		/// Method to execute the Get STP
		/// </summary>
		/// <param name="input"></param>
		/// <returns></returns>
		public async Task<TModel> GetAsync(TGetModel input) => await ExecuteSingleAsync(input, TargetGetSTP, GetPreExecute, GetPostExecute, GetFillOverride);

		/// <summary>
		/// Additional action to run before communicating with the database
		/// </summary>
		public Func<TGetAllModel, Task>? GetAllPreExecute = null;
		/// <summary>
		/// Additional action to run after communicating with the database
		/// </summary>
		public Func<TGetAllModel, List<TListModel>, Task>? GetAllPostExecute = null;
		/// <summary>
		/// Override for how to use the Fill deserialization method.
		/// If none given, the default DatabaseSharp one is used.
		/// </summary>
		public Func<DatabaseResult, TGetAllModel, Task<List<TListModel>>>? GetAllFillOverride = null;
		/// <summary>
		/// Method to execute the Get All STP
		/// </summary>
		/// <param name="input"></param>
		/// <returns></returns>
		public async Task<List<TListModel>> GetAllAsync(TGetAllModel input) => await ExecuteListAsync(input, TargetGetAllSTP, GetAllPreExecute, GetAllPostExecute, GetAllFillOverride);

		/// <summary>
		/// Additional action to run before communicating with the database
		/// </summary>
		public Func<TDelete, Task>? DeletePreExecute = null;
		/// <summary>
		/// Additional action to run after communicating with the database
		/// </summary>
		public Func<TDelete, TEmpty, Task>? DeletePostExecute = null;
		/// <summary>
		/// Method to execute the Delete STP
		/// </summary>
		/// <param name="input"></param>
		/// <returns></returns>
		public async Task<TEmpty> DeleteAsync(TDelete input) => await ExecuteSingleAsync(input, TargetDeleteSTP, DeletePreExecute, DeletePostExecute);

		private async Task<TOut> ExecuteSingleAsync<TIn, TOut>(
			TIn input,
			string stp,
			Func<TIn, Task>? preExecute = null,
			Func<TIn, TOut, Task>? postExecute = null,
			Func<DatabaseResult, TIn, Task<TOut>>? fillOverride = null)
			where TIn : class, new()
			where TOut : class, new()
		{
			if (preExecute != null)
				await preExecute(input);
			var result = await _dbClient.ExecuteAsync(stp, input);
			if (typeof(TOut) == typeof(TEmpty))
			{
				if (postExecute != null)
					await postExecute(input, new());
				return new();
			}
			if (result.Count == 0)
				return new();
			if (result[0].Count == 0)
				return new();
			if (fillOverride != null)
			{
				var model = await fillOverride(result, input);
				if (postExecute != null)
					await postExecute(input, model);
				return model;
			}
			else
			{
				var model = result[0][0].Fill<TOut>();
				if (postExecute != null)
					await postExecute(input, model);
				return model;
			}
		}

		private async Task<List<TOut>> ExecuteListAsync<TIn, TOut>(
			TIn input,
			string stp,
			Func<TIn, Task>? preExecute = null,
			Func<TIn, List<TOut>, Task>? postExecute = null,
			Func<DatabaseResult, TIn, Task<List<TOut>>>? fillOverride = null)
			where TIn : class, new()
			where TOut : class, new()
		{
			if (preExecute != null)
				await preExecute(input);
			var result = await _dbClient.ExecuteAsync(stp, input);
			if (typeof(TOut) == typeof(TEmpty))
			{
				if (postExecute != null)
					await postExecute(input, new());
				return new();
			}
			if (result.Count == 0)
				return new();
			if (result[0].Count == 0)
				return new();
			if (fillOverride != null)
			{
				var model = await fillOverride(result, input);
				if (postExecute != null)
					await postExecute(input, model);
				return model;
			}
			else
			{
				var model = result[0].FillAll<TOut>();
				if (postExecute != null)
					await postExecute(input, model);
				return model;
			}
		}
	}
}
