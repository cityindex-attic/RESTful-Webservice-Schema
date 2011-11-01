using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using System.Xml.XPath;
using TradingApi.Configuration;

namespace MetadataGeneration.Core
{
    public class WcfConfigReader
    {
        public XmlDocSource Read(string configPath)
        {
            var wcfConfig = new XmlDocSource();
            var config = XDocument.Load(configPath);
            var apiNode = config.XPathSelectElement("configuration/tradingApi");
            var profile = apiNode.XPathSelectElement("profiles").Descendants("profile").First();
            
            foreach(var dtoAssemblyName in profile.Descendants("dtoAssemblies").Descendants("add").Select(n => n.Attribute("assembly").Value).ToArray())
            {
                wcfConfig.Dtos.Add(DtoAssembly.CreateFromName(dtoAssemblyName));
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

       
    }

    public class DtoAssembly
    {
        public Assembly Assembly { get; set; }
        public XDocument AssemblyXML { get; set; }

        public static DtoAssembly CreateFromName(string dtoAssemblyName)
        {
             var assembly = Assembly.Load(dtoAssemblyName);
             var assemblyXml = LoadXml(assembly);
             return new DtoAssembly
                                       {
                                           Assembly = assembly, 
                                           AssemblyXML = assemblyXml
                                       };
        }

        private static XDocument LoadXml(Assembly assembly)
        {
            var fileName = Path.GetFileNameWithoutExtension(assembly.CodeBase) + ".xml";
            var filePath = Path.Combine(Path.GetDirectoryName(assembly.CodeBase), fileName);

            var doc = XDocument.Load(filePath);
            return doc;
        }
    }
}
