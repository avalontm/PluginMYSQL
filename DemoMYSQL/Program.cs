using DemoMYSQL.DataBase.Tables;
using PluginSQL;
using System;
using System.Threading;

namespace DemoMYSQL
{
    class Program
    {
        static bool isRunning = false;

        static void Main(string[] args)
        {
            Console.Title = "CoreNET - MYSQL";
            isRunning = true;

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

                //Si no hay tablas la creamos
                if (MYSQL.Table<Cuenta>().Count == 0)
                {
                    Cuenta cuenta = new Cuenta();
                    cuenta.date_created = DateTime.Now;
                    cuenta.date_updated = DateTime.Now;
                    cuenta.account = "admin";
                    cuenta.password = "admin";
                    cuenta.name = "avalontm";

                    if (cuenta.Insert())
                    {
                        LOG.WriteLine($"[Tabla] se ha creado la tabla de 'cuenta'.");
                    }
                }
            }

            bool mysql_status = MYSQL.CheckStatus();

            if (!mysql_status) //Si no se conecto.
            {
                LOG.WriteLine($"[MYSQL] No se pudo conectar a MYSQL.", ConsoleColor.Red);
                return;
            }


            //Si se ha conectado continuamos.
            LOG.WriteLine($"[MYSQL]  Coneccion correcta.", ConsoleColor.Green);

            LOG.WriteLine("Iniciado. [Para detener el server preciona CTRL+C]", ConsoleColor.Green);

            //Leemos los datos
            Cuenta _cuenta = Cuenta.Get(1);

            if (_cuenta != null)
            {
                _cuenta.name = "avalontm21";
                _cuenta.password = "password";

                if (_cuenta.Update())
                {
                    _cuenta = Cuenta.Get("test", "'or '1'='1"); //Volvemos a leer.\

                    if (_cuenta != null)
                    {
                        LOG.WriteLine($"[CUENTA] se ha actualizado.", ConsoleColor.DarkGray);

                        LOG.WriteLine($"[CUENTA] Nombre: {_cuenta.name}.", ConsoleColor.White);
                    }
                }

               
            }

            Console.WriteLine("");
            SpinWait.SpinUntil(() => false);
        }

    }
}
