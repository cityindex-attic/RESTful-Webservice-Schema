using System;
using System.IO;
using System.Reflection;
using NUnit.Framework;

namespace MetadataGeneration.Core.Tests
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
            //System.Diagnostics.Debugger.Launch();
            var assemblyname = args.Name.Split(',')[0];
            var assemblyFileName = Path.Combine(_dtoAssemblyBasePath, assemblyname + ".dll");
            var thisAssemblyPath = Assembly.GetAssembly(GetType()).CodeBase.Replace("file:///", "").Replace("/", @"\");
            var assemblyFilePath = Path.Combine(Path.GetDirectoryName(thisAssemblyPath), assemblyFileName);
            Console.WriteLine(string.Format("Loading assembly: {0}", assemblyFilePath));
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