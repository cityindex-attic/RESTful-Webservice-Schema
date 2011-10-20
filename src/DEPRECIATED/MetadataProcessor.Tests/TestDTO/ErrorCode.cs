using System.Runtime.Serialization;

namespace MetadataProcessor.Tests.TestDTO
{
    ///<summary>
    /// This is a description of the ErrorCode enum
    ///</summary>
    [DataContract]
    public enum ErrorCode
    {
        /// <summary>
        /// This is a description of Forbidden
        /// </summary>
        [EnumMember]
        Forbidden = 403,
        /// <summary>
        /// This is a description of InternalServerError 
        /// </summary>
        [EnumMember]
        InternalServerError = 500,
        /// <summary>
        /// This is a description of InvalidParameterType
        /// </summary>
        [EnumMember]
        InvalidParameterType = 4000,
        /// <summary>
        /// This is a description of ParameterMissing
        /// </summary>
        [EnumMember]
        ParameterMissing = 4001,
        /// <summary>
        /// This is a description of InvalidParameterValue 
        /// </summary>
        [EnumMember]
        InvalidParameterValue = 4002,
        /// <summary>
        /// This is a description of InvalidJsonRequest 
        /// </summary>
        [EnumMember]
        InvalidJsonRequest = 4003,
        /// <summary>
        /// This is a description of InvalidJsonRequestCaseFormat 
        /// </summary>
        [EnumMember]
        InvalidJsonRequestCaseFormat = 4004
    }
}
