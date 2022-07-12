using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;

namespace PluginSQL
{
    public sealed class TableObject : DynamicObject
    {
        public readonly Dictionary<string, object> _properties;

        public Dictionary<string, object> Get
        { 
            get { return _properties; }
        }

        public TableObject()
        {
            _properties = new Dictionary<string, object>();
        }

        public TableObject(Dictionary<string, object> properties)
        {
            _properties = properties;
        }

        public void Add(string key, object value)
        {
            _properties.Add(key, value);
        }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(_properties);
        }
    }
}
