using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using JsonSchemaGeneration;
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
                string intputFileName = args[0];
                string jschemaOutputFileName = args[1];
                string smdOutputFileName = args[2];
                
                
                // todo parameterize
                string patchJson = File.ReadAllText("patch.js");


                WcfConfigReader reader = new WcfConfigReader();

                reader.Read(intputFileName);

                XmlDocUtils.EnsureXmlDocsAreValid(reader.DTOAssemblyNames);

                new Auditor().AuditTypes(reader.DTOAssemblyNames);

                var jsonSchema = new JsonSchemaDtoEmitter().EmitDtoJson(reader.DTOAssemblyNames);

                File.WriteAllText(jschemaOutputFileName, jsonSchema);


                var smdEmitter = new JsonSchemaGeneration.WcfSMD.Emitter();
                
                var smd = smdEmitter.EmitSmdJson(reader.Routes, true, reader.DTOAssemblyNames,patchJson);
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
