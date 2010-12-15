using System;
using System.Runtime.Serialization;

namespace MetadataProcessor.Tests.TestDTO.Request
{
    /// <jschema/>
    [Serializable, DataContract]
    public class LogOnRequestDTO
    {
        ///<summary>
        /// Username is case sensitive
        ///</summary>
        /// <jschema
        /// minLength="6"
        /// maxLength="20"
        /// demoValue="3T999"
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