using System;
using System.IO;
using System.Linq;
using MetadataGeneration.Core;

namespace MetadataGenerator
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

                // todo parameterize
                var patchJson = File.ReadAllText("patch.js");
                
                var xmlDocSource = reader.Read(intputFileName);

                var results = generator.GenerateJsonSchema(xmlDocSource);

                if (results.MetadataGenerationErrors.Count > 0)
                {
                    throw new Exception(string.Join(@"\n", results.MetadataGenerationErrors.Select(e => e.ToString())));
                }

                File.WriteAllText(jschemaOutputFileName, results.JsonSchema.ToString());

                var smdResults = generator.GenerateSmd(xmlDocSource, results.JsonSchema);

                if (smdResults.MetadataGenerationErrors.Count > 0)
                {
                    throw new Exception(string.Join(@"\n", smdResults.MetadataGenerationErrors.Select(e => e.ToString())));
                }

                var streaming = File.ReadAllText("streaming.json");
                generator.AddStreamingSMD(smdResults.SMD, streaming);

                File.WriteAllText(smdOutputFileName, smdResults.SMD.ToString());
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
