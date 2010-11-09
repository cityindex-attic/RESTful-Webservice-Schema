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

            bool includeDemoValue = context.Request.Params["includeDemoValue"] == null ? false : Convert.ToBoolean(context.Request.Params["includeDemoValue"]);

            //// read path to get profile key
            //string profileKey = context.Request.Params["profile"] ?? "";
            //// get the profile from which to reflect upon and load assembly xml build output
            var profile = TradingApiConfigurationSection.Instance.Profiles[""/* was profileKey*/];

            string smdUrl = context.Request.Url.AbsoluteUri;
            //string smdTargetUrl = smdUrl.Substring(0, smdUrl.LastIndexOf('/') + 1) + (string.IsNullOrWhiteSpace(profileKey) ? "" : profileKey + "/");
            //string smdSchemaUrl = smdTargetUrl + "schema" + (string.IsNullOrWhiteSpace(profileKey) ? "" : "?profile=" + profileKey);

            string smdTargetUrl = smdUrl.Substring(0, smdUrl.LastIndexOf('/') + 1);
            string smdSchemaUrl = smdTargetUrl + "schema";
            string apiVersion = profile.Version;
            string smdDescription = "City Index RESTful API " + apiVersion;
            const string smdVersion = "2.0";
            
            var smdBase = JsonSchemaUtilities.BuildSMDBase(smdUrl, smdTargetUrl, smdSchemaUrl, smdDescription, apiVersion, smdVersion);

            var dtoAssemblies = profile.DtoAssemblies.Cast<AssemblyReferenceElement>().ToList();

            var mappedTypes = new List<Type>(); // just to keep track of types so we don't map twice
            foreach (UrlMapElement route in profile.Routes)
            {
               JsonSchemaUtilities.BuildServiceMapping(route, mappedTypes, smdBase, dtoAssemblies, includeDemoValue);
            }

            // TODO: set caching headers and use asp.net cache

            context.Response.Write(smdBase.ToString());
        }

        public bool IsReusable
        {
            get { return true; }
        }
    }
}
