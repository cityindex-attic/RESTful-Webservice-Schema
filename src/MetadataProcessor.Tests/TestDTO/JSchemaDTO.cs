using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MetadataProcessor.Tests.TestDTO
{
    ///<summary>
    /// this is a description of JSchemaDTO
    /// see http://tools.ietf.org/html/draft-zyp-json-schema-02 
    ///</summary>
    /// <jschema/>
    public class JSchemaDTO
    {

 
        /// <summary>
        /// TestEnumProperty Description
        /// </summary>
        /// <jschema 
        /// />
        public TestEnum TestEnumProperty { get; set; }


 
        /// <summary>
        /// TestEnumAsIntProperty Description
        /// </summary>
        /// <jschema underlyingType="MetadataProcessor.Tests.TestDTO.TestEnum, MetadataProcessor.Tests"/>
        public int TestEnumAsIntProperty { get; set; }


        ///<summary>
        /// DateTimeProperty description
        ///</summary>
        /// <jschema
        /// demoValue="\/Date(1290164280000)\/"
        /// format = "wcf-date"
        /// />
        public DateTime DateTimeProperty { get; set; }

        ///<summary>
        /// DateTimeOffsetProperty description
        ///</summary>
        /// <jschema
        /// demoValue="\/Date(1290164280000)\/"
        /// format = "wcf-date"
        /// />
        public DateTimeOffset DateTimeOffsetProperty { get; set; }

        ///<summary>
        /// this is a description of JSchemaDTO.IntProperty
        ///</summary>
        /// <jschema
        /// required="true"
        /// title="This provides a short description of the instance property.  The value must be a string. Full description is in xml-doc summary node"
        /// maximumCanEqual="false"
        /// minimumCanEqual="true"
        /// minimum="0" 
        /// maximum="36" 
        /// demoValue="100"
        /// />
        public int IntProperty { get; set; }

        ///<summary>
        /// this is a description of JSchemaDTO.UintProperty
        ///</summary>
        /// <jschema
        /// minimum="0" 
        /// maximum="36" 
        /// demoValue="100"
        /// />
        public uint UintProperty { get; set; }

        ///<summary>
        /// this is a description of JSchemaDTO.LongProperty
        ///</summary>
        /// <jschema
        /// minimum="0" 
        /// maximum="36" 
        /// demoValue="100"
        /// />
        public long LongProperty { get; set; }

        ///<summary>
        /// this is a description of JSchemaDTO.UlongProperty
        ///</summary>
        /// <jschema
        /// minimum="0" 
        /// maximum="36" 
        /// demoValue="100"
        /// />
        public ulong UlongProperty { get; set; }

        ///<summary>
        /// this is a description of JSchemaDTO.FloatProperty
        ///</summary>
        /// <jschema
        /// minimum="0" 
        /// maximum="36" 
        /// demoValue="100"
        /// />
        public float FloatProperty { get; set; }

        ///<summary>
        /// this is a description of JSchemaDTO.DoubleProperty
        ///</summary>
        /// <jschema
        /// minimum="0" 
        /// maximum="36" 
        /// demoValue="100"
        /// />
        public double DoubleProperty { get; set; }

        ///<summary>
        /// this is a description of JSchemaDTO.DecimalProperty
        ///</summary>
        /// <jschema
        /// minimum="0" 
        /// maximum="36" 
        /// demoValue="100"
        /// />
        public decimal DecimalProperty { get; set; }

        ///<summary>
        /// this is a description of JSchemaDTO.CharProperty
        ///</summary>
        /// <jschema
        /// demoValue="a"
        /// />
        public char CharProperty { get; set; }

        ///<summary>
        /// this is a description of JSchemaDTO.StringArrayProperty
        /// 
        /// 
        ///</summary>
        /// <jschema 
        /// demoValue="['a','b','c']"
        /// minLength="1"
        /// default="default-foo"
        /// />
        public string[] StringArrayProperty { get; set; }

        ///<summary>
        /// this is a description of JSchemaDTO.IntArrayProperty
        /// 
        /// 
        ///</summary>
        /// <jschema 
        /// demoValue="[1,2,3]"
        /// minimum="0" 
        /// maximum="36" 
        /// />
        public int[] IntArrayProperty { get; set; }

        ///<summary>
        /// this is a description of JSchemaDTO.StringProperty
        /// 
        /// 
        ///</summary>
        /// <jschema 
        /// required="true"
        /// title="This provides a short description of the instance property.  The value must be a string. Full description is in xml-doc summary node"
        /// format="guid" 
        /// contentEncoding="text/plain"
        /// minLength="36" 
        /// maxLength="36" 
        /// pattern="^(([0-9a-fA-F]){8}-([0-9a-fA-F]){4}-([0-9a-fA-F]){4}-([0-9a-fA-F]){4}-([0-9a-fA-F]){12})$" 
        /// demoValue="D2FF3E4D-01EA-4741-86F0-437C919B5559"
        /// 
        /// />
        public string StringProperty { get; set; }

        ///<summary>
        /// this is a description of JSchemaDTO.BoolProperty
        ///</summary>
        /// <jschema
        /// required="false"
        /// demoValue="true"
        /// 
        /// />
        public bool BoolProperty { get; set; }

        // TODO: insert correct json for demoValue?
        /// <summary>
        /// JSchemaDTOProperty description
        /// </summary>
        /// <jschema
        /// demoValue="{foo: &quot;bar&quot;}"
        /// />
        public JSchemaDTO JSchemaDTOProperty { get; set; }


      

        /// <summary>
        /// this is a description of JSchemaDTO.IListJSchemaDTOProperty
        /// </summary>
        /// <jschema
        /// <jschema
        /// demoValue="{foo: &quot;bar&quot;}"
        /// />
        public IList<JSchemaDTO> IListJSchemaDTOProperty { get; set; }
           

    }

    /// <summary>
    /// This is a derived class
    /// </summary>
    /// <jschema/>
    public class JSchemaDTOImpl : JSchemaDTO
    {
        /// <summary>
        /// This is a property on a derived class
        /// </summary>
        /// <jschema demoValue="demo"/>
        public string PropertyOnDerivedClass { get; set; }
    }

    /// <summary>
    /// this is a description of TestEnum
    /// </summary>
    /// <jschema demoValue="2"/>
    public enum TestEnum
    {
        /// <summary>
        /// this is a description of TestEnum.None
        /// </summary>
        None = 0,

#pragma warning disable 1591
        // no description for testing
        One = 1,
#pragma warning restore 1591
        /// <summary>
        /// this is a description of TestEnum.Two
        /// </summary>
        Two = 2
    }

}
