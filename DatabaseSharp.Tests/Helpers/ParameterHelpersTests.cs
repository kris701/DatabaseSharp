using DatabaseSharp.Helpers;
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
	}
}
