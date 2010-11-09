using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web;
using System.Xml.Linq;
using MetadataProcessor;
using Newtonsoft.Json.Linq;
using TradingApi.Configuration;

namespace MetadataProcessor
{
    public class SchemaHandler : IHttpHandler
    {
        public void ProcessRequest(HttpContext context)
        {
            JObject properties = new JObject();
            JObject schema = new JObject();
            schema.Add("properties", properties);
            bool includeDemoValue = context.Request.Params["includeDemoValue"] == null ? false : Convert.ToBoolean(context.Request.Params["includeDemoValue"]);
            // read path to get profile key
            
            string profileKey = context.Request.Params["profile"]??"";
            
            // get the profile from which to reflect upon and load assembly xml build output
            var profile = TradingApiConfigurationSection.Instance.Profiles[profileKey];
            
            foreach (AssemblyReferenceElement dtoAssembly in profile.DtoAssemblies)
            {
                var assembly = Assembly.Load(dtoAssembly.Assembly);

                foreach (var type in assembly.GetTypes())
                {
                    var doc = XDocument.Load(context.Server.MapPath(Path.Combine("~/bin", Path.GetFileNameWithoutExtension(type.Assembly.ManifestModule.Name) + ".xml")));
                    var typeSchema = JsonSchemaUtilities.BuildTypeSchema(type, doc, includeDemoValue);
                    if (typeSchema != null)
                    {
                        properties.Add(type.Name, typeSchema);
                    }
                }

            }

            // TODO: set caching headers and use asp.net cache
            var schemaJSON = schema.ToString();
            context.Response.Write(schemaJSON);

        }

        public bool IsReusable
        {
            get { return true; }
        }
    }
}
