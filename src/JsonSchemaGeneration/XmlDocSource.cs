using System.Collections.Generic;
using System.Reflection;
using TradingApi.Configuration;

namespace JsonSchemaGeneration
{
    public class XmlDocSource
    {
        public List<UrlMapElement> Routes;
        public string JsonSchemaPatch;
        public string StreamingJsonPatch;
        public string SMDPatchPath;
        public List<DtoAssembly> Dtos = new List<DtoAssembly>();
    }
}