using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

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
