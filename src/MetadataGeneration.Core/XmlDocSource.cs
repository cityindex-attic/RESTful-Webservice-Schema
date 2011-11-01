using System.Collections.Generic;
using TradingApi.Configuration;

namespace MetadataGeneration.Core
{
    public class XmlDocSource
    {
        public List<UrlMapElement> Routes;
        public List<DtoAssembly> Dtos = new List<DtoAssembly>();
    }
}