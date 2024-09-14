using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace DatabaseSharp.Serializers
{
	public class DatabaseEnumSerializer : IDatabaseSerializer
	{
		/// <summary>
		/// Name of the JSON serializer
		/// </summary>
		public const string SerializerName = "ENUM";

		public dynamic Deserialise(string text, Type asType) => Enum.Parse(asType, text);
		public string Serialize(dynamic item) => $"{(int)item}";
	}
}
