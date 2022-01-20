using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;


namespace PluginSQL
{
    [AttributeUsage(AttributeTargets.Property)]
    public class PrimaryKeyAttribute : Attribute
    {
        public bool isPrimary { private set; get; }
        public PrimaryKeyAttribute()
        {
            isPrimary = true;
        }
    }
}
