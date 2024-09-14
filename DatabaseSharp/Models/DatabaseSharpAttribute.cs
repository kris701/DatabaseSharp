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
		public string? ParameterName { get; set; } = null;

		/// <summary>
		/// Name of the column in the database
		/// </summary>
		public string? ColumnName { get; set; } = null;

		/// <summary>
		/// Optional type name
		/// </summary>
		public string? TypeName { get; set; } = null;

		/// <summary>
		/// Optional serializer for property to string and vise versa
		/// </summary>
		public string? Serializer { get; set; } = null;
	}
}
