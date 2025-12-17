using DatabaseSharp.Helpers;
using DatabaseSharp.Models;
using DatabaseSharp.Serializers;
using DatabaseSharp.Tests.TestModels;
using Microsoft.IdentityModel.Tokens;
using System.Data.SqlTypes;

namespace DatabaseSharp.Tests.Helpers
{
	[TestClass]
	public class ParameterHelpersTests
	{
		[TestMethod]
		public void Can_GenerateSTPParameters()
		{
			// ARRANGE
			var item = new TestClass();

			// ACT
			var parameters = ParameterHelpers.GenerateParametersFromObject(
				item,
				new Dictionary<string, IDatabaseSerializer>()
				{
					{ DatabaseDefaultSerializer.SerializerName, new DatabaseDefaultSerializer() }
				});

			// ASSERT
			Assert.IsNotNull(parameters);
			Assert.AreEqual(3, parameters.Count);
			Assert.IsTrue(parameters.Any(x => x.Name == "SomeParam"));
			Assert.IsTrue(parameters.Any(x => x.Name == "SomeOtherParam"));
			Assert.IsTrue(parameters.Any(x => x.Name == "Guids"));
		}

		[TestMethod]
		public void Can_GenerateSTPParameters2()
		{
			// ARRANGE
			var item = new TestClass6()
			{
				Nullable1 = 50,
				Nullable2 = null
			};

			// ACT
			var parameters = ParameterHelpers.GenerateParametersFromObject(
				item,
				new Dictionary<string, IDatabaseSerializer>() {
					{ DatabaseDefaultSerializer.SerializerName, new DatabaseDefaultSerializer() },
					{ DatabaseEnumSerializer.SerializerName, new DatabaseEnumSerializer() }
				});

			// ASSERT
			Assert.IsNotNull(parameters);
			Assert.IsTrue(parameters.Any(x => x.Name == "Nullable1"));
			Assert.IsTrue(parameters.Any(x => x.Name == "Nullable2"));
		}

		[TestMethod]
		public void Can_GenerateSTPParameters3()
		{
			// ARRANGE
			var item = new TestClass7()
			{
				Nullable1 = 50,
				Nullable2 = new List<TestClass6>()
				{
					new TestClass6()
				}
			};

			// ACT
			var parameters = ParameterHelpers.GenerateParametersFromObject(
				item,
				new Dictionary<string, IDatabaseSerializer>() {
					{ DatabaseDefaultSerializer.SerializerName, new DatabaseDefaultSerializer() },
					{ DatabaseJsonSerializer.SerializerName, new DatabaseJsonSerializer() }
				});

			// ASSERT
			Assert.IsNotNull(parameters);
			Assert.IsTrue(parameters.Any(x => x.Name == "Nullable1"));
			Assert.IsTrue(parameters.Any(x => x.Name == "Nullable2"));
		}

		[TestMethod]
		public void Can_GenerateSTPParameters4()
		{
			// ARRANGE
			var items = new List<TestClass11>(){
				new TestClass11()
				{
					Count = 50,
					Status = 10,
					BigData = [10,2,5]
				},
				new TestClass11()
				{
					Count = 500,
					Status = 1,
					BigData = [1,5]
				}
			};
			
			var serializer = new Dictionary<string, IDatabaseSerializer>() {
					{ DatabaseDefaultSerializer.SerializerName, new DatabaseDefaultSerializer() }
				};
			var container = new Container<TestClass11>(items);

			// ACT
			var parameters = ParameterHelpers.GenerateParametersFromObject(
				container,
				serializer);

			// ASSERT
			Assert.IsNotNull(parameters);
			Assert.HasCount(1, parameters);
			var containerParam = (SQLListParam)parameters[0];
			Assert.HasCount(2, containerParam.Values);
			var dataTable = containerParam.CreateDataTable(serializer);
			Assert.IsTrue(dataTable.Columns[2].DataType == typeof(SqlBinary));
		}

		public class Container<T>
		{
			public List<T> Items { get; set; }

			public Container(List<T> items)
			{
				Items = items;
			}
		}
	}
}
