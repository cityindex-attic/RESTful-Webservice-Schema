using System.Collections.Generic;
using TradingApi.Configuration;

namespace MetadataGeneration.Core
{
    public class XmlDocSource
    {
        public List<UrlMapElement> Routes;
        public AssemblyWithXmlDocs RouteAssembly { get; set; }
        public List<AssemblyWithXmlDocs> Dtos = new List<AssemblyWithXmlDocs>();
    }
}