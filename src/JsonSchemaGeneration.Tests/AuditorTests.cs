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

        [Test, Ignore("WIP")]
        public void InvalidCollectionTypesShouldBeReportedAsErrors()
        {
            var xmlDocSource = new XmlDocSource();
            xmlDocSource.Dtos.Add(DtoAssembly.CreateFromName("TestAssembly.BadDTO, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null"));

            var result = new Auditor().AuditTypes(xmlDocSource);

            Assert.Greater(result.MetadataGenerationErrors.Count, 0, "Errors should have been reported");
            Assert.AreEqual("IEnumerable are not supported. Use IList", result.MetadataGenerationErrors[0].ErrorReason);
        }
    }
}
