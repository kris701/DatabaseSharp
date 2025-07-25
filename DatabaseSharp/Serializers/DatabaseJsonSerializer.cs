using System.Text.Json;

namespace DatabaseSharp.Serializers
{
	public class DatabaseJsonSerializer : IDatabaseSerializer
	{
		/// <summary>
		/// Name of the JSON serializer
		/// </summary>
		public const string SerializerName = "JSON";

		public Type DatabaseType { get; } = typeof(string);
		public dynamic Deserialise(string text, Type asType) => JsonSerializer.Deserialize(text, asType);
		public string Serialize(dynamic item, Type asType) => JsonSerializer.Serialize(item, asType);
	}
}
