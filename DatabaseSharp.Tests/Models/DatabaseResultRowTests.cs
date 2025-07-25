using DatabaseSharp.Models;
using DatabaseSharp.Serializers;
using DatabaseSharp.Tests.TestModels;
using System.Data;
using System.Text.Json;

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
			table.Columns.Add(new DataColumn("col10", typeof(DateTime)));
			table.Rows.Add(table.NewRow());
			table.Rows[0].SetField(table.Columns[0], "abc");
			table.Rows[0].SetField(table.Columns[1], 123);
			table.Rows[0].SetField(table.Columns[2], DateTime.MinValue);
			table.Rows[0].SetField(table.Columns[3], 4.2);
			table.Rows[0].SetField(table.Columns[4], 0);
			table.Rows[0].SetField(table.Columns[5], 1);
			table.Rows[0].SetField(table.Columns[6], false);
			table.Rows[0].SetField(table.Columns[7], true);
			table.Rows[0].SetField(table.Columns[8], DateTime.Parse("2024-09-13 05:48:41.237Z"));
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
			Assert.AreEqual(DateTime.Parse("2024-09-13 05:48:41.237Z"), row.GetValue<DateTime>("col9"));
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
			table.Columns.Add(new DataColumn("col5", typeof(TestEnum)));
			table.Rows.Add(table.NewRow());
			table.Rows[0].SetField(table.Columns[0], "abc");
			table.Rows[0].SetField(table.Columns[1], 123);
			table.Rows[0].SetField<DateTime?>(table.Columns[2], null);
			table.Rows[0].SetField(table.Columns[3], 4.2);
			table.Rows[0].SetField<TestEnum?>(table.Columns[4], null);
			table.Rows.Add(table.NewRow());
			dataset.Tables.Add(table);
			var result = new DatabaseResult(dataset);

			// ACT

			// ASSERT
			var row = result[0][0];
			Assert.AreEqual(123, row.GetValueOrNull<int>("col2"));
			Assert.IsNull(row.GetValueOrNull<DateTime>("col3"));
			Assert.AreEqual(4.2, row.GetValueOrNull<double>("col4"));
			Assert.IsNull(row.GetValueOrNull<TestEnum>("col5"));
		}

		[TestMethod]
		public void Can_Fill()
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
			dataset.Tables.Add(table);
			var result = new DatabaseResult(dataset);

			// ACT

			// ASSERT
			var row = result[0][0];
			var filled = row.Fill<TestClass>();
			Assert.IsNotNull(filled);
			Assert.AreEqual("abc", filled.Name);
			Assert.AreEqual(123, filled.SomeValue);
			Assert.AreEqual(tstID, filled.ID);
		}

		[TestMethod]
		public void Can_Fill2()
		{
			// ARRANGE
			var dataset = new DataSet();
			var table = new DataTable();
			table.Columns.Add(new DataColumn("Nullable1", typeof(int)));
			table.Columns.Add(new DataColumn("Nullable2", typeof(TestEnum)));
			table.Rows.Add(table.NewRow());
			table.Rows[0].SetField<int?>(table.Columns[0], 10);
			table.Rows[0].SetField<TestEnum?>(table.Columns[1], null);
			table.Rows.Add(table.NewRow());
			dataset.Tables.Add(table);
			var result = new DatabaseResult(dataset);
			result.Serializers.Add(DatabaseEnumSerializer.SerializerName, new DatabaseEnumSerializer());

			// ACT

			// ASSERT
			var row = result[0][0];
			var filled = row.Fill<TestClass6>();
			Assert.IsNotNull(filled);
			Assert.AreEqual(10, filled.Nullable1);
			Assert.IsNull(filled.Nullable2);
		}

		[TestMethod]
		public void Can_Fill3()
		{
			// ARRANGE
			var dataset = new DataSet();
			var table = new DataTable();
			table.Columns.Add(new DataColumn("Nullable1", typeof(int)));
			table.Columns.Add(new DataColumn("Nullable2", typeof(string)));
			table.Rows.Add(table.NewRow());
			table.Rows[0].SetField<int?>(table.Columns[0], 10);
			table.Rows[0].SetField<string?>(table.Columns[1], null);
			table.Rows.Add(table.NewRow());
			dataset.Tables.Add(table);
			var result = new DatabaseResult(dataset);
			result.Serializers.Add(DatabaseJsonSerializer.SerializerName, new DatabaseJsonSerializer());

			// ACT

			// ASSERT
			var row = result[0][0];
			var filled = row.Fill<TestClass7>();
			Assert.IsNotNull(filled);
			Assert.AreEqual(10, filled.Nullable1);
			Assert.IsNull(filled.Nullable2);
		}

		[TestMethod]
		public void Can_Fill4()
		{
			// ARRANGE
			var dataset = new DataSet();
			var table1 = new DataTable();
			table1.Columns.Add(new DataColumn("Name", typeof(string)));
			table1.Columns.Add(new DataColumn("Value", typeof(string)));
			table1.Rows.Add(table1.NewRow());
			table1.Rows[0].SetField<string>(table1.Columns[0], "test name");
			table1.Rows[0].SetField<string>(table1.Columns[1], "test");
			table1.Rows.Add(table1.NewRow());
			table1.Rows[1].SetField<string>(table1.Columns[0], "test name 2");
			table1.Rows[1].SetField<string>(table1.Columns[1], "test 2");
			dataset.Tables.Add(table1);
			var table2 = new DataTable();
			table2.Columns.Add(new DataColumn("val1", typeof(int)));
			table2.Columns.Add(new DataColumn("val2", typeof(string)));
			table2.Rows.Add(table2.NewRow());
			table2.Rows[0].SetField<int>(table2.Columns[0], 10);
			table2.Rows[0].SetField<string>(table2.Columns[1], "test");
			table2.Rows.Add(table2.NewRow());
			table2.Rows[1].SetField<int>(table2.Columns[0], 10);
			table2.Rows[1].SetField<string>(table2.Columns[1], "test 2");
			dataset.Tables.Add(table2);
			var result = new DatabaseResult(dataset);

			// ACT

			// ASSERT
			var row = result[0][0];
			var filled = row.Fill<TestClass8>(result);
			Assert.IsNotNull(filled);
			Assert.AreEqual("test name", filled.Name);
			Assert.AreEqual("test", filled.Value);
			Assert.AreEqual(2, filled.Items.Count);
		}

		[TestMethod]
		public void Can_Fill5()
		{
			// ARRANGE
			var dataset = new DataSet();
			var table1 = new DataTable();
			table1.Columns.Add(new DataColumn("Name", typeof(string)));
			table1.Columns.Add(new DataColumn("Value", typeof(bool)));
			table1.Rows.Add(table1.NewRow());
			table1.Rows[0].SetField<string>(table1.Columns[0], "test name");
			table1.Rows[0].SetField<bool>(table1.Columns[1], true);
			table1.Rows.Add(table1.NewRow());
			table1.Rows[1].SetField<string>(table1.Columns[0], "test name 2");
			table1.Rows[1].SetField<bool>(table1.Columns[1], false);
			dataset.Tables.Add(table1);
			var table2 = new DataTable();
			table2.Columns.Add(new DataColumn("ID", typeof(Guid)));
			table2.Columns.Add(new DataColumn("Name", typeof(string)));
			table2.Columns.Add(new DataColumn("LongName", typeof(string)));
			table2.Columns.Add(new DataColumn("Time", typeof(TimeSpan)));
			table2.Rows.Add(table2.NewRow());
			table2.Rows[0].SetField<Guid>(table2.Columns[0], Guid.NewGuid());
			table2.Rows[0].SetField<string>(table2.Columns[1], "name");
			table2.Rows[0].SetField<string>(table2.Columns[2], "longer name");
			table2.Rows[0].SetField<TimeSpan>(table2.Columns[3], TimeSpan.FromSeconds(10));
			dataset.Tables.Add(table2);
			var table3 = new DataTable();
			table3.Columns.Add(new DataColumn("Value3", typeof(string)));
			table3.Rows.Add(table3.NewRow());
			table3.Rows[0].SetField<string>(table3.Columns[0], "test val");
			dataset.Tables.Add(table3);
			var result = new DatabaseResult(dataset);

			// ACT

			// ASSERT
			var row = result[0][0];
			var filled = row.Fill<TestClass9>(result);
			Assert.IsNotNull(filled);
			Assert.AreEqual("test name", filled.Name);
			Assert.AreEqual(true, filled.Value);
			Assert.AreEqual(1, filled.Items.Count);
			Assert.AreEqual("name", filled.Items[0].Name);
			Assert.AreEqual("longer name", filled.Items[0].LongName);
			Assert.AreEqual(TimeSpan.FromSeconds(10), filled.Items[0].Time);
			Assert.AreEqual("test val", filled.Value3);
		}
	}
}
