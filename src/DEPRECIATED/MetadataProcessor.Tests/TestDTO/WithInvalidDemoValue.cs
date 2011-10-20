namespace MetadataProcessor.Tests.TestDTO
{
    /// <jschema/>
    public class WithInvalidDemoValue
    {
        /// <summary>
        /// This property has an invalid demoValue
        /// </summary>
        /// <jschema
        ///   demoValue="buy"/>
        public ACustomType UserName { get; set; }
    }

    /// <jschema/>
    public class ACustomType
    {
        /// <jschema/>
        public string Direction { get; set; }
    }
}