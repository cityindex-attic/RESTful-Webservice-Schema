using System;
using System.IO;
using System.Reflection;
using System.Web;
using System.Xml.Linq;
using Newtonsoft.Json.Linq;
using TradingApi.Configuration;


namespace MetadataProcessor
{
    public class SchemaHandler : IHttpHandler
    {
        public void ProcessRequest(HttpContext context)
        {
            RESTWebServices.XDRSupport.CORSModule.SetCORSHeaders(context.Response, context.Request);
            bool includeDemoValue = context.Request.Params["includeDemoValue"] == null ? false : Convert.ToBoolean(context.Request.Params["includeDemoValue"]);
            string json = BuildSchema(context, includeDemoValue).ToString();
            
            if (context.Request.FilePath.ToLower().EndsWith(".js"))
            {
                json = string.Format("var ciConfig=ciConfig||{{}};ciConfig.schema={0};", json);
            }

            // TODO: set aggressive cache headers + use asp.net cache

            context.Response.Write(json);

        }

        public static JObject BuildSchema(HttpContext context,bool includeDemoValue)
        {
            var schema = new JObject();

            // TODO: profile is vestigale - rip it out of configuration leaving single profile element as root
            var profile = TradingApiConfigurationSection.Instance.Profiles[""];

            foreach (AssemblyReferenceElement dtoAssembly in profile.DtoAssemblies)
            {
                var assembly = Assembly.Load(dtoAssembly.Assembly);

                foreach (var type in assembly.GetTypes())
                {
                    var doc = XDocument.Load(context.Server.MapPath(Path.Combine("~/bin", Path.GetFileNameWithoutExtension(type.Assembly.ManifestModule.Name) + ".xml")));
                    var typeSchema = JsonSchemaUtilities.BuildTypeSchema(type, doc, includeDemoValue);
                    if (typeSchema != null)
                    {
                        schema.Add(type.Name, typeSchema);
                    }
                }
            }

            return schema;
        }
        public bool IsReusable
        {
            get { return true; }
        }
    }
}
