using System;
using System.IO;
using System.Reflection;
using NUnit.Framework;

namespace JsonSchemaGeneration.Tests
{
    [TestFixture]
    public class GeneratorExpectedFailuresTests
    {
        private string _dtoAssemblyBasePath;
        private WcfConfigReader _wcfConfigReader = new WcfConfigReader();
        private Generator _generator = new Generator();
        
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
            var xmlDocSource = _wcfConfigReader.Read(@"TestData\invalid\RESTWebservices.0.869\Web.Config", "", null, "");

            try
            {
                var jsonSchema = _generator.GenerateJsonSchema(xmlDocSource);
                _generator.GenerateSmd(xmlDocSource, jsonSchema);
                Assert.Fail("Exception should have been thrown");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                StringAssert.Contains("ILoginService", e.ToString());
                StringAssert.Contains("Schema missing referenced return type ApiChangePasswordResponseDTO for method ChangePassword", e.ToString());
            }
        }
    }
}
