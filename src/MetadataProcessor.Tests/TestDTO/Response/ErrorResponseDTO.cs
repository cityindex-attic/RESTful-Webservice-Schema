using System;
using System.Runtime.Serialization;

namespace MetadataProcessor.Tests.TestDTO.Response
{
    ///<summary>
    /// This is a description of ErrorResponseDTO
    ///</summary>
    /// <jschema/>
    [Serializable, DataContract]
    public class ErrorResponseDTO
    {
        ///<summary>
        /// This is a description of the ErrorMessage property
        ///</summary>
        /// <jschema required="always"/>
        [DataMember]
        public string ErrorMessage { get; set; }

        ///<summary>
        /// This is a description of the ErrorCode property
        ///</summary>
        /// <jschema underlyingType="MetadataProcessor.Tests.TestDTO.ErrorCode, MetadataProcessor.Tests" required="always"/>
        [DataMember]
        public int ErrorCode { get; set;}
    }
}
