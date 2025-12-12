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
	public class DatabaseStrBoolSerializerTests
	{
		public class InputObject
		{
			[DatabaseSharp(Serializer = DatabaseStrBoolSerializer.SerializerName)]
			public string TrueStringBool { get; set; } = "Y";
			[DatabaseSharp(Serializer = DatabaseStrBoolSerializer.SerializerName)]
			public string FalseStringBool { get; set; } = "N";
		}

		public class InputObject2
		{
			public string Other { get; set; }
			[DatabaseSharp(Serializer = DatabaseStrBoolSerializer.SerializerName)]
			public List<string> StringBoolValues { get; set; } = new List<string>();
		}

		[TestMethod]
		public void Can_Serialize()
		{
			// ARRANGE
			var serializers = new Dictionary<string, IDatabaseSerializer>()
			{
				{ DatabaseStrBoolSerializer.SerializerName, new DatabaseStrBoolSerializer() }
			};
			var input = new InputObject()
			{
			};

			// ACT
			var param = ParameterHelpers.GenerateParametersFromObject(input, serializers);

			// ASSERT
			Assert.IsNotNull(param);
			Assert.AreEqual(2, param.Count);
			Assert.AreEqual((param[0] as SQLParam)!.Value, true);
			Assert.AreEqual((param[1] as SQLParam)!.Value, false);
		}

		[TestMethod]
		public void Can_Serialize2()
		{
			// ARRANGE
			var serializers = new Dictionary<string, IDatabaseSerializer>()
			{
				{ DatabaseDefaultSerializer.SerializerName, new DatabaseDefaultSerializer() },
				{ DatabaseStrBoolSerializer.SerializerName, new DatabaseStrBoolSerializer() }
			};
			var input = new InputObject2()
			{
				Other = "some val",
				StringBoolValues = new List<string>()
				{
					"Y",
					"N",
					"Y"
				}
			};
			var expected1 = new List<bool>() { true, false, true };

			// ACT
			var param = ParameterHelpers.GenerateParametersFromObject(input, serializers);

			// ASSERT
			Assert.IsNotNull(param);
			Assert.AreEqual(2, param.Count);
			Assert.AreEqual((param[0] as SQLParam)!.Value, input.Other);

			for (int i = 0; i < (param[1] as SQLListParam)!.Values.Count; i++)
				Assert.AreEqual(expected1[i], (param[1] as SQLListParam)!.Values[i]);
		}

		[TestMethod]
		public void Can_Deserialize()
		{
			// ARRANGE
			var serializers = new Dictionary<string, IDatabaseSerializer>()
			{
				{ DatabaseStrBoolSerializer.SerializerName, new DatabaseStrBoolSerializer() }
			};

			var dataset = new DataSet();
			var table = new DataTable();
			table.Columns.Add(new DataColumn("TrueStringBool", typeof(bool)));
			table.Columns.Add(new DataColumn("FalseStringBool", typeof(bool)));
			table.Rows.Add(table.NewRow());
			table.Rows[0].SetField<bool>(table.Columns[0], false);
			table.Rows[0].SetField<bool>(table.Columns[1], true);
			dataset.Tables.Add(table);
			var result = new DatabaseResult(dataset, serializers);

			// ACT
			var row = result[0][0];
			var model = row.Fill<InputObject>();

			// ASSERT
			Assert.IsTrue(model.TrueStringBool == "N");
			Assert.IsTrue(model.FalseStringBool == "Y");
		}
	}
}
