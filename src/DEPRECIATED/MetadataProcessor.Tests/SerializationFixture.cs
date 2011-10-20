using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;
using MetadataProcessor.Tests.TestDTO;
using NUnit.Framework;

namespace MetadataProcessor.Tests
{
    [TestFixture]
    public class SerializationFixture
    {
        [Test]
        public void ErrorCodeShouldSerializeAsInt()
        {
            var s = new DataContractJsonSerializer(typeof (ErrorCode));

            var ms = new MemoryStream();

            s.WriteObject(ms, ErrorCode.Forbidden);

            ms.Position = 0;
            StreamReader reader = new StreamReader(ms);

            var actual = reader.ReadToEnd();

        }
    }
}
