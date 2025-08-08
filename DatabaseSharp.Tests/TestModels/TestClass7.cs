using DatabaseSharp.Attributes;
using DatabaseSharp.Serializers;

namespace DatabaseSharp.Tests.TestModels
{
	public class TestClass7
	{
		public int? Nullable1 { get; set; }
		[DatabaseSharp(Serializer = DatabaseJsonSerializer.SerializerName)]
		public List<TestClass6>? Nullable2 { get; set; }
	}
}
