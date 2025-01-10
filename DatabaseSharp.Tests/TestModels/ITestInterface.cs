using System.Text.Json.Serialization;

namespace DatabaseSharp.Tests.TestModels
{
	[JsonPolymorphic(TypeDiscriminatorPropertyName = "Test")]
	[JsonDerivedType(typeof(TestClass3), typeDiscriminator: "abc")]
	public interface ITestInterface
	{
		public Guid ID { get; set; }
		public string Name { get; set; }
	}
}
