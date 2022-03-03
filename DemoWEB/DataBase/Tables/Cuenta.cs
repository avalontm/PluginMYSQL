using PluginSQL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DemoWEB.DataBase.Tables
{
    public class Cuenta : TableBase
    {
        [PrimaryKey]
        public int id { set; get; }
        public DateTime date_created { set; get; }
        public DateTime date_updated { set; get; }

        [FieldType("VARCHAR(255)")]
        public string account { set; get; }
        public string password { set; get; }
        public string name { set; get; }

        public static Cuenta Get(int id)
        {
            return MYSQL.Query<Cuenta>($"SELECT * FROM cuenta WHERE id='{id}'").FirstOrDefault();
        }

        public static Cuenta Get(string account, string password)
        {
            return MYSQL.Query<Cuenta>($"SELECT * FROM cuenta WHERE account='{account.ToLower()}' AND password='{password}'").FirstOrDefault();
        }

        public static List<Cuenta> List()
        {
            return MYSQL.Query<Cuenta>($"SELECT * FROM cuenta");
        }
    }
}
