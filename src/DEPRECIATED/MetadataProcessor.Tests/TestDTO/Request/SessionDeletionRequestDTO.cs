using System;
using System.Runtime.Serialization;

namespace MetadataProcessor.Tests.TestDTO.Request
{
 
    [Serializable, DataContract]
    public class SessionDeletionRequestDTO
    {
        [DataMember]
        public string UserName { get; set; }


        [DataMember]
        public string Session { get; set; }
    }
}