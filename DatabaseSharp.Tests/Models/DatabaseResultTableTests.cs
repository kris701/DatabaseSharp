using DatabaseSharp.Models;
using System.Data;

namespace DatabaseSharp.Tests.Models
{
	[TestClass]
	public class DatabaseResultTableTests
	{
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
			var count = result[0].Count;

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
			Assert.IsTrue(result[0].ContainsColumn("col1"));
			Assert.IsTrue(result[0].ContainsColumn("col2"));
			Assert.IsFalse(result[0].ContainsColumn("col3"));
			Assert.IsFalse(result[0].ContainsColumn("col4"));
		}

		[TestMethod]
		public void Can_IterateRows()
		{
			// ARRANGE
			var dataset = new DataSet();
			var table1 = new DataTable();
			table1.Columns.Add(new DataColumn("col1"));
			table1.Columns.Add(new DataColumn("col2"));
			table1.Columns.Add(new DataColumn("col3"));
			table1.Columns.Add(new DataColumn("col4"));
			table1.Rows.Add(table1.NewRow());
			table1.Rows.Add(table1.NewRow());
			table1.Rows.Add(table1.NewRow());
			table1.Rows.Add(table1.NewRow());
			table1.Rows.Add(table1.NewRow());
			table1.Rows.Add(table1.NewRow());
			dataset.Tables.Add(table1);
			var result = new DatabaseResult(dataset);
			var table = result[0];

			// ACT

			// ASSERT
			Assert.AreEqual(6, table.Count);
			var iterated = 0;
			foreach (var row in table)
			{
				iterated++;
			}
			Assert.AreEqual(6, iterated);
		}
	}
}
