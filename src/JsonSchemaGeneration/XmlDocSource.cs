using System.Collections.Generic;
using System.Reflection;
using TradingApi.Configuration;

namespace JsonSchemaGeneration
{
    public class XmlDocSource
    {
        public List<UrlMapElement> Routes;
        public List<DtoAssembly> Dtos = new List<DtoAssembly>();
    }
}