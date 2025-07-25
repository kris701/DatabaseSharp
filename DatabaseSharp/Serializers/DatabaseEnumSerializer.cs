namespace DatabaseSharp.Serializers
{
	public class DatabaseEnumSerializer : IDatabaseSerializer
	{
		/// <summary>
		/// Name of the JSON serializer
		/// </summary>
		public const string SerializerName = "ENUM";

		public Type DatabaseType { get; } = typeof(int);
		public dynamic Deserialise(string text, Type asType) => Enum.Parse(asType, text);
		public string Serialize(dynamic item, Type asType) => $"{(int)item}";
	}
}
