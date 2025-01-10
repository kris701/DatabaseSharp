using DatabaseSharp.Helpers;
using DatabaseSharp.Models;
using DatabaseSharp.Tests.TestModels;

namespace DatabaseSharp.Tests.Models
{
	[TestClass]
	public class SQLListParamTests
	{
		[TestMethod]
		public void Can_CreateDatatableOfSimples()
		{
			// ARRANGE
			var item = new TestClass()
			{
				Guids = new List<Guid>()
				{
					new Guid("1ebbfeca-147b-4524-855f-85edb27d78c4"),
					new Guid("1b31de58-9cd6-4c19-84dc-9e71aa8cc641")
				}
			};
			var parameters = ParameterHelpers.GenerateParametersFromObject(item, new Dictionary<string, DatabaseSharp.Serializers.IDatabaseSerializer>());
			var listparam = parameters.First(x => x is SQLListParam) as SQLListParam;

			// ACT
			var table = listparam.CreateDataTable(new Dictionary<string, DatabaseSharp.Serializers.IDatabaseSerializer>());

			// ASSERT
			Assert.AreEqual(2, table.Rows.Count);
		}

		[TestMethod]
		public void Can_CreateDatatableOfComplexes()
		{
			// ARRANGE
			var item = new TestClass4()
			{
				Name = "test",
				Items = new List<TestClass3>()
				{
					new TestClass3()
					{
						ID = new Guid("841cee85-819d-4b36-bd04-1d3f9d637c61"),
						LongName = "aaaaaaa",
						Name = "a"
					},
					new TestClass3()
					{
						ID = new Guid("c05a95a5-31f9-4f51-91d9-441103b38148"),
						LongName = "aaaaaaa",
						Name = "a"
					},
					new TestClass3()
					{
						ID = new Guid("841cee85-819d-4b36-bd04-1d3f9d637c61"),
						LongName = "bbbbb",
						Name = "a"
					}
				}
			};
			var parameters = ParameterHelpers.GenerateParametersFromObject(item, new Dictionary<string, DatabaseSharp.Serializers.IDatabaseSerializer>());
			var listparam = parameters.First(x => x is SQLListParam) as SQLListParam;

			// ACT
			var table = listparam.CreateDataTable(new Dictionary<string, DatabaseSharp.Serializers.IDatabaseSerializer>());

			// ASSERT
			Assert.AreEqual(3, table.Rows.Count);
			Assert.IsTrue(table.Columns.Contains("ID"));
			Assert.IsTrue(table.Columns.Contains("Name"));
			Assert.IsTrue(table.Columns.Contains("even_longer_name"));
		}
	}
}
