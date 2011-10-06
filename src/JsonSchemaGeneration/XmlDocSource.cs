using System.Collections.Generic;
using System.Reflection;
using TradingApi.Configuration;

namespace JsonSchemaGeneration
{
    public class XmlDocSource
    {
        public List<UrlMapElement> Routes;
        public List<Assembly> DtoAssemblies = new List<Assembly>();
        public string JsonSchemaPatch;
        public string StreamingJsonPatch;
        public string SMDPatchPath;
    }
}