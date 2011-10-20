using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace MetadataProcessor.Tests.MetaCollector
{
    [TestFixture]
    public class MetaCollectorFixture
    {
        [Test]
        public void Test()
        {
            var type = typeof (TestDTO.JSchemaDTO);
            var package = type.GetMeta();

        }

    }
}
