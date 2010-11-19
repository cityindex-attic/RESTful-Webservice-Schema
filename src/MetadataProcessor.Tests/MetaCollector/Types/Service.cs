using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;

namespace MetadataProcessor.Tests.MetaCollector.Types
{
    /// <summary>
    /// Flag a type as SMD with smd element
    /// </summary>
    /// <smd/>
    public class Service
    {
        /// <summary>
        /// Method description
        /// </summary>
        /// <returns>Description of return value</returns>
        /// <smd/>
        public DTO ServiceMethod()
        {
            return null;
        }

        
        ///<summary>
        /// Method description
        ///</summary>
        ///<param name="stringParam" >parameter description</param>
        /// <returns>Description of return value</returns>
        public DTO ServiceMethodWithParameters(string stringParam)
        {
            return null;
        }
    }
}
