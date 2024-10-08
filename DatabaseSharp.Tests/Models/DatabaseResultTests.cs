﻿using DatabaseSharp.Models;
using System.Data;

namespace DatabaseSharp.Tests.Models
{
	[TestClass]
	public class DatabaseResultTests
	{
		[TestMethod]
		public void Can_IsEmpty()
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
			var empty = result.IsEmpty;

			// ASSERT
			Assert.IsFalse(empty);
		}

		[TestMethod]
		public void Can_Count()
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
			var count = result.Count;

			// ASSERT
			Assert.AreEqual(1, count);
		}

		[TestMethod]
		public void Can_IterateTables()
		{
			// ARRANGE
			var dataset = new DataSet();
			var table1 = new DataTable();
			table1.Columns.Add(new DataColumn("col1"));
			table1.Columns.Add(new DataColumn("col2"));
			table1.Rows.Add(table1.NewRow());
			table1.Rows.Add(table1.NewRow());
			dataset.Tables.Add(table1);
			var table2 = new DataTable();
			table2.Columns.Add(new DataColumn("col3"));
			table2.Columns.Add(new DataColumn("col4"));
			table2.Rows.Add(table2.NewRow());
			table2.Rows.Add(table2.NewRow());
			dataset.Tables.Add(table2);
			var result = new DatabaseResult(dataset);

			// ACT

			// ASSERT
			Assert.AreEqual(2, result.Count);
			Assert.IsTrue(result[0].ContainsColumn("col1"));
			Assert.IsTrue(result[0].ContainsColumn("col2"));
			Assert.IsTrue(result[1].ContainsColumn("col3"));
			Assert.IsTrue(result[1].ContainsColumn("col4"));

			var iterated = 0;
			foreach (var table in result)
			{
				iterated++;
				if (table.ContainsColumn("col1"))
				{
					Assert.IsTrue(table.ContainsColumn("col1"));
					Assert.IsTrue(table.ContainsColumn("col2"));
				}
				else
				{
					Assert.IsTrue(table.ContainsColumn("col3"));
					Assert.IsTrue(table.ContainsColumn("col4"));
				}
			}
			Assert.AreEqual(2, iterated);
		}
	}
}
