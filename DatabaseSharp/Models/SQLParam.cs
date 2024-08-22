namespace DatabaseSharp.Models
{
	/// <summary>
	/// A parameter given to a STP for a SQL database
	/// </summary>
	public class SQLParam
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
	}
}
