using System;
using System.IO;
using System.Reflection;
using NUnit.Framework;

namespace JsonSchemaGeneration.Tests
{
    [TestFixture]
    public class GeneratorExpectedFailuresTests : GeneratorTestsBase
    {
//        [Test]
//        public void ValidXmlShouldGenerateValidJsonSchema()
//        {
//            var jsonSchema = "";
//            var xmlDocSource = SetupValidXmlDocSource();
//
//            try
//            {
//                jsonSchema = _generator.GenerateJsonSchema(xmlDocSource);
//            }
//            catch (Exception e)
//            {
//                Assert.Fail(string.Format("Json Schema generation should not have failed\n\nMessage:\n{0}\n\nStackTrace:\n{1}", e.Message, e.StackTrace));
//            }
//
//            File.WriteAllText("Generated.Schema.json", jsonSchema);
//            Assert.AreEqual(jsonSchema, _validJsonSchema);
//        }

        [Test]
        public void InvalidXmlShouldPinpointError()
        {
            _dtoAssemblyBasePath = @"TestData\invalid\RESTWebservices.0.869\";
            var xmlDocSource = _wcfConfigReader.Read(@"TestData\invalid\RESTWebservices.0.869\Web.Config");

            try
            {
                var results = _generator.GenerateJsonSchema(xmlDocSource);
                _generator.GenerateSmd(xmlDocSource, results.JsonSchema);
                Assert.Fail("Exception should have been thrown");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                StringAssert.Contains("ILoginService", e.ToString());
                StringAssert.Contains("param element not found for ILoginService.LogOn", e.ToString());
            }
        }
    }
}
