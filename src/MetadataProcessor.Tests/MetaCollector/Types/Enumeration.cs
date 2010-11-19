using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MetadataProcessor.Tests.MetaCollector.Types
{
    /*
     * default: default is a valid attribute but you should specify a default value (0) for your enum instead
     */

    ///<summary>
    /// Description of enum type
    ///</summary>
    /// <jschema default="0" demoValue="2"/>
    public enum Enumeration
    {
        ///<summary>
        /// Description of value None (default)
        ///</summary>
        None = 0,
        // no description
        One = 1,
        /// <summary>
        /// Description of value Two
        /// </summary>
        Two = 2
    }
}
