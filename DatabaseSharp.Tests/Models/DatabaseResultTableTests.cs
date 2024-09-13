using DatabaseSharp.Models;
using DatabaseSharp.Tests.TestModels;
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

		[TestMethod]
		public void Can_GetAllValues()
		{
			// ARRANGE
			var dataset = new DataSet();
			var table = new DataTable();
			table.Columns.Add(new DataColumn("col1", typeof(string)));
			table.Columns.Add(new DataColumn("col2", typeof(int)));
			table.Rows.Add(table.NewRow());
			table.Rows[0].SetField<string>(table.Columns[0], "asd");
			table.Rows[0].SetField<int>(table.Columns[1], 1);
			table.Rows.Add(table.NewRow());
			table.Rows[1].SetField<string>(table.Columns[0], "asdas");
			table.Rows[1].SetField<int>(table.Columns[1], 11231);
			dataset.Tables.Add(table);
			var result = new DatabaseResult(dataset);

			// ACT
			var col1 = result[0].GetAllValues<string>("col1");
			var col2 = result[0].GetAllValues<int>("col2");

			// ASSERT
			Assert.AreEqual(2, col1.Count);
			Assert.AreEqual(2, col2.Count);
		}

		[TestMethod]
		public void Can_GetAllValuesOrNull()
		{
			// ARRANGE
			var dataset = new DataSet();
			var table = new DataTable();
			table.Columns.Add(new DataColumn("col1", typeof(string)));
			table.Columns.Add(new DataColumn("col2", typeof(int)));
			table.Rows.Add(table.NewRow());
			table.Rows[0].SetField<string>(table.Columns[0], "asdas");
			table.Rows[0].SetField<int?>(table.Columns[1], null);
			table.Rows.Add(table.NewRow());
			table.Rows[1].SetField<string>(table.Columns[0], "asdas");
			table.Rows[1].SetField<int?>(table.Columns[1], 11231);
			dataset.Tables.Add(table);
			var result = new DatabaseResult(dataset);

			// ACT
			var col1 = result[0].GetAllValues<string>("col1");
			var col2 = result[0].GetAllValuesOrNull<int>("col2");

			// ASSERT
			Assert.AreEqual(2, col1.Count);
			Assert.AreEqual(2, col2.Count);
		}

		[TestMethod]
		public void Can_FillAll()
		{
			// ARRANGE
			var tstID = Guid.NewGuid();
			var dataset = new DataSet();
			var table = new DataTable();
			table.Columns.Add(new DataColumn("Name", typeof(string)));
			table.Columns.Add(new DataColumn("col2", typeof(int)));
			table.Columns.Add(new DataColumn("col3", typeof(Guid)));
			table.Rows.Add(table.NewRow());
			table.Rows[0].SetField(table.Columns[0], "abc");
			table.Rows[0].SetField(table.Columns[1], 123);
			table.Rows[0].SetField(table.Columns[2], tstID);
			table.Rows.Add(table.NewRow());
			table.Rows[1].SetField(table.Columns[0], "abcsa");
			table.Rows[1].SetField(table.Columns[1], 1241);
			table.Rows[1].SetField(table.Columns[2], Guid.Empty);
			dataset.Tables.Add(table);
			var result = new DatabaseResult(dataset);

			// ACT
			var filled = result[0].FillAll<TestClass>();

			// ASSERT
			Assert.AreEqual(2, filled.Count);
			Assert.AreEqual(filled[0].Name, "abc");
			Assert.AreEqual(filled[1].Name, "abcsa");
			Assert.AreEqual(filled[0].SomeValue, 123);
			Assert.AreEqual(filled[1].SomeValue, 1241);
		}
	}
}
