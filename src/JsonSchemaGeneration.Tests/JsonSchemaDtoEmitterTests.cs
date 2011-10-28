using System;
using JsonSchemaGeneration.JsonSchemaDTO;
using NUnit.Framework;

namespace JsonSchemaGeneration.Tests
{
    [TestFixture]
    public class JsonSchemaDtoEmitterTests: GeneratorTestsBase 
    {
        [Test]
        public void AllArrayTypesShouldBeDescribedAsArray()
        {
            var xmlDocSource = new XmlDocSource();
            xmlDocSource.Dtos.Add(DtoAssembly.CreateFromName("TestAssembly.DTO, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null"));

            var jsonSchema = new JsonSchemaDtoEmitter().EmitDtoJson(xmlDocSource);

            Console.WriteLine(jsonSchema["properties"]["ArrayTypes"]);
            Assert.That(jsonSchema["properties"]["ArrayTypes"]["properties"]["IEnumerableOfInt"]["type"].ToString(), Is.EqualTo("array"));
            Assert.That(jsonSchema["properties"]["ArrayTypes"]["properties"]["ArrayOfInt"]["type"].ToString(), Is.EqualTo("array"));
            Assert.That(jsonSchema["properties"]["ArrayTypes"]["properties"]["ListOfInt"]["type"].ToString(), Is.EqualTo("array"));
            Assert.That(jsonSchema["properties"]["ArrayTypes"]["properties"]["IListOfInt"]["type"].ToString(), Is.EqualTo("array"));
        }
    }
}