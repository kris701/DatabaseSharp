using System.Data;

namespace DatabaseSharp.Models
{
	/// <summary>
	/// Interface to handle dynamic data convertion to Datatables
	/// </summary>
	public interface IListHandler
	{
		/// <summary>
		/// Name of the type to send it as
		/// </summary>
		public string DatabaseTypeName { get; set; }

		/// <summary>
		/// Generate a datatable from this object
		/// </summary>
		/// <returns></returns>
		public DataTable CreateDataTable();
	}
}
