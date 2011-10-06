using JsonSchemaGeneration.JsonSchemaDTO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace JsonSchemaGeneration
{
    public class Generator
    {
        public string GenerateSmd(WcfConfigReader reader, string jsonSchema, string patchJson, string smdPatchPath, string streaming)
        {
            var smdEmitter = new WcfSMD.Emitter();
            var smd = smdEmitter.EmitSmdJson(reader.Routes, true, reader.DTOAssemblyNames, patchJson, smdPatchPath, (JObject) JsonConvert.DeserializeObject(jsonSchema));
            
            JObject smdObj = (JObject) JsonConvert.DeserializeObject(smd);
            JObject streamingObj = (JObject) JsonConvert.DeserializeObject(streaming);
            smdObj["services"]["streaming"] = streamingObj;
            smd = smdObj.ToString(Formatting.Indented);
            
            return smd;
        }

        public string GenerateJsonSchema(WcfConfigReader wcfConfigReader)
        {
            return GenerateJsonSchema(wcfConfigReader, null);
        }

        public string GenerateJsonSchema(WcfConfigReader wcfConfigReader, string schemaPatchPath)
        {
            XmlDocUtils.EnsureXmlDocsAreValid(schemaPatchPath, wcfConfigReader.DTOAssemblyNames);

            new Auditor().AuditTypes(schemaPatchPath,wcfConfigReader.DTOAssemblyNames);

            return new JsonSchemaDtoEmitter().EmitDtoJson(schemaPatchPath, wcfConfigReader.DTOAssemblyNames);
        }
    }
}
