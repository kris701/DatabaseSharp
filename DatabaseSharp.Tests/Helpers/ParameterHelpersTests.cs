using DatabaseSharp.Helpers;
using DatabaseSharp.Serializers;
using DatabaseSharp.Tests.TestModels;

namespace DatabaseSharp.Tests.Helpers
{
	[TestClass]
	public class ParameterHelpersTests
	{
		[TestMethod]
		public void Can_GenerateSTPParameters()
		{
			// ARRANGE
			var item = new TestClass();

			// ACT
			var parameters = ParameterHelpers.GenerateParametersFromObject(item, new Dictionary<string, DatabaseSharp.Serializers.IDatabaseSerializer>());

			// ASSERT
			Assert.IsNotNull(parameters);
			Assert.AreEqual(3, parameters.Count);
			Assert.IsTrue(parameters.Any(x => x.Name == "SomeParam"));
			Assert.IsTrue(parameters.Any(x => x.Name == "SomeOtherParam"));
			Assert.IsTrue(parameters.Any(x => x.Name == "Guids"));
		}

		[TestMethod]
		public void Can_GenerateSTPParameters2()
		{
			// ARRANGE
			var item = new TestClass6()
			{
				Nullable1 = 50,
				Nullable2 = null
			};

			// ACT
			var parameters = ParameterHelpers.GenerateParametersFromObject(item, new Dictionary<string, DatabaseSharp.Serializers.IDatabaseSerializer>() { { DatabaseEnumSerializer.SerializerName, new DatabaseEnumSerializer() } });

			// ASSERT
			Assert.IsNotNull(parameters);
			Assert.IsTrue(parameters.Any(x => x.Name == "Nullable1"));
			Assert.IsTrue(parameters.Any(x => x.Name == "Nullable2"));
		}
	}
}
