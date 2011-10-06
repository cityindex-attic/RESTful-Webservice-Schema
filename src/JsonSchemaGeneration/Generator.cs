using JsonSchemaGeneration.JsonSchemaDTO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace JsonSchemaGeneration
{
    public class Generator
    {
        public string GenerateSmd(WcfConfig reader, string jsonSchema, string patchJson, string smdPatchPath, string streaming)
        {
            var smdEmitter = new WcfSMD.Emitter();
            var smd = smdEmitter.EmitSmdJson(reader.Routes, true, reader.DTOAssemblyNames, patchJson, smdPatchPath, (JObject) JsonConvert.DeserializeObject(jsonSchema));
            
            JObject smdObj = (JObject) JsonConvert.DeserializeObject(smd);
            JObject streamingObj = (JObject) JsonConvert.DeserializeObject(streaming);
            smdObj["services"]["streaming"] = streamingObj;
            smd = smdObj.ToString(Formatting.Indented);
            
            return smd;
        }

        public string GenerateJsonSchema(WcfConfig wcfConfig)
        {
            return GenerateJsonSchema(wcfConfig, null);
        }

        public string GenerateJsonSchema(WcfConfig wcfConfig, string schemaPatchPath)
        {
            XmlDocUtils.EnsureXmlDocsAreValid(schemaPatchPath, wcfConfig.DTOAssemblyNames);

            new Auditor().AuditTypes(schemaPatchPath,wcfConfig.DTOAssemblyNames);

            return new JsonSchemaDtoEmitter().EmitDtoJson(schemaPatchPath, wcfConfig.DTOAssemblyNames);
        }
    }
}
