using DatabaseSharp.Models;
using System.Reflection;

namespace DatabaseSharp.Serializers
{
	/// <summary>
	/// Interface for converting data back and forth between C# and SQL server
	/// </summary>
	public interface IDatabaseSerializer
	{
		/// <summary>
		/// The data type of the given item in the database
		/// </summary>
		public Type? OverrideDatabaseType { get; }
		/// <summary>
		/// Deserialize the given property
		/// </summary>
		/// <param name="propInfo"></param>
		/// <param name="row"></param>
		/// <param name="source"></param>
		/// <returns></returns>
		public dynamic? Deserialise(PropertyInfo propInfo, DatabaseResultRow row, DatabaseResult source);
		/// <summary>
		/// Serialize the given property to a parameter
		/// </summary>
		/// <param name="item"></param>
		/// <param name="propInfo"></param>
		/// <returns></returns>
		public ISQLParameter Serialize(object item, PropertyInfo propInfo);
	}
}
