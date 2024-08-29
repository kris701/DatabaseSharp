﻿using DatabaseSharp.Models;
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
			table.Rows.Add(table.NewRow());
			table.Rows[0].SetField(table.Columns[0], "abc");
			table.Rows[0].SetField(table.Columns[1], 123);
			table.Rows[0].SetField(table.Columns[2], DateTime.MinValue);
			table.Rows[0].SetField(table.Columns[3], 4.2);
			table.Rows[0].SetField(table.Columns[4], 0);
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
	}
}
