using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using JsonSchemaGeneration;
using JsonSchemaGeneration.JsonSchemaDTO;
using JsonSchemaGeneration.WcfSMD;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace JschemaGenerator
{
    class Program
    {
        static void Main(string[] args)
        {

            if (args.Length == 0)
            {
                DisplayUsage();
                return;
            }

            try
            {
                string intputFileName = args[0];
                string jschemaOutputFileName = args[1];
                string smdOutputFileName = args[2];


                string smdPatchPath = "smd-patch.xml";
                string schemaPatchPath = null;

                // todo parameterize
                string patchJson = File.ReadAllText("patch.js");


                WcfConfigReader reader = new WcfConfigReader();

                reader.Read(intputFileName);

                XmlDocUtils.EnsureXmlDocsAreValid(schemaPatchPath, reader.DTOAssemblyNames);

                new Auditor().AuditTypes(schemaPatchPath,reader.DTOAssemblyNames);

                var jsonSchema = new JsonSchemaDtoEmitter().EmitDtoJson(schemaPatchPath, reader.DTOAssemblyNames);

                File.WriteAllText(jschemaOutputFileName, jsonSchema);

                

                var smdEmitter = new JsonSchemaGeneration.WcfSMD.Emitter();

                var smd = smdEmitter.EmitSmdJson(reader.Routes, true, reader.DTOAssemblyNames, patchJson, smdPatchPath, (JObject) JsonConvert.DeserializeObject(jsonSchema));

                JObject smdObj = (JObject) JsonConvert.DeserializeObject(smd);
                var streaming = File.ReadAllText("streaming.json");
                JObject streamingObj = (JObject) JsonConvert.DeserializeObject(streaming);
                smdObj["services"]["streaming"] = streamingObj;
                smd = smdObj.ToString(Formatting.Indented);
                File.WriteAllText(smdOutputFileName, smd);

 
            }
            catch (Exception e)
            {

                Console.WriteLine(e.ToString());
                DisplayUsage();
            }

            Console.WriteLine("press enter to exit. ;-)");
            Console.ReadLine();

        }

        static void DisplayUsage()
        {
            Console.WriteLine("JschemaGenerator Usage:\nJschemaGenerator config-input-file-path schema-output-file-path smd-output-file-path");
        }
    }
}
