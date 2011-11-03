using System.ServiceModel;
using System.ServiceModel.Web;
using MetadataProcessor.DTOTestAssembly;

namespace TestAssembly.Service
{
    
        /// <summary>
        /// This whole service should be excluded
        /// </summary>
        /// <smd/>
        [ServiceContract]
        public interface IExcludedService
        {
            ///<summary>
            /// This service should not be documented
            ///</summary>
            ///<returns></returns>
            ///<smd/>
            [OperationContract]
            [WebInvoke(Method = "GET",
                UriTemplate = "/excluded",
                ResponseFormat = WebMessageFormat.Json,
                RequestFormat = WebMessageFormat.Json)]
            Class1 ExcludedEndpoint();
        }

}
