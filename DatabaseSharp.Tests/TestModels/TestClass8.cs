using DatabaseSharp.Attributes;

namespace DatabaseSharp.Tests.TestModels
{
	public class TestClass8
	{
		public string Name { get; set; }
		public string Value { get; set; }
		[DatabaseSharp(FillTable = 1, ColumnName = "val2")]
		public List<string> Items { get; set; }
	}
}
