using System;
using System.Collections.Generic;
using System.Text;

namespace PluginSQL
{
    [AttributeUsage(AttributeTargets.Property)]
    public class FieldOmiteAttribute : Attribute
    {
        public bool isOmite { private set; get; }
        public FieldOmiteAttribute()
        {
            isOmite = true;
        }
    }
}
