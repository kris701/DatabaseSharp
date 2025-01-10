using DatabaseSharp.Models;

namespace DatabaseSharp.Tests.TestModels
{
	public class TestClass3 : ITestInterface
	{
		public Guid? ID { get; set; } = new Guid("062f1af2-c85a-4a17-aaf0-c8b77ab92dbd");
		public string Name { get; set; } = "ad";
		[DatabaseSharp(ParameterName = "even_longer_name")]
		public string LongName { get; set; } = "asdasd";
		public TimeSpan Time {  get; set; }
	}
}
