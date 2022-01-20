
using PluginSQL;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace DemoMYSQL.DataBase.Tables
{
    [TableName("cuenta")] //opcional
    public class Cuenta : TableBase
    {
        [PrimaryKey]
        public int id { set; get; }
        public DateTime date_created { set; get; }
        public DateTime date_updated { set; get; }
        public string account { set; get; }
        public string password { set; get; }
        public string name { set; get; }

        //METODOS PERSONALIZADOS PARA ESTA TABLA

        public static Cuenta Get(int id)
        {
            string table = Table.Name<Cuenta>();
            return MYSQL.Query<Cuenta>($"SELECT * FROM `{table}` WHERE id='{id}'").FirstOrDefault();
        }

        public static Cuenta Get(string account, string password)
        {
            string table = Table.Name<Cuenta>();
            return MYSQL.Query<Cuenta>($"SELECT * FROM `{table}` WHERE account='{account.ToLower()}' AND password='{password.MySQLEscape()}'").FirstOrDefault();
        }

        public static List<Cuenta> List()
        {
            string table = Table.Name<Cuenta>();
            return MYSQL.Query<Cuenta>($"SELECT * FROM `{table}`");
        }

        public static int Count()
        {
            return int.Parse( MYSQL.ExecuteScalar($"SELECT COUNT(*) FROM `cuentas`"));
        }
    }
}
