using DatabaseSharp.Models;
using DatabaseSharp.PostgreSQL.Tests.Models;

namespace DatabaseSharp.PostgreSQL.Tests
{
	[TestClass]
	public class PostgreSQLClientTests
	{
		public static string ConnectionString = "Server=localhost;Port=54211;User Id=postgres;Password=superstrongpassword;Database=postgres;";

		[AssemblyInitialize]
		public static async Task BeforeClass(TestContext tc)
		{
			var client = new PostgreSQLClient(ConnectionString);
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
			var client = new PostgreSQLClient(ConnectionString);

			// ACT
			var result = await client.ExecuteAsync("NoArguments_SingleValue");

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
			var client = new PostgreSQLClient(ConnectionString);

			// ACT
			var result = await client.ExecuteAsync(
				"public.Arguments_SingleValue",
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
			var client = new PostgreSQLClient(ConnectionString);

			// ACT
			var result = await client.ExecuteAsync(
				"public.Arguments_SingleValue",
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

		// Not really natively possible with PostgreSQL at this point...

		#endregion
	}
}
