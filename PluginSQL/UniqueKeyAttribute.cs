using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;


namespace PluginSQL
{
    [AttributeUsage(AttributeTargets.Property)]
    public class UniqueKeyAttribute : Attribute
    {
        public bool isUnique { private set; get; }
        public UniqueKeyAttribute()
        {
            isUnique = true;
        }
    }
}
