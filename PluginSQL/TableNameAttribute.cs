using System;
using System.Collections.Generic;
using System.Text;

/***********************************/
/* COMPLEMENTO CREADO POR AVALONTM */
/***********************************/

namespace PluginSQL
{
    public class TableNameAttribute : Attribute
    {
        public string Name { private set; get;}

        public TableNameAttribute(string name = "")
        {
            Name = name;
        }
    }
}
