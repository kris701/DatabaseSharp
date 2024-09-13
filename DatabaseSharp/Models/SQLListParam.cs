using Microsoft.SqlServer.Server;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseSharp.Models
{
	/// <summary>
	/// A list parameter given to a STP for a SQL database
	/// </summary>
	public class SQLListParam<T> : ISQLParameter, IListHandler
	{
		/// <summary>
		/// Name of the parameter
		/// </summary>
		public string Name { get; set; }
		/// <summary>
		/// Name of the column to put the data into
		/// </summary>
		public string TableColumnName { get; set; }
		/// <summary>
		/// Name of the type to send it as
		/// </summary>
		public string DatabaseTypeName { get; set; }
		/// <summary>
		/// Value of the parameter
		/// </summary>
		public List<T> Values { get; set; }

		/// <summary>
		/// Main constructor
		/// </summary>
		/// <param name="name"></param>
		/// <param name="values"></param>
		/// <param name="tableColumnName"></param>
		/// <param name="databaseTypeName"></param>
		public SQLListParam(string name, List<T> values, string tableColumnName = "", string databaseTypeName = "")
		{
			Name = name;
			Values = values;
			TableColumnName = tableColumnName;
			DatabaseTypeName = databaseTypeName;
		}

		public DataTable CreateDataTable()
		{
			DataTable table = new DataTable();
			table.Columns.Add(TableColumnName, typeof(T));
			foreach (var value in Values)
				table.Rows.Add(value);
			return table;
		}
		
		public override int GetHashCode()
		{
			return HashCode.Combine(Name, Values);
		}

		public override bool Equals(object? obj)
		{
			if (obj is SQLParam other)
			{
				if (Name != other.Name) return false;
				if (Values != other.Value) return false;
				return true;
			}
			return false;
		}
	}
}
