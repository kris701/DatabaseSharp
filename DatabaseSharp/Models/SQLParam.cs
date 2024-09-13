namespace DatabaseSharp.Models
{
	/// <summary>
	/// A parameter given to a STP for a SQL database
	/// </summary>
	public class SQLParam : ISQLParameter
	{
		/// <summary>
		/// Name of the parameter
		/// </summary>
		public string Name { get; set; }
		/// <summary>
		/// Value of the parameter
		/// </summary>
		public object Value { get; set; }

		/// <summary>
		/// Main constructor
		/// </summary>
		/// <param name="name"></param>
		/// <param name="value"></param>
		public SQLParam(string name, object value)
		{
			Name = name;
			Value = value;
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
