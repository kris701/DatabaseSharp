using DatabaseSharp.Serializers;

namespace DatabaseSharp.Attributes
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
		public string? ParameterName { get; set; } = null;

		/// <summary>
		/// Name of the column in the database
		/// </summary>
		public string? ColumnName { get; set; } = null;

		/// <summary>
		/// Optional serializer for property to string and vise versa
		/// </summary>
		public string? Serializer { get; set; } = DatabaseDefaultSerializer.SerializerName;

		/// <summary>
		/// If a property needs to get its result (such as a list) from another table than the current one filling from, set the index here.
		/// </summary>
		public int FillTable { get; set; } = -1;
	}
}
