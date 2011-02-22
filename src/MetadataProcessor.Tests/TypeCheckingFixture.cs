using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml.Linq;
using MetadataProcessor.Tests.TestDTO.Request;
using Newtonsoft.Json.Linq;
using NUnit.Framework;

namespace MetadataProcessor.Tests
{
    [TestFixture]
    public class TypeCheckingFixture
    {

        [Test]
        public void ThrowsOnTypeMissingFromJSchema()
        {
            Assembly dtoAssembly = typeof(MetadataProcessor.DTOTestAssembly.Class1).Assembly;
            JObject schemaObj = BuildSchema(dtoAssembly);
            schemaObj.Remove("Class2");
            string schema = schemaObj.ToString();
            TypeChecker checker = new TypeChecker();
            Assert.Throws<MetadataProcessor.MetaVerificationException>(() =>
            {
                checker.CheckSchema(schema, dtoAssembly);
            }, "type Class2 is not represented in schema");

        }

        [Test]
        public void EnsureTypesAreRepresentedInJsonSchema()
        {

            Assembly dtoAssembly = typeof(MetadataProcessor.DTOTestAssembly.Class1).Assembly;
            string schema = BuildSchema(dtoAssembly).ToString();
            TypeChecker checker = new TypeChecker();
            checker.CheckSchema(schema, dtoAssembly);
        }

        [Test]
        public void CanIdentifyNonNullableType()
        {
            var actual = JsonSchemaUtilities.GetNullableTypeIfAny(typeof(string));
            Assert.AreEqual(typeof(string), actual);
        }
        [Test]
        public void CanIdentifyNullableType()
        {
            var actual = JsonSchemaUtilities.GetNullableTypeIfAny(typeof(DateTime?));
            Assert.AreEqual(typeof(DateTime), actual);
        }

        [Test]
        public void CanIdentifyListType()
        {

            Assert.Throws<Exception>(() => JsonSchemaUtilities.IsCollectionType(typeof(List<string>)), "List`1 types are not supported. Use IList");
        }

        [Test]
        public void CanIdentifyIListType()
        {
            Type type = typeof(IList<string>);
            Assert.IsTrue(JsonSchemaUtilities.IsCollectionType(type));

            var actual = JsonSchemaUtilities.GetCollectionType(type);
            Assert.AreEqual(typeof(string), actual);

        }

        [Test]
        public void CanIdentifyArrayType()
        {

            Assert.Throws<Exception>(() => JsonSchemaUtilities.IsCollectionType(typeof(string[])), "array types are not supported. Use IList");
        }

        [Test]
        public void CanIdentifyNonCollectionType()
        {
            Type type = typeof(string);
            Assert.IsFalse(JsonSchemaUtilities.IsCollectionType(type));

            var actual = JsonSchemaUtilities.GetCollectionType(type);
            Assert.IsNull(actual);

        }


        [Test, Ignore("TODO: decide with david whether to place IList restriction in MetadataProcessor or in the test. Metadata is the most obvious place but would have to be configurable")]
        public void Test()
        {
            Type type = typeof (BadDTOTestAssembly.ClassWithBadCollections);
            Assembly assembly = type.Assembly;
            var doc = XmlDocExtensions.GetXmlDocs(assembly.GetTypes()[0]);
            Assert.Throws<Exception>(() => JsonSchemaUtilities.BuildTypeSchema(type, doc, false),"");
            
        }


        public static JObject BuildSchema(Assembly assembly)
        {
            var schema = new JObject();


            var doc = XmlDocExtensions.GetXmlDocs(assembly.GetTypes()[0]);


            foreach (var type in assembly.GetTypes())
            {
                var typeSchema = JsonSchemaUtilities.BuildTypeSchema(type, doc, false);
                if (typeSchema != null)
                {
                    schema.Add(type.Name, typeSchema);
                }
            }


            return schema;
        }
    }

}
