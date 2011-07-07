using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using JsonSchemaGeneration.JsonSchemaDTO;

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
                string namespaceName = args[0];
                string outputFileName = args[1];

                List<string> assemblyFileNames = new List<string>();
                for (int i = 2; i < args.Length; i++)
                {
                    assemblyFileNames.Add(args[i]);
                }

                List<string> assemblyDisplayNames = new List<string>();

                foreach (var assemblyFileName in assemblyFileNames)
                {
                    var asm = Assembly.ReflectionOnlyLoadFrom(assemblyFileName);
                    assemblyDisplayNames.Add(asm.FullName);
                }



                XmlDocUtils.EnsureXmlDocsAreValid(assemblyDisplayNames.ToArray());

                new Auditor().AuditTypes(assemblyDisplayNames.ToArray());

                var result = new JsonSchemaDtoEmitter().EmitDtoJson(namespaceName, assemblyDisplayNames.ToArray());

                File.WriteAllText(outputFileName, result);
            }
            catch (Exception e)
            {

                Console.WriteLine(e.ToString());
                DisplayUsage();
            }

        }

        static void DisplayUsage()
        {
            Console.WriteLine(@"JschemaGenerator Usage:

JschemaGenerator generated-namespace  output-file-path [assembly file names]

Place appropriately decorated assemblies and xml files in the 
execution directory manually or via build step and specify assembly names in command line.");
        }
    }
}
