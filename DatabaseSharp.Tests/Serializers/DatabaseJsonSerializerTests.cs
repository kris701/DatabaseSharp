using DatabaseSharp.Helpers;
using DatabaseSharp.Models;
using DatabaseSharp.Serializers;
using DatabaseSharp.Tests.TestModels;
using System.Data;
using System.Data.SqlClient;
using System.Text.Json;
using static DatabaseSharp.Tests.Serializers.DatabaseEnumSerializerTests;

namespace DatabaseSharp.Tests.Serializers
{
	[TestClass]
	public class DatabaseJsonSerializerTests
	{
		public class InputObject
		{
			[DatabaseSharp(Serializer = DatabaseJsonSerializer.SerializerName)]
			public TestClass Col1 { get; set; } = new TestClass();
			[DatabaseSharp(Serializer = DatabaseJsonSerializer.SerializerName)]
			public TestClass7? Col2 { get; set; } = null;
		}

		public class InputObject2
		{
			public string Other { get; set; }
			[DatabaseSharp(Serializer = DatabaseJsonSerializer.SerializerName)]
			public List<TestClass> Col1 { get; set; } = new List<TestClass>();
		}

		[TestMethod]
		public void Can_Serialize()
		{
			// ARRANGE
			var serializers = new Dictionary<string, IDatabaseSerializer>()
			{
				{ DatabaseJsonSerializer.SerializerName, new DatabaseJsonSerializer() }
			};
			var input = new InputObject()
			{
			};
			var expected1 = JsonSerializer.Serialize(input.Col1);

			// ACT
			var param = ParameterHelpers.GenerateParametersFromObject(input, serializers);

			// ASSERT
			Assert.IsNotNull(param);
			Assert.AreEqual(2, param.Count);
			Assert.AreEqual((param[0] as SQLParam)!.Value, expected1);
			Assert.AreEqual((param[1] as SQLParam)!.Value, null);
		}

		[TestMethod]
		public void Can_Serialize2()
		{
			// ARRANGE
			var serializers = new Dictionary<string, IDatabaseSerializer>()
			{
				{ DatabaseDefaultSerializer.SerializerName, new DatabaseDefaultSerializer() },
				{ DatabaseJsonSerializer.SerializerName, new DatabaseJsonSerializer() }
			};
			var input = new InputObject2()
			{
				Other = "some val"
			};
			var expected1 = JsonSerializer.Serialize(input.Col1);

			// ACT
			var param = ParameterHelpers.GenerateParametersFromObject(input, serializers);

			// ASSERT
			Assert.IsNotNull(param);
			Assert.AreEqual(2, param.Count);
			Assert.AreEqual((param[0] as SQLParam)!.Value, input.Other);
			Assert.AreEqual((param[1] as SQLParam)!.Value, expected1);
		}

		[TestMethod]
		public void Can_Deserialize()
		{
			// ARRANGE
			var serializers = new Dictionary<string, IDatabaseSerializer>()
			{
				{ DatabaseJsonSerializer.SerializerName, new DatabaseJsonSerializer() }
			};

			var input1 = JsonSerializer.Serialize(new TestClass());

			var dataset = new DataSet();
			var table = new DataTable();
			table.Columns.Add(new DataColumn("Col1", typeof(string)));
			table.Columns.Add(new DataColumn("Col2", typeof(string)));
			table.Rows.Add(table.NewRow());
			table.Rows[0].SetField<string>(table.Columns[0], input1);
			table.Rows[0].SetField<string?>(table.Columns[1], null);
			dataset.Tables.Add(table);
			var result = new DatabaseResult(dataset, serializers);

			// ACT
			var row = result[0][0];
			var model = row.Fill<InputObject>();

			// ASSERT
			Assert.AreEqual(input1, JsonSerializer.Serialize(model.Col1));
			Assert.AreEqual(null, model.Col2);
		}
	}
}
