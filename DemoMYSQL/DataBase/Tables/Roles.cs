using PluginSQL;
using System;
using System.Collections.Generic;
using System.Text;

namespace DemoCONSOLE.DataBase.Tables
{
    public class Roles : TableBase
    {
        [PrimaryKey]
        public int id { set; get; }
        public int rol_id { set; get; }
        public string name { set; get; }
    }
}
