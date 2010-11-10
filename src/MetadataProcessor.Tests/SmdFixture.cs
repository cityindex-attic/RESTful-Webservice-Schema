using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NUnit.Framework;
using TradingApi.Configuration;

namespace MetadataProcessor.Tests
{
    ///<summary>
    ///</summary>
    [TestFixture]
    public class SmdFixture
    {
        public static string GetTestTarget(string name)
        {
            return File.ReadAllText(@"..\..\test-targets\" + name + ".txt");
        }

       
    }
}