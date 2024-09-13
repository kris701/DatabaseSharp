using DatabaseSharp.Models;
using System.Data;

namespace DatabaseSharp.Tests.Models
{
	[TestClass]
	public class DatabaseResultRowTests
	{
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
			var row = result[0][0];
			Assert.AreEqual("abc", row.GetValue<string>("col1"));
			Assert.AreEqual(123, row.GetValue<int>("col2"));
			Assert.AreEqual(DateTime.MinValue, row.GetValue<DateTime>("col3"));
			Assert.AreEqual(4.2, row.GetValue<double>("col4"));
			Assert.AreEqual(false, row.GetValue<bool>("col5"));
			Assert.AreEqual(true, row.GetValue<bool>("col6"));
			Assert.AreEqual(false, row.GetValue<bool>("col7"));
			Assert.AreEqual(true, row.GetValue<bool>("col8"));
			Assert.AreEqual(DateTime.Parse("2024-09-13 05:48:41.237"), row.GetValue<DateTime>("col9"));
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
			var row = result[0][0];
			Assert.AreEqual(123, row.GetValueOrNull<int>("col2"));
			Assert.IsNull(row.GetValueOrNull<DateTime>("col3"));
			Assert.AreEqual(4.2, row.GetValueOrNull<double>("col4"));
		}
	}
}
