using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using MetadataGeneration.Core;

namespace MetadataGenerator
{
    class Program
    {
        enum ExitCode : int
        {
            Success = 0,
            Failure = 1
        }

        static int Main(string[] args)
        {
            var generator = new Generator();
            var reader = new WcfConfigReader();

            if (args.Length == 0)
            {
                DisplayUsage();
                return (int)ExitCode.Failure;
            }

            var configFile = ExtractArg(args,"--ConfigFile");
            var jschemaOutputFileName = ExtractArg(args, "--JschemaOutput");
            var smdOutputFileName = ExtractArg(args, "--SMDOutput");
            var streamingSMD = ExtractArg(args, "--StreamingSMD");
                
            var xmlDocSource = reader.Read(configFile);

            var jsonSchemaResults = generator.GenerateJsonSchema(xmlDocSource);
            File.WriteAllText(jschemaOutputFileName, jsonSchemaResults.JsonSchema.ToString());

            var smdResults = generator.GenerateSmd(xmlDocSource, jsonSchemaResults.JsonSchema);

            var streaming = File.ReadAllText(streamingSMD);
            generator.AddStreamingSMD(smdResults.SMD, streaming);

            File.WriteAllText(smdOutputFileName, smdResults.SMD.ToString());

            if (jsonSchemaResults.HasErrors || smdResults.HasErrors)
            {
                Console.WriteLine(string.Join(@"\r\n", jsonSchemaResults.MetadataGenerationErrors.Select(e => e.ToString())));
                Console.WriteLine(string.Join(@"\r\n", jsonSchemaResults.MetadataGenerationErrors.Select(e => e.ToString())));
                return (int)ExitCode.Failure;
            }

            return (int)ExitCode.Success;
        }

        private static string ExtractArg(IEnumerable<string> args, string paramName)
        {
            var argPart = args.First(arg => arg.StartsWith(paramName));
            return argPart.Split('=')[1];
        }

        static void DisplayUsage()
        {
            Console.WriteLine(@"MetadataGenerator Usage:\r\n" 
                + @"MetadataGenerator --ConfigFile={config-input-file-path} "
                + @"--StreamingSMD={location-of-streaming-smd} " 
                + @"--JschemaOutput={jschema-output-file-path} " 
                + @"--SMDOutput={smd-output-file-path}");
        }
    }


}