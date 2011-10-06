using JsonSchemaGeneration.JsonSchemaDTO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace JsonSchemaGeneration
{
    public class Generator
    {
        public string GenerateJsonSchema(XmlDocSource xmlDocSource)
        {
            XmlDocUtils.EnsureXmlDocsAreValid(xmlDocSource);

            new Auditor().AuditTypes(xmlDocSource);

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
