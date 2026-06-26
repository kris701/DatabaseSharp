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
	public class DatabaseStrListSerializerTests
	{
		public class InputObject
		{
			[DatabaseSharp(Serializer = DatabaseStrListSerializer.SerializerName)]
			public List<string> Col1 { get; set; } = new List<string>();
			[DatabaseSharp(Serializer = DatabaseStrListSerializer.SerializerName)]
			public List<string>? Col2 { get; set; } = null;
		}

		public class InputObject2
		{
			public string Other { get; set; }
			[DatabaseSharp(Serializer = DatabaseStrListSerializer.SerializerName)]
			public List<string> Col1 { get; set; } = new List<string>();
		}

		[TestMethod]
		public void Can_Serialize()
		{
			// ARRANGE
			var serializers = new Dictionary<string, IDatabaseSerializer>()
			{
				{ DatabaseStrListSerializer.SerializerName, new DatabaseStrListSerializer() }
			};
			var input = new InputObject()
			{
				Col1 = new List<string>()
				{
					"ab",
					"35u90af",
					"aaaaa"
				},
				Col2 = null
			};
			var expected1 = string.Join(';', input.Col1);

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
				{ DatabaseStrListSerializer.SerializerName, new DatabaseStrListSerializer() }
			};
			var input = new InputObject2()
			{
				Other = "some val",
				Col1 = new List<string>()
				{
					"1",
					"aaaaa   QKGS"
				}
			};
			var expected1 = string.Join(';', input.Col1);

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
				{ DatabaseStrListSerializer.SerializerName, new DatabaseStrListSerializer() }
			};

			var input = new InputObject()
			{
				Col1 = new List<string>()
				{
					"ab",
					"35u90af",
					"aaaaa"
				},
				Col2 = null
			};
			var input1 = string.Join(';', input.Col1);

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
			Assert.AreEqual(input1, string.Join(';', model.Col1));
			Assert.AreEqual(null, model.Col2);
		}
	}
}
