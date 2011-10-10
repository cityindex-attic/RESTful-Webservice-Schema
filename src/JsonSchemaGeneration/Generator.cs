using JsonSchemaGeneration.JsonSchemaDTO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace JsonSchemaGeneration
{
    public class Generator
    {
        public string GenerateJsonSchema(XmlDocSource xmlDocSource)
        {
            //Checks that DTOs all have valid XML comments
            XmlDocUtils.EnsureXmlDocsAreValid(xmlDocSource);

            //Checks that each DTO type can be documented
            new Auditor().AuditTypes(xmlDocSource);

            //Creates Jschema for all DTO types where it can find XML docs
            //NB, Auditor errors if any of the DTOs don't have XML docs
            return new JsonSchemaDtoEmitter().EmitDtoJson(xmlDocSource);
        }

        public string GenerateSmd(XmlDocSource xmlDocSource, string jsonSchema)
        {
            var smdEmitter = new WcfSMD.Emitter();
            var smd = smdEmitter.EmitSmdJson(xmlDocSource, true, (JObject) JsonConvert.DeserializeObject(jsonSchema));
            
            JObject smdObj = (JObject) JsonConvert.DeserializeObject(smd);
            JObject streamingObj = (JObject) JsonConvert.DeserializeObject(xmlDocSource.StreamingJsonPatch);
            smdObj["services"]["streaming"] = streamingObj;
            smd = smdObj.ToString(Formatting.Indented);
            
            return smd;
        }
    }
}
