using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
