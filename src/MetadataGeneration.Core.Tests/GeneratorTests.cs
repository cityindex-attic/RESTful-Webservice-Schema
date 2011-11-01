using System.IO;
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
            _validJsonSchema = File.ReadAllText(@"TestData\valid\CIAPI.Schema.json");
            _validSMD = File.ReadAllText(@"TestData\valid\CIAPI.SMD.json");
        }

        private XmlDocSource SetupValidXmlDocSource()
        {
            _dtoAssemblyBasePath = @"TestData\valid\";
            return _wcfConfigReader.Read(@"TestData\valid\Web.Config", _dtoAssemblyBasePath);
        }

        [Test]
        public void ValidXmlShouldGenerateValidJsonSchema()
        {
            var xmlDocSource = SetupValidXmlDocSource();

            var results = _generator.GenerateJsonSchema(xmlDocSource);
            
            Assert.IsFalse(results.HasErrors,string.Format("Json Schema generation should not have failed\n\n{0}", results.ToString()));

            File.WriteAllText("Generated.Schema.json", results.JsonSchema.ToString());
            Assert.AreEqual(results.JsonSchema.ToString(), _validJsonSchema);
        }

        [Test]
        public void ValidXmlShouldGenerateValidSMD()
        {
            var xmlDocSource = SetupValidXmlDocSource();
            var jsonSchemaResults = _generator.GenerateJsonSchema(xmlDocSource);
            var smdResults = _generator.GenerateSmd(xmlDocSource, jsonSchemaResults.JsonSchema);
                
            Assert.That(jsonSchemaResults.HasErrors,Is.False,"SMD generation should not have failed");
            Assert.That(smdResults.HasErrors,Is.False,"SMD generation should not have failed");

            File.WriteAllText("Generated.SMD.json", smdResults.SMD.ToString());
            Assert.AreEqual(smdResults.SMD.ToString(), _validSMD);
        }
    }
}
