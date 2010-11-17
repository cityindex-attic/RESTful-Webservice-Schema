using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using TradingApi.Configuration;

namespace RESTWebServices.XDRSupport
{
    public class XDomainPolicyHandler : IHttpHandler
    {
        public void ProcessRequest(HttpContext context)
        {
            CORSModule.SetCORSHeaders(context.Response, context.Request);

            // TODO: check for ms format url as well 
            context.Response.Clear();
            context.Response.ContentType = "text/xml; charset=utf-8";
            var segments = context.Request.Url.PathAndQuery.Split('/');
            string profileKey = segments[segments.Length - 2];

            // get the profile from which to reflect upon and load assembly xml build output
            var profile = TradingApiConfigurationSection.Instance.Profiles[profileKey];
            var policy = profile.XDomainPolicy;

            // could build a doc programmatically but why bother? the doc type is going to give us trouble anyway

            context.Response.Write(@"<?xml version=""1.0""?>
<!DOCTYPE cross-domain-policy SYSTEM ""http://www.macromedia.com/xml/dtds/cross-domain-policy.dtd"">
<cross-domain-policy>
  <allow-access-from domain=""~AllowAccessFrom~"" />
</cross-domain-policy>".Replace("~AllowAccessFrom~", policy.AllowAccessFrom));

        }

        public bool IsReusable
        {
            get { return true; }
        }
    }
}
