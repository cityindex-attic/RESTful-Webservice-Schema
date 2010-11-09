using System;
using System.Runtime.Serialization;

namespace RESTWebServicesDTO.Response
{
    ///<summary>
    /// Response to a CreateSessionRequest
    ///</summary>
    /// <jschema/>
    [Serializable, DataContract]
    public class CreateSessionResponseDTO
    {
        ///<summary>
        /// Your session token (treat as a random string)
        /// Session tokens are valid for a set period (7 days) from the time of their creation.
        /// The period is subject to change, and may vary depending on who you logon as.
        ///</summary>
        /// <jschema 
        ///     minLength="36" 
        ///     maxLength="100" 
        ///     demoValue="D2FF3E4D-01EA-4741-86F0-437C919B5559"/>
        [DataMember]
        public string Session { get; set; }
    }
}
