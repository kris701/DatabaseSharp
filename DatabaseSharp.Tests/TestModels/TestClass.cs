using DatabaseSharp.Models;

namespace DatabaseSharp.Tests.TestModels
{
	public class TestClass
	{
		[DatabaseSharpIgnore(IgnoreAsParameter = true, IgnoreAsFill = false)]
		public string Name { get; set; } = "";

		[DatabaseSharp(ColumnName = "col2", ParameterName = "SomeParam")]
		public int SomeValue { get; set; } = 0;

		[DatabaseSharp(ColumnName = "col3", ParameterName = "SomeOtherParam")]
		public Guid ID { get; set; } = Guid.Empty;

		[DatabaseSharpIgnore]
		public Guid MoreIDs { get; set; } = Guid.Empty;
	}
}
