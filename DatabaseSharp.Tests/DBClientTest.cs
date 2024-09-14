using DatabaseSharp.Tests.TestModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseSharp.Tests
{
    [TestClass]
    public class DBClientTest
    {
        [TestMethod]
        public void Can_GenerateSTPParameters()
        {
            // ARRANGE
            var item = new TestClass();
            var client = new DBClient("");

            // ACT
            var parameters = client.GenerateParametersFromObject(item);

            // ASSERT
            Assert.IsNotNull(parameters);
            Assert.AreEqual(2, parameters.Count);
            Assert.IsTrue(parameters.Any(x => x.Name == "SomeParam"));
            Assert.IsTrue(parameters.Any(x => x.Name == "SomeOtherParam"));
		}
	}
}
