using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Web;
using System.Xml.Linq;
using MetadataProcessor.Utilities;
using Newtonsoft.Json.Linq;
using TradingApi.Configuration;


namespace MetadataProcessor
{
    public class SmdHandler : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {

            RESTWebServices.XDRSupport.CORSModule.SetCORSHeaders(context.Response, context.Request);

            bool includeDemoValue = context.Request.Params["includeDemoValue"] == null ? false : Convert.ToBoolean(context.Request.Params["includeDemoValue"]);
            bool embedSchema = context.Request.Params["embedSchema"] == null ? false : Convert.ToBoolean(context.Request.Params["embedSchema"]);

            JObject smdBase = BuildSMD(context, includeDemoValue, embedSchema);


            string json = smdBase.ToString();

            if (context.Request.FilePath.ToLower().EndsWith(".js"))
            {
                json = string.Format("var ciConfig=ciConfig||{{}};ciConfig.smd={0};", json);
            }

            context.Response.Write(json);
        }

        public static JObject BuildSMD(HttpContext context, bool includeDemoValue, bool embedSchema)
        {
            var profile = TradingApiConfigurationSection.Instance.Profiles[""/* was profileKey*/];

            string smdUrl = context.Request.Url.AbsoluteUri;

            string smdTargetUrl = smdUrl.Substring(0, smdUrl.LastIndexOf('/') + 1);
            string smdSchemaUrl = smdTargetUrl + "schema";
            string apiVersion = profile.Version;
            string smdDescription = "City Index RESTful API " + apiVersion;
            const string smdVersion = "2.0";

            var smdBase = SmdGenerator.BuildSMDBase(smdUrl, smdTargetUrl, smdSchemaUrl, smdDescription, apiVersion, smdVersion, includeDemoValue);

            var dtoAssemblies = profile.DtoAssemblies.Cast<AssemblyReferenceElement>().ToList();

            var mappedTypes = new List<Type>(); // just to keep track of types so we don't map twice
            foreach (UrlMapElement route in profile.Routes)
            {
                SmdGenerator.BuildServiceMapping(route, mappedTypes, smdBase, dtoAssemblies, includeDemoValue);
            }

            // TODO: set caching headers and use asp.net cache

            if (embedSchema)
            {
                JObject schema = SchemaHandler.BuildSchema(context, includeDemoValue);
                smdBase["schema"] = schema;
            }
            return smdBase;
        }
        public bool IsReusable
        {
            get { return true; }
        }
    }
}
