using System;
using System.Collections.Generic;
using System.Text;

namespace PluginSQL
{

    public class FieldTypeAttribute : Attribute
    {
        public string Data { private set; get; }
        /// <summary>
        /// Field Custom Example:  [FieldType("VARCHAR(255)")]
        /// </summary>
        /// <param name="data"></param>
        public FieldTypeAttribute(string data = "")
        {
            Data = data;
        }
    }
}
