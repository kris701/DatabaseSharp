using DatabaseSharp.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseSharp.Tests.Models
{
	[TestClass]
	public class DatabaseResultTests
	{
		[TestMethod]
		public void Can_SetDataset()
		{
			// ARRANGE
			var dataset = new DataSet();

			// ACT
			var result = new DatabaseResult(dataset);

			// ASSERT
			Assert.AreEqual(dataset, result.DataSet);
		}

		[TestMethod]
		public void Can_GetRowCount()
		{
			// ARRANGE
			var dataset = new DataSet();
			var table = new DataTable();
			table.Columns.Add(new DataColumn("col1"));
			table.Columns.Add(new DataColumn("col2"));
			table.Rows.Add(table.NewRow());
			table.Rows.Add(table.NewRow());
			dataset.Tables.Add(table);
			var result = new DatabaseResult(dataset);

			// ACT
			var count = result.RowCount();

			// ASSERT
			Assert.AreEqual(2, count);
		}

		[TestMethod]
		public void Can_ContainsColumn()
		{
			// ARRANGE
			var dataset = new DataSet();
			var table = new DataTable();
			table.Columns.Add(new DataColumn("col1"));
			table.Columns.Add(new DataColumn("col2"));
			table.Rows.Add(table.NewRow());
			table.Rows.Add(table.NewRow());
			dataset.Tables.Add(table);
			var result = new DatabaseResult(dataset);

			// ACT

			// ASSERT
			Assert.IsTrue(result.ContainsColumn("col1"));
			Assert.IsTrue(result.ContainsColumn("col2"));
			Assert.IsFalse(result.ContainsColumn("col3"));
			Assert.IsFalse(result.ContainsColumn("col4"));
		}

		[TestMethod]
		public void Can_GetValue()
		{
			// ARRANGE
			var dataset = new DataSet();
			var table = new DataTable();
			table.Columns.Add(new DataColumn("col1", typeof(string)));
			table.Columns.Add(new DataColumn("col2", typeof(int)));
			table.Columns.Add(new DataColumn("col3", typeof(DateTime)));
			table.Columns.Add(new DataColumn("col4", typeof(double)));
			table.Columns.Add(new DataColumn("col5", typeof(int)));
			table.Columns.Add(new DataColumn("col6", typeof(int)));
			table.Columns.Add(new DataColumn("col7", typeof(bool)));
			table.Columns.Add(new DataColumn("col8", typeof(bool)));
			table.Columns.Add(new DataColumn("col9", typeof(DateTime)));
			table.Rows.Add(table.NewRow());
			table.Rows[0].SetField(table.Columns[0], "abc");
			table.Rows[0].SetField(table.Columns[1], 123);
			table.Rows[0].SetField(table.Columns[2], DateTime.MinValue);
			table.Rows[0].SetField(table.Columns[3], 4.2);
			table.Rows[0].SetField(table.Columns[4], 0);
			table.Rows[0].SetField(table.Columns[5], 1);
			table.Rows[0].SetField(table.Columns[6], false);
			table.Rows[0].SetField(table.Columns[7], true);
			table.Rows[0].SetField(table.Columns[8], DateTime.Parse("2024-09-13 05:48:41.237"));
			table.Rows.Add(table.NewRow());
			dataset.Tables.Add(table);
			var result = new DatabaseResult(dataset);

			// ACT

			// ASSERT
			Assert.AreEqual("abc", result.GetValue<string>("col1"));
			Assert.AreEqual(123, result.GetValue<int>("col2"));
			Assert.AreEqual(DateTime.MinValue, result.GetValue<DateTime>("col3"));
			Assert.AreEqual(4.2, result.GetValue<double>("col4"));
			Assert.AreEqual(false, result.GetValue<bool>("col5"));
			Assert.AreEqual(true, result.GetValue<bool>("col6"));
			Assert.AreEqual(false, result.GetValue<bool>("col7"));
			Assert.AreEqual(true, result.GetValue<bool>("col8"));
			Assert.AreEqual(DateTime.Parse("2024-09-13 05:48:41.237"), result.GetValue<DateTime>("col9"));
		}

		[TestMethod]
		public void Can_GetValueOrNull()
		{
			// ARRANGE
			var dataset = new DataSet();
			var table = new DataTable();
			table.Columns.Add(new DataColumn("col1", typeof(string)));
			table.Columns.Add(new DataColumn("col2", typeof(int)));
			table.Columns.Add(new DataColumn("col3", typeof(DateTime)));
			table.Columns.Add(new DataColumn("col4", typeof(double)));
			table.Rows.Add(table.NewRow());
			table.Rows[0].SetField(table.Columns[0], "abc");
			table.Rows[0].SetField(table.Columns[1], 123);
			table.Rows[0].SetField<DateTime?>(table.Columns[2], null);
			table.Rows[0].SetField(table.Columns[3], 4.2);
			table.Rows.Add(table.NewRow());
			dataset.Tables.Add(table);
			var result = new DatabaseResult(dataset);

			// ACT

			// ASSERT
			Assert.AreEqual(123, result.GetValueOrNull<int>("col2"));
			Assert.IsNull(result.GetValueOrNull<DateTime>("col3"));
			Assert.AreEqual(4.2, result.GetValueOrNull<double>("col4"));
		}

		[TestMethod]
		public void Can_GetStructuredValue_List()
		{
			// ARRANGE
			var dataset = new DataSet();
			var table = new DataTable();
			table.Columns.Add(new DataColumn("col1", typeof(List<string>)));
			table.Rows.Add(table.NewRow());
			table.Rows[0].SetField(table.Columns[0], new List<string>() { "a", "b" });
			table.Rows.Add(table.NewRow());
			dataset.Tables.Add(table);
			var result = new DatabaseResult(dataset);

			// ACT

			// ASSERT
			var str = result.GetStructuredValue<List<string>>("col1");
			Assert.IsTrue(str.Contains("a"));
			Assert.IsTrue(str.Contains("b"));
		}

		[TestMethod]
		public void Can_GetStructuredValue_Type()
		{
			// ARRANGE
			var dataset = new DataSet();
			var table = new DataTable();
			table.Columns.Add(new DataColumn("col1", typeof(TestClass)));
			table.Rows.Add(table.NewRow());
			table.Rows[0].SetField(table.Columns[0], new TestClass() { Id = 1241, Name = "asb" });
			table.Rows.Add(table.NewRow());
			dataset.Tables.Add(table);
			var result = new DatabaseResult(dataset);

			// ACT

			// ASSERT
			var str = result.GetStructuredValue<TestClass>("col1");
			Assert.AreEqual(1241, str.Id);
			Assert.AreEqual("asb", str.Name);
		}

		private class TestClass
		{
			public int Id { get; set; }
			public string Name { get; set; }
		}
	}
}
