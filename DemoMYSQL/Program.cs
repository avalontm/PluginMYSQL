using DemoCONSOLE.DataBase.Tables;
using DemoMYSQL.DataBase.Tables;
using PluginSQL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace DemoMYSQL
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Title = "CoreNET - MYSQL";

            Console.CancelKeyPress += (s, ev) =>
            {
                LOG.WriteLine("Salir del servidor.");
            };

            Thread mainThread = new Thread(new ThreadStart(MainThread));
            mainThread.Start();
        }

        static void MainThread()
        {
            LOG.WriteLine($"[MYSQL] conectando con MYSQL...", ConsoleColor.Yellow);

            //Iniciamos la Conección a MYSQL
            MYSQL.Init("127.0.0.1", 3306, "root", "", "demo_core");

            bool isCreate = MYSQL.CreateDataBase();

            if (isCreate)
            {
                //Creamos las tablas
                MYSQL.CreateTable<Cuenta>();
                MYSQL.CreateTable<Roles>();
                MYSQL.CreateTable<Usuario>();

                //Si no hay tablas la creamos
                if (MYSQL.Table<Cuenta>().Count == 0)
                {
                    Cuenta cuenta = new Cuenta();
                    cuenta.date_created = DateTime.Now;
                    cuenta.date_updated = DateTime.Now;
                    cuenta.rol_id = 2;
                    cuenta.account = "admin";
                    cuenta.password = "admin";
                    cuenta.Insert();

                    cuenta = new Cuenta();
                    cuenta.date_created = DateTime.Now;
                    cuenta.date_updated = DateTime.Now;
                    cuenta.rol_id = 1;
                    cuenta.account = "user";
                    cuenta.password = "user";
                    cuenta.Insert();

                    cuenta = new Cuenta();
                    cuenta.date_created = DateTime.Now;
                    cuenta.date_updated = DateTime.Now;
                    cuenta.rol_id = 1;
                    cuenta.account = "user2";
                    cuenta.password = "user2";
                    cuenta.Insert();
                }

                if (MYSQL.Table<Roles>().Count == 0)
                {
                    Roles rol = new Roles();
                    rol.rol_id = 1;
                    rol.name = "User";
                    rol.Insert();

                    rol = new Roles();
                    rol.rol_id = 2;
                    rol.name = "Adminstrator";
                    rol.Insert();

                }

                if (MYSQL.Table<Usuario>().Count == 0)
                {
                    Usuario user = new Usuario();
                    user.account_id = 1;
                    user.name = "AvalonTM";
                    user.Insert();

                    user = new Usuario();
                    user.account_id = 2;
                    user.name = "User_1";
                    user.Insert();

                    user = new Usuario();
                    user.account_id = 3;
                    user.name = "User_2";
                    user.Insert();
                }
            }

            bool mysql_status = MYSQL.CheckStatus();

            if (!mysql_status) //Si no se conecto.
            {
                LOG.WriteLine($"[MYSQL] No se pudo conectar a MYSQL.", ConsoleColor.Red);
                return;
            }

            //Si se ha conectado continuamos.
            LOG.WriteLine($"[MYSQL]  Conexion correcta.", ConsoleColor.Green);

            LOG.WriteLine("Iniciado. [Para detener el server preciona CTRL+C]", ConsoleColor.Green);

            onNewMethod();
           
            Console.WriteLine("");
            SpinWait.SpinUntil(() => false);
        }


        static void onNewMethod()
        {
            DB dbCuenta = new DB();

            List<Cuenta> cuentas = dbCuenta.Table("cuenta AS c")
                .Join("roles AS r", "r.rol_id", "=", "c.rol_id")
                .JoinRaw("users AS u ON u.account_id = c.id")
                .Select("c.*", "r.name AS rolname", "u.name AS name", "COUNT(c.id) AS count")
                //.Where("c.rol_id", ">", "0")
                .WhereRaw("c.rol_id = 1")
                .WhereRaw("OR c.rol_id = 2")
                .OrderBy("name", OrderBY.DESC)
                .GroupBy("c.rol_id")
                .Get<Cuenta>();

            Console.WriteLine($"\n[QUERY] ({dbCuenta.ToString()})\n");

            Console.WriteLine($"\n[RESULTS]\n");
            foreach (var cuenta in cuentas)
            {
                Console.WriteLine($"[Cuenta] ID: {cuenta.id} | NAME: {cuenta.name} | ROL: {cuenta.rolname} | COUNT: {cuenta.count}");
            }
        }

    }
}
