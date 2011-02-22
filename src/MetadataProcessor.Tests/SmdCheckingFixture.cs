using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace MetadataProcessor.Tests
{
    [TestFixture]
    public class SmdCheckingFixture
    {
        [Test]
        public void EnsureMissingTypesThrow()
        {
            var smd = JsonSchemaFixture.GetTestTarget("test-smd");
            var schema = JsonSchemaFixture.GetTestTarget("test-smd-schema01");
            var checker = new TypeChecker();
            try
            {
                checker.VerifySmd(schema, smd, false);
                Assert.Fail("expected exception");
            }
            catch (Exception ex)
            {

                Assert.AreEqual("Method Service1 return type TestDTO is not represented in schema", ex.Message);

            }
        }

        [Test]
        public void EnsurePresentTypesDoNotThrow()
        {
            var smd = JsonSchemaFixture.GetTestTarget("test-smd");
            var schema = JsonSchemaFixture.GetTestTarget("test-smd-schema02");
            var checker = new TypeChecker();
            checker.VerifySmd(schema, smd, false);
        }
    }
}
