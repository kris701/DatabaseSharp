using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseSharp.Models
{
	/// <summary>
	/// Attribute to ignore a property
	/// </summary>
	[AttributeUsage(AttributeTargets.Property)]
	public class DatabaseSharpIgnoreAttribute : Attribute
	{
		/// <summary>
		/// Ignore the automatic convertion of this property into SQL STP parameters
		/// </summary>
		public bool IgnoreAsParameter { get; set; } = true;
		/// <summary>
		/// Ignore the automatic convertion from datatable to class object.
		/// </summary>
		public bool IgnoreAsFill { get; set; } = true;
	}
}
