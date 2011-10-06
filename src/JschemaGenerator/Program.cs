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
            var generator = new Generator();
            WcfConfigReader reader = new WcfConfigReader();

            if (args.Length == 0)
            {
                DisplayUsage();
                return;
            }

            try
            {
                var intputFileName = args[0];
                var jschemaOutputFileName = args[1];
                var smdOutputFileName = args[2];
                var smdPatchPath = "smd-patch.xml";
                string schemaPatchPath = null;

                var streaming = File.ReadAllText("streaming.json");

                // todo parameterize
                var patchJson = File.ReadAllText("patch.js");
                
                var wcfConfig = reader.Read(intputFileName, patchJson, smdPatchPath, streaming);

                var jsonSchema = generator.GenerateJsonSchema(wcfConfig, schemaPatchPath);

                File.WriteAllText(jschemaOutputFileName, jsonSchema);

                var smd = generator.GenerateSmd(wcfConfig, jsonSchema);

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
