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
	public class SQLListParam : ISQLParameter
	{
		/// <summary>
		/// Name of the parameter
		/// </summary>
		public string Name { get; set; }
		/// <summary>
		/// Value of the parameter
		/// </summary>
		public List<object> Value { get; set; }

		/// <summary>
		/// Main constructor
		/// </summary>
		/// <param name="name"></param>
		/// <param name="value"></param>
		public SQLListParam(string name, List<object> value)
		{
			Name = name;
			Value = value;
		}

		internal IEnumerable<SqlDataRecord> GenerateParameter()
		{
			SqlMetaData[] metaData = new SqlMetaData[1];
			metaData[0] = new SqlMetaData("Value", SqlDbType.Variant);
			SqlDataRecord record = new SqlDataRecord(metaData);
			foreach (var value in Value)
			{
				record.SetValue(0, value);
				yield return record;
			}
		}

		public override int GetHashCode()
		{
			return HashCode.Combine(Name, Value);
		}

		public override bool Equals(object? obj)
		{
			if (obj is SQLParam other)
			{
				if (Name != other.Name) return false;
				if (Value != other.Value) return false;
				return true;
			}
			return false;
		}
	}
}
