using DatabaseSharp.Serializers;
using System.Data;

namespace DatabaseSharp.Models
{
	/// <summary>
	/// Interface to handle dynamic data convertion to Datatables
	/// </summary>
	public interface IListHandler
	{
		/// <summary>
		/// Generate a datatable from this object
		/// </summary>
		/// <returns></returns>
		public DataTable CreateDataTable(Dictionary<string, IDatabaseSerializer> serializers);
	}
}
