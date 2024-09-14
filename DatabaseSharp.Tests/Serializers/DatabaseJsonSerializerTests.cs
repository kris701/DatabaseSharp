using DatabaseSharp.Models;
using DatabaseSharp.Tests.TestModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace DatabaseSharp.Tests.Serializers
{
	[TestClass]
	public class DatabaseJsonSerializerTests
	{
		[TestMethod]
		public void Can_Serialize()
		{
			// ARRANGE
			var client = new DBClient("");
			var expected = JsonSerializer.Serialize(new TestClass());
			var dataset = new DataSet();
			var table = new DataTable();
			table.Columns.Add(new DataColumn("col1", typeof(string)));
			table.Rows.Add(table.NewRow());
			table.Rows[0].SetField(table.Columns[0], expected);
			dataset.Tables.Add(table);
			var result = new DatabaseResult(dataset, client.Serializers);

			// ACT

			// ASSERT
			var row = result[0][0];
			var model = row.Fill<TestClass2>();

			Assert.AreEqual(expected, JsonSerializer.Serialize(model.Test));
		}
	}
}
