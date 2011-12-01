using System;
using System.IO;
using MetadataGeneration.Core.JsonSchemaDTO;
using MetadataGeneration.Core.Streaming;
using Newtonsoft.Json.Linq;
using NUnit.Framework;


namespace MetadataGeneration.Core.Tests
{
    [TestFixture]
    public class GeneratorTests : GeneratorTestsBase
    {
        private string _patchJson;
        private string _smdPatchPath;
        private string _streamingJson;
        private string _validSMD;
        private string _validJsonSchema;

        public GeneratorTests()
        {
            //_validJsonSchema = File.ReadAllText(@"TestData\valid\RESTWebServices.20111129\Metadata\schema.json");
            //_validSMD = File.ReadAllText(@"TestData\valid\RESTWebServices.20111129\Metadata\smd.json");
        }

        private XmlDocSource SetupValidXmlDocSource()
        {
            _dtoAssemblyBasePath = @".\TestData\valid\RESTWebServices.20111201\";
            return _wcfConfigReader.Read(_dtoAssemblyBasePath + @"\Web.Config", _dtoAssemblyBasePath);
        }
        [Test]
        public void StreamingReturnTypesShouldBePresent()
        {
            var ssmdText = File.ReadAllText(@"TestData\valid\RESTWebServices.20111201\Metadata\streaming.fragment.json");
            JObject smd = JObject.Parse(ssmdText);
            var xmlDocSource = SetupValidXmlDocSource();
            
            MetadataGenerationResult results = _generator.GenerateJsonSchema(xmlDocSource);
  

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
            var xmlDocSource = SetupValidXmlDocSource();

            MetadataGenerationResult results = _generator.GenerateJsonSchema(xmlDocSource);

            string resultsToString = results.ToString();
            Assert.IsFalse(results.HasErrors, string.Format("Json Schema generation should not have failed\n\n{0}", resultsToString));

            //TODO: Add this back when we have valid metadata
//            File.WriteAllText("Generated.Schema.json", results.JsonSchema.ToString());
//            Assert.AreEqual(_validJsonSchema, results.JsonSchema.ToString());
        }

        [Test]
        public void ValidXmlShouldGenerateValidSMD()
        {
            var xmlDocSource = SetupValidXmlDocSource();
            var jsonSchemaResults = _generator.GenerateJsonSchema(xmlDocSource);
            var smdResults = _generator.GenerateSmd(xmlDocSource, jsonSchemaResults.JsonSchema);

            jsonSchemaResults.MetadataGenerationErrors.ForEach(e => Console.WriteLine(e.ToString()));
            Assert.That(jsonSchemaResults.HasErrors,Is.False,"SMD generation should not have failed");
            smdResults.MetadataGenerationErrors.ForEach(e => Console.WriteLine(e.ToString()));
            Assert.That(smdResults.HasErrors,Is.False,"SMD generation should not have failed");

            //TODO: Add this back when we have valid metadata
//            File.WriteAllText("Generated.SMD.json", smdResults.SMD.ToString());
//            Assert.AreEqual(_validSMD, smdResults.SMD.ToString());
        }
    }
}
