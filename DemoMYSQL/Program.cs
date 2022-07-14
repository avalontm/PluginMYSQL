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
            MYSQL.Init("192.168.1.34", 3306, "senfu_dashboard", "O@5mtcgder21", "senfu_sys_dev");

            bool isCreate = false;

            //isCreate = MYSQL.CreateDataBase();

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
            DB db = new DB();

            var items = db.Table("lotes as l")
                  .Join("almacenws as a", "l.id", "=", "a.idlote")
                  .Join("tinas as ti", "ti.id", "=", "a.idtina")
                  .Join("productos as p", "p.id", "=", "a.producto_id")
                  .Select("ti.id, ti.nombre, ti.idarea,  p.nombre as producto, p.color AS color, '' as talla, ROUND(SUM(CASE WHEN a.transaccion='Salida' THEN (a.cantidad*-1) ELSE a.cantidad END),2) cantidad")
                  .Where("l.inprocess", "=", "1")
                  .Where("ti.activo", "=", "1")
                  .Where("ti.idarea", ">", "0")
                  .GroupBy("ti.id, l.id, a.idtalla")
                  .OrderBy("ti.orden, ti.nombre")
                  .Having("cantidad", ">", "0")
                  .Get();

            Console.WriteLine($"\n[QUERY] ({db.ToString()})\n");

            Console.WriteLine(items.ToJson());
        }

    }
}
