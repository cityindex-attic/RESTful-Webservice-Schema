using System;
using System.IO;
using System.Reflection;
using NUnit.Framework;

namespace JsonSchemaGeneration.Tests
{
    public class GeneratorTestsBase
    {
        protected string _dtoAssemblyBasePath;
        protected WcfConfigReader _wcfConfigReader = new WcfConfigReader();
        protected Generator _generator = new Generator();

        [TestFixtureSetUp]
        public void AddCustomAssemblyResolver()
        {
            //Ensure we also look for assemblies referenced in the Web.Config in the specified dtoAssemblyBasePath
            AppDomain.CurrentDomain.AssemblyResolve += CustomAssemblyResolver;
        }

        private Assembly CustomAssemblyResolver(object o, ResolveEventArgs args)
        {
            var assemblyname = args.Name.Split(',')[0];
            var assemblyFileName = Path.Combine(_dtoAssemblyBasePath, assemblyname + ".dll");
            var assemblyFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, assemblyFileName);
            var assembly = Assembly.LoadFile(assemblyFilePath);
            return assembly;
        }

        [TestFixtureTearDown]
        public void RemoveCustomAssemblyResolver()
        {
            AppDomain.CurrentDomain.AssemblyResolve -= CustomAssemblyResolver;
        }
    }
}