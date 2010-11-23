using System;
using System.ServiceModel.Web;
using System.Web;

namespace RESTWebServices.XDRSupport
{
    public class CORSModule : IHttpModule
    {
        // TODO: put these in the config element
        public const string Access_Control_Allow_Origin = "*";
        public const string Access_Control_Allow_Methods = "POST, GET, OPTIONS";
        public const string Access_Control_Allow_Headers = "X-Requested-With, Content-Type";
        public const string Access_Control_Max_Age = "1728000";


        public void Init(HttpApplication context)
        {
            context.BeginRequest += BeginRequest;
        }

        static void BeginRequest(object sender, EventArgs e)
        {
            var application = (HttpApplication)sender;

            if (application.Request.HttpMethod == "OPTIONS" && !string.IsNullOrWhiteSpace(application.Request.Headers["origin"]))
            {
                SetCORSHeaders(application.Response, application.Request);
                application.Response.End();
            }

        }

        /// <summary>
        /// I would like to put this in the front of the pipeline but the mocking is confusing. for now just have to call it from 
        /// each service method
        /// </summary>

        //
        public static void SetCORSHeaders(HttpResponse response, HttpRequest request)
        {
            // this is bs - webkit is not sending an 'origin' header - it then complains when it does not get back an Access-Control-Allow-Origin
            // so we have to add these response headers to EVERY response. what a pain in the ass.
            if (!string.IsNullOrWhiteSpace(request.Headers["origin"]))
            {
                response.AddHeader("Access-Control-Allow-Origin", Access_Control_Allow_Origin);
                response.AddHeader("Access-Control-Allow-Methods", Access_Control_Allow_Methods);
                response.AddHeader("Access-Control-Allow-Headers", Access_Control_Allow_Headers);
                response.AddHeader("Access-Control-Max-Age", Access_Control_Max_Age);
            }
        }
        public static void SetCORSHeaders(OutgoingWebResponseContext response, IncomingWebRequestContext request)
        {
            // this is bs - webkit is not sending an 'origin' header - it then complains when it does not get back an Access-Control-Allow-Origin
            // so we have to add these response headers to EVERY response. what a pain in the ass.
            if (!string.IsNullOrWhiteSpace(request.Headers["origin"]))
            {
                response.Headers.Add("Access-Control-Allow-Origin", Access_Control_Allow_Origin);
                response.Headers.Add("Access-Control-Allow-Methods", Access_Control_Allow_Methods);
                response.Headers.Add("Access-Control-Allow-Headers", Access_Control_Allow_Headers);
                response.Headers.Add("Access-Control-Max-Age", Access_Control_Max_Age);
            }
        }

        public void Dispose()
        {

        }
    }
}