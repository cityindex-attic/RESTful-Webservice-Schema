using System;
using CIAPI.MetadataGenerator.Tests;
using MetadataGeneration.Core.JsonSchemaDTO;
using NUnit.Framework;

namespace MetadataGeneration.Core.Tests
{
    [TestFixture]
    public class AuditorTests : GeneratorTestsBase
    {
        [Test]
        public void ValidSourceShouldHaveCollectionOfSuccessesAndNoErrors()
        {
            _dtoAssemblyBasePath = @"TestData\valid\RESTWebServices.20111111\";
            var xmlDocSource = _wcfConfigReader.Read(@"TestData\valid\RESTWebServices.20111111\Web.Config", _dtoAssemblyBasePath);

            var result = new Auditor().AuditTypes(xmlDocSource);
            Assert.Greater(result.MetadataGenerationSuccesses.Count, 0, "success count should be > 0");
            Assert.AreEqual(result.MetadataGenerationErrors.Count, 0, "error count should be 0");
        }
    }
}
