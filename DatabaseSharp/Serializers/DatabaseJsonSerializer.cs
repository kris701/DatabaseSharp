using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace DatabaseSharp.Serializers
{
	public class DatabaseJsonSerializer : IDatabaseSerializer
	{
		/// <summary>
		/// Name of the JSON serializer
		/// </summary>
		public const string SerializerName = "JSON";

		public dynamic Deserialise(string text, Type asType) => JsonSerializer.Deserialize(text, asType);
		public string Serialize(dynamic item, Type asType) => JsonSerializer.Serialize(item, asType);
	}
}
