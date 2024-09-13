using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseSharp.Models
{
	/// <summary>
	/// Attribute to override names for automatic database serialization and deserialization
	/// </summary>
	[AttributeUsage(AttributeTargets.Property)]
	public class DatabaseSharpAttribute : Attribute
    {
		/// <summary>
		/// Name of the input parameter for the STP
		/// </summary>
		public string ParameterName { get; set; } = "";

		/// <summary>
		/// Name of the column in the database
		/// </summary>
		public string ColumnName { get; set; } = "";

		/// <summary>
		/// Optional type name
		/// </summary>
		public string TypeName { get; set; } = "";
	}
}
