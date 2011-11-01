using MetadataGeneration.Core.JsonSchemaDTO;
using MetadataGeneration.Core.WcfSMD;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MetadataGeneration.Core
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

        public MetadataGenerationResult GenerateSmd(XmlDocSource xmlDocSource, JObject jsonSchema)
        {
            var result = new Emitter().EmitSmdJson(xmlDocSource, true, jsonSchema);
            return result;
        }

        public void AddStreamingSMD(JObject smd, string streamingJsonPatch)
        {
            smd["services"]["streaming"] = (JObject) JsonConvert.DeserializeObject(streamingJsonPatch);
        }
    }
}
