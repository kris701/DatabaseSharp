using DatabaseSharp.Helpers;
using DatabaseSharp.Models;
using DatabaseSharp.Serializers;
using DatabaseSharp.Tests.TestModels;
using System.Data;
using System.Data.SqlClient;
using System.Text.Json;

namespace DatabaseSharp.Tests.Serializers
{
	[TestClass]
	public class DatabaseEnumSerializerTests
	{
		public enum TestEnum { None, Value1, abc, AAAA }
		public class InputObject
		{
			[DatabaseSharp(Serializer = DatabaseEnumSerializer.SerializerName)]
			public TestEnum EnumValue { get; set; }
		}
		public class InputObject2
		{
			[DatabaseSharp(Serializer = DatabaseEnumSerializer.SerializerName, FillTable = 0)]
			public List<TestEnum> EnumValues { get; set; }
		}

		[TestMethod]
		public void Can_Serialize()
		{
			// ARRANGE
			var serializers = new Dictionary<string, IDatabaseSerializer>()
			{
				{ DatabaseEnumSerializer.SerializerName, new DatabaseEnumSerializer() }
			};
			var input = new InputObject()
			{
				EnumValue = TestEnum.AAAA
			};

			// ACT
			var param = ParameterHelpers.GenerateParametersFromObject(input, serializers);

			// ASSERT
			Assert.IsNotNull(param);
			Assert.AreEqual(1, param.Count);
			Assert.AreEqual((param[0] as SQLParam)!.Value, (int)TestEnum.AAAA);
		}

		[TestMethod]
		public void Can_Serialize_List()
		{
			// ARRANGE
			var serializers = new Dictionary<string, IDatabaseSerializer>()
			{
				{ DatabaseEnumSerializer.SerializerName, new DatabaseEnumSerializer() }
			};
			var input = new InputObject2()
			{
				EnumValues = new List<TestEnum>() { TestEnum.AAAA, TestEnum.abc }
			};

			// ACT
			var param = ParameterHelpers.GenerateParametersFromObject(input, serializers);

			// ASSERT
			Assert.IsNotNull(param);
			Assert.AreEqual(1, param.Count);
			Assert.IsTrue((param[0] as SQLListParam)!.Values.Contains((int)TestEnum.AAAA));
			Assert.IsTrue((param[0] as SQLListParam)!.Values.Contains((int)TestEnum.abc));
		}

		[TestMethod]
		[DataRow(TestEnum.None)]
		[DataRow(TestEnum.Value1)]
		[DataRow(TestEnum.abc)]
		[DataRow(TestEnum.AAAA)]
		public void Can_Deserialize(TestEnum enumVal)
		{
			// ARRANGE
			var serializers = new Dictionary<string, IDatabaseSerializer>()
			{
				{ DatabaseEnumSerializer.SerializerName, new DatabaseEnumSerializer() }
			};
			var dataset = new DataSet();
			var table = new DataTable();
			table.Columns.Add(new DataColumn("EnumValue", typeof(int)));
			table.Rows.Add(table.NewRow());
			table.Rows[0].SetField(table.Columns[0], enumVal);
			dataset.Tables.Add(table);
			var result = new DatabaseResult(dataset, serializers);

			// ACT
			var row = result[0][0];
			var model = row.Fill<InputObject>();

			// ASSERT
			Assert.AreEqual(enumVal, model.EnumValue);
		}

		[TestMethod]
		[DataRow(TestEnum.None, TestEnum.Value1)]
		public void Can_Deserialize_List(params TestEnum[] values)
		{
			// ARRANGE
			var serializers = new Dictionary<string, IDatabaseSerializer>()
			{
				{ DatabaseEnumSerializer.SerializerName, new DatabaseEnumSerializer() }
			};
			var dataset = new DataSet();
			var table = new DataTable();
			table.Columns.Add(new DataColumn("EnumValues", typeof(int)));
			for (int i = 0; i < values.Length; i++) {
				table.Rows.Add(table.NewRow());
				table.Rows[i].SetField(table.Columns[0], values[i]);
			}
			dataset.Tables.Add(table);
			var result = new DatabaseResult(dataset, serializers);

			// ACT
			var row = result[0][0];
			var model = row.Fill<InputObject2>();

			// ASSERT
			Assert.IsTrue(model.EnumValues.Count == values.Length);
		}
	}
}
