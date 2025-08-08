using DatabaseSharp.Attributes;
using DatabaseSharp.Serializers;

namespace DatabaseSharp.Tests.TestModels
{
	public class TestClass2
	{
		[DatabaseSharp(ColumnName = "col1", Serializer = DatabaseJsonSerializer.SerializerName)]
		public TestClass Test { get; set; } = new TestClass();
		[DatabaseSharp(ColumnName = "col2", Serializer = DatabaseJsonSerializer.SerializerName)]
		public ITestInterface Test2 { get; set; } = new TestClass3();
	}
}
