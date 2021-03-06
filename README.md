# PluginMYSQL

you can go read the [WIKI](https://github.com/avalontm/PluginMYSQL/wiki/MYSQL)

EXAMPLE

```

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
            LOG.WriteLine($"[MYSQL]  Conexion correcta.", ConsoleColor.Green);

            LOG.WriteLine("Iniciado. [Para detener el server preciona CTRL+C]", ConsoleColor.Green);

            //Leemos los datos
            var cuentas = Table.GetAll<Cuenta>();

            foreach (var cuenta in cuentas)
            {
                Console.WriteLine($"[Cuenta] {cuenta.account}");
            }
            
            Console.WriteLine("");
            SpinWait.SpinUntil(() => false);
        }
}
```

TABLE MODEL

```
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
    }
    
```

METODOS GENERALES

```
listado de una tabla
 List<Cuenta> cuentas = Table.GetAll<Cuenta>();
 

buscar por id
 Cuenta cuenta = Table.Find<Cuenta>(1);
 
```

METODOS EXTRAS

```
 static void onNewMethod()
        {
            DB dbCuenta = new DB();

            List<Cuenta> cuentas = dbCuenta.Table("cuenta AS c")
                .Join("roles AS r", "r.rol_id", "=", "c.rol_id")
                .Join("users AS u", "u.account_id", "=", "c.id")
                .Select("c.*, r.name AS rolname, u.name AS name, COUNT(c.id) AS count")
                .Where("c.rol_id", ">", "0")
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
```
