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
            MYSQL.Init("127.0.0.1", 3306, "root", "", "cinetix_service");

            bool isCreate = false;

             MYSQL.CreateDataBase();

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

            List<Seat> seats = Seat.Get(1);
  
            foreach(Seat seat in seats)
            {
                Console.WriteLine($"seat: {seat.name}");
            }
            Console.WriteLine("");
            SpinWait.SpinUntil(() => false);
        }


        static void onNewMethod()
        {
            DB db = new DB();

            var items = db.Table("productos as p")
             .Join("almacenws as a", "a.producto_id", "=", "p.id")
             .Join("tinas as ti", "ti.id", "=", "a.idtina")
             .Join("lotes as l", "l.id", "=", "a.idlote")
             .Join("tallas as ta", "ta.id", "=", "a.idtalla")
             .Select("REPLACE(IFNULL(ta.nombre,'No'), ' ','_') as talla, COALESCE(ROUND(SUM(CASE WHEN a.transaccion='Salida' THEN (a.cantidad*-1) ELSE a.cantidad END),2),0) total")
             .Where("ti.idarea", ">", "0")
             .Where("l.inprocess", "=", "1")
             .Where("p.idtipo", "=", "6")
             .WhereRaw("SUBSTRING(l.idlote,2,1) = 'A' OR SUBSTRING(l.idlote,2,1) = 'G'")
             .GroupBy("ta.id, p.nombre, ta.nombre")
             .Having("total", ">", "0")
             .Get();

            if (items == null || items.Count == 0)
            {
                items = new List<TableObject>();
                items.Add(Utils.GetDynamicObject(new Dictionary<string, object>() {
                    { "talla", "NO" },
                    { "total", 0 }})
                );
            }

            Console.WriteLine($"[RESULTADO] {items.ToJson()}");
        }

    }
}
