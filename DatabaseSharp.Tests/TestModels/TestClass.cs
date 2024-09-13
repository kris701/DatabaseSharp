using DatabaseSharp.Models;

namespace DatabaseSharp.Tests.TestModels
{
	public class TestClass
	{
		public string Name { get; set; } = "";

		[DatabaseSharp(ColumnName = "col2")]
		public int SomeValue { get; set; } = 0;

		[DatabaseSharp(ColumnName = "col3")]
		public Guid ID { get; set; } = Guid.Empty;
	}
}
