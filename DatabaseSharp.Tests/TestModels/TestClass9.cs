using DatabaseSharp.Attributes;

namespace DatabaseSharp.Tests.TestModels
{
	public class TestClass9
	{
		public string Name { get; set; }
		public bool Value { get; set; }
		[DatabaseSharp(FillTable = 1)]
		public List<TestClass3> Items { get; set; }
		[DatabaseSharp(FillTable = 2)]
		public string Value3 { get; set; }
	}
}
