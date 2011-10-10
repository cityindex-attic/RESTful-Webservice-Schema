using JsonSchemaGeneration.JsonSchemaDTO;
using NUnit.Framework;

namespace JsonSchemaGeneration.Tests
{
    [TestFixture]
    public class AuditorTests
    {
        [Test, Ignore("WIP")]
        public void ShouldReturnCollectionOfErrors()
        {
            var xmlDocSource = new XmlDocSource();
            var result = new Auditor().AuditTypes(xmlDocSource);
            Assert.IsTrue(result.HasErrors,"result should contain errors");
            Assert.Greater(result.MetadataGenerationErrors.Count, 0, "error count should be > 0");
        }
    }
}
