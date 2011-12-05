using System;
using NUnit.Framework;
using System.Linq;

namespace CIAPI.MetadataGenerator.Tests
{
    [TestFixture]
    public class GeneratorExpectedFailuresTests : GeneratorTestsBase
    {
        [Test, Ignore]
        public void InvalidXmlShouldPinpointError()
        {
            _dtoAssemblyBasePath = @"TestData\invalid\RESTWebservices.0.869\";
            var xmlDocSource = _wcfConfigReader.Read(@"TestData\invalid\RESTWebservices.0.869\Web.Config", _dtoAssemblyBasePath);

            var jsonSchemaResults = _generator.GenerateJsonSchema(xmlDocSource);
            var smdResults = _generator.GenerateSmd(xmlDocSource, jsonSchemaResults.JsonSchema);

            Assert.That(smdResults.HasErrors, Is.True, "SMD generation should have failed");

            var allErrors = string.Join("\n", smdResults.MetadataGenerationErrors.Select(e => e.ToString()));
            Console.WriteLine(allErrors);
            StringAssert.Contains("ILoginService", allErrors);
            StringAssert.Contains("param element not found for ILoginService.LogOn", allErrors);
        }
    }
}
