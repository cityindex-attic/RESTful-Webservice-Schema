using System;
using NUnit.Framework;
using System.Linq;

namespace CIAPI.MetadataGenerator.Tests
{
    [TestFixture]
    public class GeneratorExpectedFailuresTests : GeneratorTestsBase
    {
        //FIXME
        [Test, Ignore("Running this test in the same test run as other causes the wrong assemblies to be loaded by subsiquent tests")]
        public void InvalidXmlShouldPinpointError()
        {
            _dtoAssemblyBasePath = @"TestData\invalid\RESTWebservices.0.975\";
            var xmlDocSource = _wcfConfigReader.Read(@"TestData\invalid\RESTWebservices.0.975\Web.Config", _dtoAssemblyBasePath);

            var jsonSchemaResults = _generator.GenerateJsonSchema(xmlDocSource);
            var smdResults = _generator.GenerateSmd(xmlDocSource, jsonSchemaResults.JsonSchema);

            Assert.That(smdResults.HasErrors, Is.True, "SMD generation should have failed");

            var allErrors = string.Join("\n", smdResults.MetadataGenerationErrors.Select(e => e.ToString()));
            Console.WriteLine(allErrors);
            StringAssert.Contains("IAccountInformationService", allErrors);
            StringAssert.Contains("schema type not found for IAccountInformationService.SaveAccountInformation ", allErrors);
        }
    }
}
