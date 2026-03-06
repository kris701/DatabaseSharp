using DatabaseSharp.Attributes;
using DatabaseSharp.Helpers;
using DatabaseSharp.Models;
using DatabaseSharp.Serializers;
using DatabaseSharp.Tests.TestModels;
using System.Data;
using System.Text.Json;

namespace DatabaseSharp.Tests.Serializers
{
	[TestClass]
	public class DatabaseBase64SerializerTests
	{
		public class InputObject
		{
			[DatabaseSharp(Serializer = DatabaseBase64Serializer.SerializerName)]
			public string Col1 { get; set; } = "abc";
			[DatabaseSharp(Serializer = DatabaseBase64Serializer.SerializerName)]
			public string? Col2 { get; set; } = "11111";
		}

		[TestMethod]
		public void Can_Serialize()
		{
			// ARRANGE
			var serializers = new Dictionary<string, IDatabaseSerializer>()
			{
				{ DatabaseBase64Serializer.SerializerName, new DatabaseBase64Serializer() }
			};
			var input = new InputObject()
			{
			};
			var expected1 = "YWJj";
			var expected2 = "MTExMTE=";

			// ACT
			var param = ParameterHelpers.GenerateParametersFromObject(input, serializers);

			// ASSERT
			Assert.IsNotNull(param);
			Assert.AreEqual(2, param.Count);
			Assert.AreEqual((param[0] as SQLParam)!.Value, expected1);
			Assert.AreEqual((param[1] as SQLParam)!.Value, expected2);
		}

		[TestMethod]
		public void Can_Deserialize()
		{
			// ARRANGE
			var serializers = new Dictionary<string, IDatabaseSerializer>()
			{
				{ DatabaseBase64Serializer.SerializerName, new DatabaseBase64Serializer() }
			};

			var input1 = "abc";
			var input2 = "aasdasdsdsbc";

			var input1Base = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(input1));
			Assert.AreNotEqual(input1, input1Base);
			var input2Base = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(input2));
			Assert.AreNotEqual(input2, input2Base);

			var dataset = new DataSet();
			var table = new DataTable();
			table.Columns.Add(new DataColumn("Col1", typeof(string)));
			table.Columns.Add(new DataColumn("Col2", typeof(string)));
			table.Rows.Add(table.NewRow());
			table.Rows[0].SetField<string>(table.Columns[0], input1Base);
			table.Rows[0].SetField<string>(table.Columns[1], input2Base);
			dataset.Tables.Add(table);
			var result = new DatabaseResult(dataset, serializers);

			// ACT
			var row = result[0][0];
			var model = row.Fill<InputObject>();

			// ASSERT
			Assert.AreEqual(input1, model.Col1);
			Assert.AreEqual(input2, model.Col2);
		}
	}
}
