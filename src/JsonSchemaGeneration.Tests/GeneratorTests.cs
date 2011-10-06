using System;
using System.IO;
using System.Reflection;
using NUnit.Framework;

namespace JsonSchemaGeneration.Tests
{
    [TestFixture]
    public class GeneratorTests
    {
        private string _dtoAssemblyBasePath;
        private string _patchJson;
        private string _smdPatchPath;
        private string _streamingJson;
        private WcfConfigReader _wcfConfigReader = new WcfConfigReader();

        public GeneratorTests()
        {
            _patchJson = File.ReadAllText(@"TestData\valid\patch.js");
            _streamingJson = File.ReadAllText(@"TestData\valid\streaming.json");
            _smdPatchPath = @"TestData\valid\smd-patch.xml";

            //Ensure we also look for assemblies referenced in the Web.Config in the specified dtoAssemblyBasePath
            AppDomain.CurrentDomain.AssemblyResolve += (o, args) => {
                    var assemblyname = args.Name.Split(',')[0];
                    var assemblyFileName = Path.Combine(_dtoAssemblyBasePath, assemblyname + ".dll");
                    var assembly = Assembly.LoadFrom(assemblyFileName);
                    return assembly; 
            };
        }

        [Test]
        public void ValidXmlShouldGenerateValidJsonSchema()
        {
            var jsonSchema = "";
            try
            {
                _dtoAssemblyBasePath = @"TestData\valid\";
                var xmlDocSource = _wcfConfigReader.Read(@"TestData\valid\Web.Config");

                jsonSchema = new Generator().GenerateJsonSchema(xmlDocSource, null);
                File.WriteAllText("Generated.Schema.json", jsonSchema);
            }
            catch (Exception e)
            {
                Assert.Fail(string.Format("Json Schema generation should not have failed\n\nMessage:\n{0}\n\nStackTrace:\n{1}", e.Message, e.StackTrace));
            }

            Assert.AreEqual(jsonSchema, File.ReadAllText(@"TestData\valid\CIAPI.Schema.json"));
        }

        [Test]
        public void ValidXmlShouldGenerateValidSMD()
        {
            var smd = "";
            try
            {
                _dtoAssemblyBasePath = @"TestData\valid\";
                var xmlDocSource = _wcfConfigReader.Read(@"TestData\valid\Web.Config");

                var generator = new Generator();
                var jsonSchema = generator.GenerateJsonSchema(xmlDocSource, null);
                smd = generator.GenerateSmd(xmlDocSource, jsonSchema, _patchJson, _smdPatchPath, _streamingJson);
                File.WriteAllText("Generated.SMD.json", smd);
            }
            catch (Exception e)
            {
                Assert.Fail(string.Format("SMD generation should not have failed: \n\nMessage:\n{0}\n\nStackTrace:\n{1}", e.Message, e.StackTrace));
            }

            Assert.AreEqual(smd, File.ReadAllText(@"TestData\valid\CIAPI.SMD.json"));
        }
    }
}
