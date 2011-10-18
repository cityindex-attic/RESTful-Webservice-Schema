using System;
using System.IO;
using NUnit.Framework;

namespace JsonSchemaGeneration.Tests
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
            _patchJson = File.ReadAllText(@"TestData\valid\patch.js");
            _streamingJson = File.ReadAllText(@"TestData\valid\streaming.json");
            _smdPatchPath = @"TestData\valid\smd-patch.xml";
            _validJsonSchema = File.ReadAllText(@"TestData\valid\CIAPI.Schema.json");
            _validSMD = File.ReadAllText(@"TestData\valid\CIAPI.SMD.json");
        }

        private XmlDocSource SetupValidXmlDocSource()
        {
            _dtoAssemblyBasePath = @"TestData\valid\";
            return _wcfConfigReader.Read(@"TestData\valid\Web.Config", _patchJson, _smdPatchPath, _streamingJson);
        }

        [Test]
        public void ValidXmlShouldGenerateValidJsonSchema()
        {
            var xmlDocSource = SetupValidXmlDocSource();

            var results = _generator.GenerateJsonSchema(xmlDocSource);
            
            Assert.IsFalse(results.HasErrors,string.Format("Json Schema generation should not have failed\n\n{0}", results.ToString()));

            File.WriteAllText("Generated.Schema.json", results.JsonSchema);
            Assert.AreEqual(results.JsonSchema, _validJsonSchema);
        }

        [Test]
        public void ValidXmlShouldGenerateValidSMD()
        {
            var smd = "";
            var xmlDocSource = SetupValidXmlDocSource();

            try
            {
                var results = _generator.GenerateJsonSchema(xmlDocSource);
                smd = _generator.GenerateSmd(xmlDocSource, results.JsonSchema);
            }
            catch (Exception e)
            {
                Assert.Fail(string.Format("SMD generation should not have failed: \n\nMessage:\n{0}\n\nStackTrace:\n{1}", e.Message, e.StackTrace));
            }

            File.WriteAllText("Generated.SMD.json", smd);
            Assert.AreEqual(smd, _validSMD);
        }
    }
}
