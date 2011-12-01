using MetadataGeneration.Core;

namespace CIAPI.MetadataGenerator.Tests
{
    public class GeneratorTestsBase
    {
        protected string _dtoAssemblyBasePath;
        protected WcfConfigReader _wcfConfigReader = new WcfConfigReader();
        protected Generator _generator = new Generator();

       
    }
}