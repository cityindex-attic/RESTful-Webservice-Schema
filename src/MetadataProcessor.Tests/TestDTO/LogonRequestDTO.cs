using System;
using System.Runtime.Serialization;

namespace RESTWebServicesDTO.Request
{
    /// <jschema/>
    [Serializable, DataContract]
    public class LogOnRequestDTO
    {
        ///<summary>
        /// Username is case sensitive
        /// TODO: Why isn't this appearing in the documentation?
        ///</summary>
        /// <jschema
        /// minLength="6"
        /// maxLength="20"
        /// demoValue="CC735158"
        /// />
        [DataMember]
        public string UserName { get; set; }

        ///<summary>
        /// Password is case sensitive
        ///</summary>
        /// <jschema
        /// minLength="6"
        /// maxLength="20"
        /// demoValue="password"
        /// />
        [DataMember]
        public string Password { get; set; }
    }
}