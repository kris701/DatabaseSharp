using DatabaseSharp.Models;

namespace DatabaseSharp.Tests.TestModels
{
	public class TestClass
	{
		[DatabaseSharpIgnore(IgnoreAsParameter = true, IgnoreAsFill = false)]
		public string Name { get; set; } = "adfgasf";

		[DatabaseSharp(ColumnName = "col2", ParameterName = "SomeParam")]
		public int SomeValue { get; set; } = 12354;

		[DatabaseSharp(ColumnName = "col3", ParameterName = "SomeOtherParam")]
		public Guid? ID { get; set; } = new Guid("bf737802-4b09-4525-90ec-af77dd297e35");

		[DatabaseSharpIgnore]
		public Guid MoreIDs { get; set; } = Guid.Empty;

		[DatabaseSharpIgnore(IgnoreAsParameter = false)]
		public List<Guid> Guids { get; set; } = new List<Guid>();
	}
}
