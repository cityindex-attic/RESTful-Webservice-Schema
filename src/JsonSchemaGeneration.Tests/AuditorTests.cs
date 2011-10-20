using System.Collections.Generic;
using System.Reflection;
using JsonSchemaGeneration.JsonSchemaDTO;
using NUnit.Framework;

namespace JsonSchemaGeneration.Tests
{
    [TestFixture]
    public class AuditorTests : GeneratorTestsBase
    {
        [Test]
        public void ValidSourceShouldHaveCollectionOfSuccessesAndNoErrors()
        {
            _dtoAssemblyBasePath = @"TestData\valid\";
            var xmlDocSource = _wcfConfigReader.Read(@"TestData\valid\Web.Config");

            var result = new Auditor().AuditTypes(xmlDocSource);
            Assert.Greater(result.MetadataGenerationSuccesses.Count, 0, "success count should be > 0");
            Assert.AreEqual(result.MetadataGenerationErrors.Count, 0, "error count should be 0");
        }

//        [Test]
//        public void InvalidCollectionTypesShouldBeReportedAsErrors()
//        {
//            var xmlDocSource = new XmlDocSource();
//            xmlDocSource.Dtos.Add(new DtoAssembly
//            {
//                Assembly = Assembly.Load(dtoAssemblyName),
//                AssemblyXML = LoadXml(Assembly.Load(dtoAssemblyName))
//            });
//
//            var result = new Auditor().AuditTypes(xmlDocSource);
//
//            Assert.Greater(result.MetadataGenerationErrors.Count, 0, "Errors should have been reported");
//        }
    }
}
