using System;
using System.IO;
using MetadataGeneration.Core;
using MetadataGeneration.Core.JsonSchemaDTO;
using MetadataGeneration.Core.Streaming;
using Newtonsoft.Json.Linq;
using NUnit.Framework;

namespace CIAPI.MetadataGenerator.Tests
{
    [TestFixture]
    public class GeneratorTests : GeneratorTestsBase
    {
        private string _streamingJson;
        private string _validSMD;
        private string _validJsonSchema;
        private XmlDocSource _xmlDocSource;

        [TestFixtureSetUp]
        public void SetupTestData()
        {
            _validJsonSchema = File.ReadAllText(@"TestData\valid\Metadata\schema.json.js");
            _validSMD = File.ReadAllText(@"TestData\valid\Metadata\smd.json.js");
            _streamingJson = File.ReadAllText(@"TestData\valid\Metadata\streaming.fragment.json");
            _dtoAssemblyBasePath = @".\TestData\valid\";
            _xmlDocSource = _wcfConfigReader.Read(_dtoAssemblyBasePath + @"\Web.Config", _dtoAssemblyBasePath);
        }

        [Test]
        public void StreamingReturnTypesShouldBePresent()
        {
            var smd = JObject.Parse(_streamingJson);
            var results = _generator.GenerateJsonSchema(_xmlDocSource);
   
            var validator = new StreamingValidator();
            try
            {
                validator.ValidateStreamingFragment(smd, results.JsonSchema);
            }
            catch (MetadataValidationException ex)
            {
                Assert.Fail(ex.ToString());
            }
        }
        [Test]
        public void ValidXmlShouldGenerateValidJsonSchema()
        {
            //Checks that dto all have valid XML comments
            XmlDocUtils.EnsureXmlDocsAreValid(_xmlDocSource);
            var results = _generator.GenerateJsonSchema(_xmlDocSource);

            var resultsToString = results.ToString();
            Assert.IsFalse(results.HasErrors, string.Format("Json Schema generation should not have failed\n\n{0}", resultsToString));

            string schema = results.JsonSchema.ToString();
            File.WriteAllText("Generated.Schema.json", schema);
            Assert.AreEqual(_validJsonSchema, schema);
        }

        [Test]
        public void ValidXmlShouldGenerateValidSMD()
        {
            
            var jsonSchemaResults = _generator.GenerateJsonSchema(_xmlDocSource);
            var smdResults = _generator.GenerateSmd(_xmlDocSource, jsonSchemaResults.JsonSchema);
            _generator.AddStreamingSMD(smdResults.SMD, _streamingJson);
            
            jsonSchemaResults.MetadataGenerationErrors.ForEach(e => Console.WriteLine(e.ToString()));
            Assert.That(jsonSchemaResults.HasErrors, Is.False, "SMD generation should not have failed");
            smdResults.MetadataGenerationErrors.ForEach(e => Console.WriteLine(e.ToString()));
            Assert.That(smdResults.HasErrors, Is.False, "SMD generation should not have failed");

            var smd = smdResults.SMD.ToString();

            File.WriteAllText("Generated.SMD.json", smd);
            Assert.AreEqual(_validSMD, smd);
        }
    }
}
