﻿namespace DatabaseSharp.Models
{
	/// <summary>
	/// A parameter given to a STP for a SQL database
	/// </summary>
	public interface ISQLParameter
	{
		/// <summary>
		/// Name of the parameter
		/// </summary>
		public string Name { get; set; }
	}
}
