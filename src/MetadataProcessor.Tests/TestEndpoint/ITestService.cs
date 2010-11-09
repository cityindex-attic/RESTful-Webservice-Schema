using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using RESTWebServicesDTO.Request;
using RESTWebServicesDTO.Response;

namespace MetadataProcessor.Tests.TestEndpoint
{
    
    /// <summary>
    /// TODO: flesh this out with all the trimmings and edge cases 
    /// </summary>
    /// <smd/>
    [ServiceContract]
    public interface ITestService
    {
        ///<summary>
        /// CreateSession Method Description
        ///</summary>
        ///<param name="logOnRequest">disregarded - description from DTO Properties</param>
        ///<returns></returns>
        /// <smd/>
        [OperationContract]
        [WebInvoke(Method = "POST",
            UriTemplate = "/",
            ResponseFormat = WebMessageFormat.Json,
            RequestFormat = WebMessageFormat.Json)]
        CreateSessionResponseDTO CreateSession(LogOnRequestDTO logOnRequest);

        
        /// <summary>
        /// non GET/POST IGNORED EVEN WITH SMD 
        /// </summary>
        /// <param name="logoutRequest"></param>
        /// <returns></returns>
        /// <smd/>
        [OperationContract]
        [WebInvoke(Method = "DELETE",
            UriTemplate = "/",
            ResponseFormat = WebMessageFormat.Json,
            RequestFormat = WebMessageFormat.Json)]
        SessionDeletionResponseDTO Logout(SessionDeletionRequestDTO logoutRequest);


        
        ///<summary>
        ///</summary>
        ///<param name="userName" minLength="6" maxLength="20" demoValue="CC735158">Username is case sensitive. May be set as a service parameter or as a request header.</param>
        ///<param name="session" format="guid" minLength="36" maxLength="36" demoValue="5998CBE8-3594-4232-A57E-09EC3A4E7AA8">The session token. May be set as a service parameter or as a request header.</param>
        ///<returns></returns>
        /// <smd 
        /// method="DeleteSession"
        /// />
        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "/deleteSession?userName={userName}&session={session}", ResponseFormat = WebMessageFormat.Json)]
        SessionDeletionResponseDTO LogoutFromQueryString(string userName, string session);
    }
}
