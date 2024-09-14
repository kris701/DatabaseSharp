using System.Collections;
using System.Data;

namespace DatabaseSharp.Models
{
	/// <summary>
	/// A list parameter given to a STP for a SQL database
	/// </summary>
	public class SQLListParam : ISQLParameter, IListHandler
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
		public IList Values { get; set; }

		/// <summary>
		/// Main constructor
		/// </summary>
		/// <param name="name"></param>
		/// <param name="values"></param>
		/// <param name="tableColumnName"></param>
		/// <param name="databaseTypeName"></param>
		public SQLListParam(string name, IList values, string tableColumnName = "", string databaseTypeName = "")
		{
			Name = name;
			Values = values;
			TableColumnName = tableColumnName;
			DatabaseTypeName = databaseTypeName;
		}

		/// <summary>
		/// Generate a datatable from this object
		/// </summary>
		/// <returns></returns>
		public DataTable CreateDataTable()
		{
			DataTable table = new DataTable();
			if (Values.Count == 0)
				return table;
			table.Columns.Add(TableColumnName, Values[0].GetType());
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
