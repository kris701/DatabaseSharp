using DatabaseSharp.Models;
using DatabaseSharp.Serializers;
using System.Text.Json;

namespace DatabaseSharp.Tests.TestModels
{
	public class TestClass3 : ITestInterface
	{
		public Guid ID { get; set; } = new Guid("062f1af2-c85a-4a17-aaf0-c8b77ab92dbd");
		public string Name { get; set; } = "ad";
		public string LongName { get; set; } = "asdasd";
	}
}
