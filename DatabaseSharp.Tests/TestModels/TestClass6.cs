using DatabaseSharp.Attributes;
using DatabaseSharp.Serializers;

namespace DatabaseSharp.Tests.TestModels
{
	public enum TestEnum { Nothing, Type1, Type2 }
	public class TestClass6
	{
		public int? Nullable1 { get; set; }
		[DatabaseSharp(Serializer = DatabaseEnumSerializer.SerializerName)]
		public TestEnum? Nullable2 { get; set; }
	}
}
