using System;
using System.Linq;
using JsonSchemaGeneration.JsonSchemaDTO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace JsonSchemaGeneration
{
    public class Generator
    {
        public MetadataGenerationResult GenerateJsonSchema(XmlDocSource xmlDocSource)
        {
            var results = new MetadataGenerationResult();
            //Checks that DTOs all have valid XML comments
            XmlDocUtils.EnsureXmlDocsAreValid(xmlDocSource);

            //Checks that each DTO type can be documented
            results.AddValidationResults(new Auditor().AuditTypes(xmlDocSource));

            //Creates Jschema for all DTO types where it can find XML docs
            results.JsonSchema = new JsonSchemaDtoEmitter().EmitDtoJson(xmlDocSource);

            return results;
        }

        public JObject GenerateSmd(XmlDocSource xmlDocSource, JObject jsonSchema)
        {
            var smdEmitter = new WcfSMD.Emitter();
            var smd = smdEmitter.EmitSmdJson(xmlDocSource, true, jsonSchema);
            
            return smd;
        }

        public string AddStreamingSMD(string smd, string streamingJsonPatch)
        {
            JObject smdObj = (JObject) JsonConvert.DeserializeObject(smd);
            JObject streamingObj = (JObject) JsonConvert.DeserializeObject(streamingJsonPatch);
            smdObj["services"]["streaming"] = streamingObj;
            smd = smdObj.ToString(Formatting.Indented);
            return smd;
        }
    }
}
