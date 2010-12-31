using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml.Linq;
using MetadataProcessor.Tests.TestDTO;
using Newtonsoft.Json.Linq;
using NUnit.Framework;

namespace MetadataProcessor.Tests
{
    [TestFixture]
    public class JsonSchemaFixture
    {

        public static string GetTestTarget(string name)
        {
            return File.ReadAllText(@"..\..\test-targets\" + name + ".txt");
        }

        [Test]
        public void SimpleTypeBaseCheck()
        {
            Assert.AreEqual("{\r\n  \"type\": \"string\"\r\n}", JsonSchemaUtilities.BuildPropertyBase(typeof(string)).ToString());
            Assert.AreEqual("{\r\n  \"type\": \"array\",\r\n  \"items\": {\r\n    \"type\": \"string\"\r\n  }\r\n}", JsonSchemaUtilities.BuildPropertyBase(typeof(string[])).ToString());
            Assert.AreEqual("{\r\n  \"type\": \"array\",\r\n  \"items\": {\r\n    \"type\": \"string\"\r\n  }\r\n}", JsonSchemaUtilities.BuildPropertyBase(typeof(IList<string>)).ToString());
            

            Assert.AreEqual("{\r\n  \"type\": \"integer\"\r\n}", JsonSchemaUtilities.BuildPropertyBase(typeof(int)).ToString());
            Assert.AreEqual("{\r\n  \"type\": \"array\",\r\n  \"items\": {\r\n    \"type\": \"integer\"\r\n  }\r\n}", JsonSchemaUtilities.BuildPropertyBase(typeof(int[])).ToString());
            Assert.AreEqual("{\r\n  \"type\": \"array\",\r\n  \"items\": {\r\n    \"type\": \"integer\"\r\n  }\r\n}", JsonSchemaUtilities.BuildPropertyBase(typeof(IList<int>)).ToString());



            Assert.AreEqual("{\r\n  \"$ref\": \"#.JSchemaDTO\"\r\n}", JsonSchemaUtilities.BuildPropertyBase(typeof(JSchemaDTO)).ToString());
            Assert.AreEqual("{\r\n  \"type\": \"array\",\r\n  \"items\": {\r\n    \"$ref\": \"#.JSchemaDTO\"\r\n  }\r\n}", JsonSchemaUtilities.BuildPropertyBase(typeof(JSchemaDTO[])).ToString());
            Assert.AreEqual("{\r\n  \"type\": \"array\",\r\n  \"items\": {\r\n    \"$ref\": \"#.JSchemaDTO\"\r\n  }\r\n}", JsonSchemaUtilities.BuildPropertyBase(typeof(IList<JSchemaDTO>)).ToString());

            Assert.AreEqual("{\r\n  \"$ref\": \"#.TestEnum\"\r\n}", JsonSchemaUtilities.BuildPropertyBase(typeof(TestEnum)).ToString());

        }

        [Test]
        public void CanLoadDocs()
        {
            var target = JsonSchemaUtilities.GetXmlDocs(typeof(JSchemaDTO));
            Assert.AreEqual("doc", target.Root.Name.ToString());
        }

        [Test]
        public void CanGetMemberNode()
        {
            var doc = JsonSchemaUtilities.GetXmlDocs(typeof(JSchemaDTO));
            Assert.IsNotNull(JsonSchemaUtilities.GetMemberNode(doc, "T:MetadataProcessor.Tests.TestDTO.JSchemaDTO"));

        }

        [Test]
        public void CanGetMemberNodes()
        {
            var doc = JsonSchemaUtilities.GetXmlDocs(typeof(JSchemaDTO));
            IEnumerable<XElement> actual = JsonSchemaUtilities.GetMemberNodes(doc, "P:MetadataProcessor.Tests.TestDTO.JSchemaDTO");
            Assert.Greater(actual.Count(), 0);
        }

        [Test]
        public void ApplyDescription()
        {
            JObject propBase = JsonSchemaUtilities.BuildPropertyBase(typeof(string));
            XDocument doc = JsonSchemaUtilities.GetXmlDocs(typeof(JSchemaDTO));
            XElement propElement = JsonSchemaUtilities.GetMemberNode(doc, "P:MetadataProcessor.Tests.TestDTO.JSchemaDTO.StringProperty");
            // apply description

            JsonSchemaUtilities.ApplyDescription(propBase, propElement);

            var propJSON = propBase.ToString();

            Assert.AreEqual(GetTestTarget("ApplyDescription"), propJSON);

        }

        // out-of-band demoValue

        [Test]
        public void ApplyStringDemoValue()
        {
            JObject propBase = JsonSchemaUtilities.BuildPropertyBase(typeof(string));
            XDocument doc = JsonSchemaUtilities.GetXmlDocs(typeof(JSchemaDTO));
            XElement propElement = JsonSchemaUtilities.GetMemberNode(doc, "P:MetadataProcessor.Tests.TestDTO.JSchemaDTO.StringProperty");

            XElement jschema = propElement.Descendants("jschema").FirstOrDefault();

            var demoValueAttribute = jschema.Attributes("demoValue").FirstOrDefault();

            if (demoValueAttribute != null)
            {
                JsonSchemaUtilities.ApplyTypedValue(propBase, demoValueAttribute);
            }

            var propJSON = propBase.ToString();
            Assert.AreEqual(GetTestTarget("ApplyStringDemoValue"), propJSON);
        }


        [Test]
        public void ApplyStringArrayDemoValue()
        {
            JObject propBase = JsonSchemaUtilities.BuildPropertyBase(typeof(string[]));
            XDocument doc = JsonSchemaUtilities.GetXmlDocs(typeof(JSchemaDTO));
            XElement propElement = JsonSchemaUtilities.GetMemberNode(doc, "P:MetadataProcessor.Tests.TestDTO.JSchemaDTO.StringArrayProperty");

            XElement jschema = propElement.Descendants("jschema").FirstOrDefault();

            var demoValueAttribute = jschema.Attributes("demoValue").FirstOrDefault();

            if (demoValueAttribute != null)
            {
                JsonSchemaUtilities.ApplyTypedValue(propBase, demoValueAttribute);
            }

            var propJSON = propBase.ToString();
            Assert.AreEqual(GetTestTarget("ApplyStringArrayDemoValue"), propJSON);
        }



        [Test]
        public void ApplyIntArrayDemoValue()
        {
            JObject propBase = JsonSchemaUtilities.BuildPropertyBase(typeof(int[]));
            XDocument doc = JsonSchemaUtilities.GetXmlDocs(typeof(JSchemaDTO));
            XElement propElement = JsonSchemaUtilities.GetMemberNode(doc, "P:MetadataProcessor.Tests.TestDTO.JSchemaDTO.IntArrayProperty");

            XElement jschema = propElement.Descendants("jschema").FirstOrDefault();

            var demoValueAttribute = jschema.Attributes("demoValue").FirstOrDefault();

            if (demoValueAttribute != null)
            {
                JsonSchemaUtilities.ApplyTypedValue(propBase, demoValueAttribute);
            }

            var propJSON = propBase.ToString();
            Assert.AreEqual(GetTestTarget("ApplyIntArrayDemoValue"), propJSON);
        }


        [Test]
        public void ApplyStringPropertyAttributes()
        {
            JObject propBase = JsonSchemaUtilities.BuildPropertyBase(typeof(string));
            XDocument doc = JsonSchemaUtilities.GetXmlDocs(typeof(JSchemaDTO));
            XElement propElement = JsonSchemaUtilities.GetMemberNode(doc, "P:MetadataProcessor.Tests.TestDTO.JSchemaDTO.StringProperty");

            XElement jschema = propElement.Descendants("jschema").FirstOrDefault();
            foreach (var attribute in jschema.Attributes())
            {
                JsonSchemaUtilities.ApplyPropertyAttribute(propBase, attribute, typeof(JSchemaDTO).FullName, "StringProperty");
            }


            var propJSON = propBase.ToString();
            Assert.AreEqual(GetTestTarget("ApplyStringPropertyAttributes"), propJSON);

        }


        [Test]
        public void BuildEnumSchema()
        {
            XDocument doc = JsonSchemaUtilities.GetXmlDocs(typeof(TestEnum));
            var actual = JsonSchemaUtilities.BuildEnumSchema(doc, typeof(TestEnum),true);
            var propJSON = actual.ToString();
            Assert.AreEqual(GetTestTarget("BuildEnumSchema"), propJSON);
        }

        [Test]
        public void BuildTypeSchema()
        {
            var type = typeof(JSchemaDTO);
            var doc = JsonSchemaUtilities.GetXmlDocs(type);

            var jsob = JsonSchemaUtilities.BuildTypeSchema(type, doc, true);

            var jsobJSON = jsob.ToString();
            Assert.AreEqual(GetTestTarget("BuildTypeSchema"), jsobJSON);


        }

        [Test]
        public void BuildDerivedTypeSchema()
        {
            var type = typeof(JSchemaDTOImpl);
            var doc = JsonSchemaUtilities.GetXmlDocs(type);

            var jsob = JsonSchemaUtilities.BuildTypeSchema(type, doc, true);

            var jsobJSON = jsob.ToString();
            Assert.AreEqual(GetTestTarget("BuildDerivedTypeSchema"), jsobJSON);

        }


        [Test, Ignore("dependant on external assembly")]
        public void GenerateRWSDTO()
        {
            JObject properties = new JObject();
            JObject schema = new JObject();
            schema.Add("properties", properties);


            var assemblyNames = new[] { "TradingApi.CoreDTO, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "RESTWebServicesDTO, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null" };
            foreach (var assemblyName in assemblyNames)
            {
                var assembly = Assembly.Load(assemblyName);

                foreach (var type in assembly.GetTypes())
                {
                    var doc = JsonSchemaUtilities.GetXmlDocs(type);
                    var typeSchema = JsonSchemaUtilities.BuildTypeSchema(type, doc, true);
                    if (typeSchema != null)
                    {
                        properties.Add(type.Name, typeSchema);
                    }
                }
                
            }


            var schemaJSON = schema.ToString();
            Assert.AreEqual(GetTestTarget("GenerateRWSDTO"), schemaJSON);
        }

        [Test]
        public void ThrowsMeaningfulErrorWhenDemoValueIsInvalid()
        {
            var type = typeof(WithInvalidDemoValue);
            var doc = JsonSchemaUtilities.GetXmlDocs(type);

            try
            {
                JsonSchemaUtilities.BuildTypeSchema(type, doc, true);
                Assert.Fail("Exception should have been thrown");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.InnerException.Message);

                StringAssert.Contains("WithInvalidDemoValue", ex.Message, "Must mention the DTO class");
                StringAssert.Contains("UserName", ex.Message, "Must mention the property");
                StringAssert.Contains("demoValue=\"buy\"", ex.InnerException.Message,"Must mention the invalid attribute");
                StringAssert.Contains("ACustomType", ex.InnerException.Message,"Must mention the type which is causing the error");
            }
           
        }
    }
}
