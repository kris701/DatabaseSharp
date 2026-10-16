using DatabaseSharp.Models;
using DatabaseSharp.SQLServer.Tests.Models;

namespace DatabaseSharp.SQLServer.Tests
{
	[TestClass]
	public class SQLServerClientTests
	{
		public static string ConnectionString = "Server=tcp:localhost,36790;Persist Security Info=False;User ID=sa;Password=yourStrong(!)Password;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=True;Connection Timeout=30;";

		[AssemblyInitialize]
		public static async Task BeforeClass(TestContext tc)
		{
			var client = new SQLServerClient(ConnectionString);
			var initializerText = File.ReadAllText("databaseinit.sql");
			var split = initializerText.Split("GO");
			foreach (var query in split)
				if (query.Trim() != "")
					await client.ExecuteFreeAsync(query.Trim());
		}

		#region Single Value
		[TestMethod]
		public async Task Can_GetSingleValue_NoArguments()
		{
			// ARRANGE
			var client = new SQLServerClient(ConnectionString);

			// ACT
			var result = await client.ExecuteAsync("dbo.NoArguments_SingleValue");

			// ASSERT
			Assert.AreEqual(1, result.Count);
			var table = result[0];
			Assert.AreEqual(1, table.Count);
			var value = table[0].GetValue<int>("SingleValue");
			Assert.AreEqual(1, value);
		}

		[TestMethod]
		[DataRow(10)]
		[DataRow(0)]
		[DataRow(19412)]
		public async Task Can_GetSingleValue_Arguments_Direct(int argument)
		{
			// ARRANGE
			var client = new SQLServerClient(ConnectionString);

			// ACT
			var result = await client.ExecuteAsync(
				"dbo.Arguments_SingleValue",
				new List<ISQLParameter>()
				{
					new SQLParam("ARGVALUE", argument)
				});

			// ASSERT
			Assert.AreEqual(1, result.Count);
			var table = result[0];
			Assert.AreEqual(1, table.Count);
			var value = table[0].GetValue<int>("SingleValue");
			Assert.AreEqual(argument, value);
		}

		[TestMethod]
		[DataRow(10)]
		[DataRow(0)]
		[DataRow(19412)]
		public async Task Can_GetSingleValue_Arguments_Model(int argument)
		{
			// ARRANGE
			var client = new SQLServerClient(ConnectionString);

			// ACT
			var result = await client.ExecuteAsync(
				"dbo.Arguments_SingleValue",
				new SimpleArg(argument));

			// ASSERT
			Assert.AreEqual(1, result.Count);
			var table = result[0];
			Assert.AreEqual(1, table.Count);
			var value = table[0].Fill<SingleValueResult>();
			Assert.AreEqual(argument, value.SingleValue);
		}

		#endregion

		#region Multiple Values

		[TestMethod]
		[DataRow(10)]
		[DataRow(0)]
		[DataRow(19412)]
		public async Task Can_GetMultipleValues_Arguments_Direct(int argument)
		{
			// ARRANGE
			var client = new SQLServerClient(ConnectionString);

			// ACT
			var result = await client.ExecuteAsync(
				"dbo.Arguments_MultiValues",
				new List<ISQLParameter>()
				{
					new SQLParam("ARGVALUE", argument)
				});

			// ASSERT
			Assert.AreEqual(1, result.Count);
			var table = result[0];
			Assert.AreEqual(3, table.Count);
			var values = table.GetAllValues<int>("SingleValue");
			Assert.AreEqual(argument, values[0]);
			Assert.AreEqual(argument + 1, values[1]);
			Assert.AreEqual(argument + 2, values[2]);
		}

		[TestMethod]
		[DataRow(10)]
		[DataRow(0)]
		[DataRow(19412)]
		public async Task Can_GetMultipleValues_Arguments_Model(int argument)
		{
			// ARRANGE
			var client = new SQLServerClient(ConnectionString);

			// ACT
			var result = await client.ExecuteAsync(
				"dbo.Arguments_MultiValues",
				new SimpleArg(argument));

			// ASSERT
			Assert.AreEqual(1, result.Count);
			var table = result[0];
			Assert.AreEqual(3, table.Count);
			var values = table.FillAll<SingleValueResult>();
			Assert.AreEqual(argument, values[0].SingleValue);
			Assert.AreEqual(argument + 1, values[1].SingleValue);
			Assert.AreEqual(argument + 2, values[2].SingleValue);
		}

		[TestMethod]
		public async Task Can_GetMultipleValues_NoArguments()
		{
			// ARRANGE
			var client = new SQLServerClient(ConnectionString);

			// ACT
			var result = await client.ExecuteAsync("dbo.NoArguments_MultiValues");

			// ASSERT
			Assert.AreEqual(1, result.Count);
			var table = result[0];
			Assert.AreEqual(3, table.Count);
			var values = table.GetAllValues<int>("SingleValue");
			Assert.HasCount(3, values);
		}

		#endregion
	}
}
