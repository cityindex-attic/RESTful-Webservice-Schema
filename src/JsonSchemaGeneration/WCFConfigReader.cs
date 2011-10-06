using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using System.Xml.XPath;
using TradingApi.Configuration;

namespace JsonSchemaGeneration
{
    public class WcfConfigReader
    {
        public XmlDocSource Read(string configPath, string patchJson, string smdPatchPath, string streamingJson)
        {
            var wcfConfig = new XmlDocSource { JsonSchemaPatch = patchJson, StreamingJsonPatch = streamingJson, SMDPatchPath = smdPatchPath };
            var config = XDocument.Load(configPath);
            var apiNode = config.XPathSelectElement("configuration/tradingApi");
            var profile = apiNode.XPathSelectElement("profiles").Descendants("profile").First();
            
            foreach(var dtoAssemblyName in profile.Descendants("dtoAssemblies").Descendants("add").Select(n => n.Attribute("assembly").Value).ToArray())
            {
                var assembly = Assembly.Load(dtoAssemblyName);
                wcfConfig.Dtos.Add(new DtoAssembly
                                       {
                                           Assembly = assembly, 
                                           AssemblyXML = LoadXml(assembly, smdPatchPath)
                                       });
            }

            var routeNodes = profile.XPathSelectElement("routes").XPathSelectElements("add").ToList();
            wcfConfig.Routes = new List<UrlMapElement>();
            foreach (var item in routeNodes)
            {
                UrlMapElement map = new UrlMapElement()
                {
                    Endpoint = item.Attribute("endpoint").Value /*+ item.Attribute("pathInfo").Value*/,
                    Name = item.Attribute("name").Value,
                    Type = item.Attribute("type").Value
                };
                wcfConfig.Routes.Add(map);
            }

            return wcfConfig;
        }

        private static XDocument LoadXml(Assembly assembly, string patchPath)
        {
            var fileName = Path.GetFileNameWithoutExtension(assembly.CodeBase) + ".xml";
            var filePath = Path.Combine(Path.GetDirectoryName(assembly.CodeBase), fileName);

            var doc = XDocument.Load(filePath);
            doc.Patch(patchPath);
            return doc;
        }
    }

    public class DtoAssembly
    {
        public Assembly Assembly { get; set; }
        public XDocument AssemblyXML { get; set; }
    }
}
