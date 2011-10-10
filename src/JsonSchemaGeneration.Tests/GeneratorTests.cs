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
        private Generator _generator = new Generator();
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

        [TestFixtureSetUp]
        public void AddCustomAssemblyResolver()
        {
            //Ensure we also look for assemblies referenced in the Web.Config in the specified dtoAssemblyBasePath
            AppDomain.CurrentDomain.AssemblyResolve += CustomAssemblyResolver;
        }

        private Assembly CustomAssemblyResolver(object o, ResolveEventArgs args)
        {
            var assemblyname = args.Name.Split(',')[0];
            var assemblyFileName = Path.Combine(_dtoAssemblyBasePath, assemblyname + ".dll");
            var assembly = Assembly.LoadFrom(assemblyFileName);
            return assembly; 
        }

        [TestFixtureTearDown]
        public void RemoveCustomAssemblyResolver()
        {
            AppDomain.CurrentDomain.AssemblyResolve -= CustomAssemblyResolver;
        }

        private XmlDocSource SetupValidXmlDocSource()
        {
            _dtoAssemblyBasePath = @"TestData\valid\";
            return _wcfConfigReader.Read(@"TestData\valid\Web.Config", _patchJson, _smdPatchPath, _streamingJson);
        }

        [Test]
        public void ValidXmlShouldGenerateValidJsonSchema()
        {
            var jsonSchema = "";
            var xmlDocSource = SetupValidXmlDocSource();

            try
            {
                jsonSchema = _generator.GenerateJsonSchema(xmlDocSource);
            }
            catch (Exception e)
            {
                Assert.Fail(string.Format("Json Schema generation should not have failed\n\nMessage:\n{0}\n\nStackTrace:\n{1}", e.Message, e.StackTrace));
            }

            File.WriteAllText("Generated.Schema.json", jsonSchema);
            Assert.AreEqual(jsonSchema, _validJsonSchema);
        }

        [Test]
        public void ValidXmlShouldGenerateValidSMD()
        {
            var smd = "";
            var xmlDocSource = SetupValidXmlDocSource();

            try
            {
                var jsonSchema = _generator.GenerateJsonSchema(xmlDocSource);
                smd = _generator.GenerateSmd(xmlDocSource, jsonSchema);
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
