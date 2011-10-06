using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using System.Xml.XPath;
using TradingApi.Configuration;

namespace JsonSchemaGeneration
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
                wcfConfig.DtoAssemblies.Add(Assembly.Load(dtoAssemblyName));
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
}
