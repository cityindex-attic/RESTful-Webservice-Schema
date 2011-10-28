using System;
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

        [Test]
        public void AllArrayTypesShouldValidate()
        {
            var xmlDocSource = new XmlDocSource();
            xmlDocSource.Dtos.Add(DtoAssembly.CreateFromName("TestAssembly.DTO, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null"));

            var result = new Auditor().AuditTypes(xmlDocSource);

            result.MetadataGenerationErrors.ForEach(e => Console.WriteLine(e.ToString()));
            Assert.AreEqual(0, result.MetadataGenerationErrors.Count, "No errors should have been reported");
        }
    }
}
