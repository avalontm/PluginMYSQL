﻿using PluginSQL;
using System;
using System.Collections.Generic;
using System.Text;

namespace DemoCONSOLE.DataBase.Tables
{
    [TableName("users")]
    public class Usuario : TableBase
    {
        [PrimaryKey]
        public int id { set; get; }
        public int account_id { set; get; }
        public string name { set; get; }
    }
}
