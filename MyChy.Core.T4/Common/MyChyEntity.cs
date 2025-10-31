using System;
using System.Collections.Generic;
using System.Reflection.Metadata;
using System.Text;

namespace MyChy.Core.T4.Common
{
    public class MyChyEntityNamespace
    {
        public MyChyEntityNamespace()
        {
            FileName = new List<MyChyEntity>();
        }

        public string Namespace { get; set; }

        public IList<MyChyEntity> FileName { get; set; }

    }


    public class MyChyEntity
    {

        public MyChyEntity()
        {
            Attributes = new List<MyChyEntityAttributes>();
            OutAttributes = new List<MyChyEntityAttributes>();
            AttributeName = new List<string>();
            ServiceList = new List<string>();
            ScriptList = new List<string>();
            CustomAttributeList = new List<string>();
        }

        //public string Namespace { get; set; }

        public string Name { get; set; }

        public string Alias { get; set; }

        public string Description { get; set; }


        public string AuthorityCode { get; set; }

        public IList<MyChyEntityAttributes> Attributes { get; set; }


        public IList<MyChyEntityAttributes> OutAttributes { get; set; }

        public IList<string> AttributeName { get; set; }

        public IList<string> ServiceList { get; set; }

        public IList<string> ScriptList { get; set; }

        /// <summary>
        /// 类Attribute
        /// </summary>
        public IList<string> CustomAttributeList { get; set; }

        public bool IsBaseWithAllEntity { get; set; }

        /// <summary>
        /// 是否视图
        /// </summary>
        public bool IsViewEntity { get; set; }

        /// <summary>
        /// 是否使用缩微图
        /// </summary>
        public bool IsThumbnail { get; set; }

    }

    public class MyChyEntityAttributes
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public string Types0f { get; set; }

        public string EnumName { get; set; }

        public string AttributeName { get; set; }

        public string AttributeCode { get; set; }

        public string AttributeNamespace { get; set; }

        public string AttributeOne { get; set; }

        public string AttributeTwo { get; set; }


        public string AttributeThree { get; set; }
    }


}
