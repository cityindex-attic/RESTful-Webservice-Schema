using System;

namespace MetadataProcessor
{
    public class ServiceRoute
    {
        public string Name { get; set; }
        public Type ServiceType { get; set; }
        public string Endpoint { get; set; }
    }
}