using JsonSchemaGeneration.JsonSchemaDTO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace JsonSchemaGeneration
{
    public class Generator
    {
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

        public string GenerateJsonSchema(XmlDocSource xmlDocSource, string schemaPatchPath)
        {
            XmlDocUtils.EnsureXmlDocsAreValid(schemaPatchPath, xmlDocSource);

            new Auditor().AuditTypes(schemaPatchPath,xmlDocSource);

            return new JsonSchemaDtoEmitter().EmitDtoJson(schemaPatchPath, xmlDocSource);
        }
    }
}
