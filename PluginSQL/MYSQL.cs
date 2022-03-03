using MySql.Data.MySqlClient;
using MySql.Data.Types;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

/***********************************/
/* COMPLEMENTO CREADO POR AVALONTM */
/***********************************/

namespace PluginSQL
{
    public static class MYSQL
    {
        public static string connectionString { private set; get; }
        public static string connectionTest  { private set; get; }
        public static string connectionNew  { private set; get; }
        public static string ErrorMessage  { private set; get; }
        static string host, user, password, database;
        static int port;

        /* Conexion normal */
        public static void Init(string _host, int _port, string _user, string _password, string _database)
        {
            host = _host;
            port = _port;
            user = _user;
            password = _password;
            database = _database;

            connectionString = $"datasource={_host};port={_port};username={_user};password={_password};database={_database};SslMode=none;Convert Zero Datetime=True";
        }

        static void ConnectNew(string host, int port, string user, string password)
        {
            connectionNew = "server=" + host + ";port=" + port + ";userid=" + user + ";password=" + password + ";" + "SslMode=none;Convert Zero Datetime=True";
        }

        public static bool CheckStatus()
        {
            MySqlConnection con = null;

            con = new MySqlConnection(connectionString);

            MySqlCommand cmd = new MySqlCommand("SELECT COUNT(*) FROM information_schema.tables", con);
  
            try
            {
                ErrorMessage = String.Empty;
                con.Open();
                int result = cmd.ExecuteNonQuery();
                con.Close();
                return true;
            }
            catch (MySqlException ex)
            {
                ErrorMessage = ex.Message;
                con.Close();
                return false;
            }
        }

        static bool DataBaseExist(string table)
        {
            ConnectNew(host, port, user, password);
            string query = $"SHOW DATABASES LIKE '{table.ToLower()}'";
           
            MySqlConnection con = new MySqlConnection(connectionNew);
            MySqlCommand cmd = new MySqlCommand(query, con);
            cmd.CommandTimeout = 60;

            try
            {
                ErrorMessage = String.Empty;
                con.Open();
                var execute = cmd.ExecuteScalar();

                if (execute == null)
                {
                    return false;
                }

                if (!string.IsNullOrEmpty(execute.ToString()))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch(Exception ex)
            {
                ErrorMessage = ex.Message;
                Console.WriteLine($"[DataBaseExist] {ex}");
                return false;
            }
            finally
            {
                con.Close();
            }
        }

        public static bool CreateDataBase()
        {

            if (string.IsNullOrEmpty(database))
            {
                return false;
            }

            if (DataBaseExist(database))
            {
                return true;
            }

            ConnectNew(host, port, user, password);
            string query = $"CREATE DATABASE `{database.ToLower()}`;";

            MySqlConnection con = new MySqlConnection(connectionNew);
            MySqlCommand cmd = new MySqlCommand(query, con);
            cmd.CommandTimeout = 60;

            try
            {
                ErrorMessage = String.Empty;
                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();
                return true;
            }
            catch(Exception ex)
            {
                ErrorMessage = ex.Message;
                return false;
            }
            finally
            {
                con.Close();
            }
        }


        static bool TableExist<T>()
        {
            T table = default(T);
            table = Activator.CreateInstance<T>();
            string nameTable = Path.GetExtension(table.GetType().ToString());

            nameTable = nameTable.Substring(1).ToLower();

            TypeInfo typeInfo = typeof(T).GetTypeInfo();
            var _tableName = typeInfo.GetCustomAttribute<TableNameAttribute>(true);

            if (_tableName != null)
            {
                if (!string.IsNullOrEmpty(_tableName.Name))
                {
                    nameTable = _tableName.Name.ToLower();
                }
            }

            string query = $"SHOW TABLE STATUS LIKE '{nameTable}'";

            MySqlConnection con = new MySqlConnection(connectionString);
            MySqlCommand cmd = new MySqlCommand(query, con);
            cmd.CommandTimeout = 60;

            int i = 0;
            try
            {
                ErrorMessage = String.Empty;
                con.Open();
                cmd.Prepare();
                var reader = cmd.ExecuteReader();
                string name = nameTable;

                while (reader.Read())
                {
                    string TableName = reader[i].ToString();
                    if (TableName.ToLower() == name.ToLower())
                    {
                        return true;
                    }
                    i++;
                }
                return false;
            }
            catch(Exception ex )
            {
                ErrorMessage = ex.Message;
                return false;
            }
            finally
            {
                con.Close();
            }
        }

        static bool ColumExist<T>(string name)
        {
            T table = default(T);
            table = Activator.CreateInstance<T>();

            string nameTable = Path.GetExtension(table.GetType().ToString());

            nameTable = nameTable.Substring(1).ToLower();

            TypeInfo typeInfo = typeof(T).GetTypeInfo();
            var _tableName = typeInfo.GetCustomAttribute<TableNameAttribute>(true);

            if (_tableName != null)
            {
                if (!string.IsNullOrEmpty(_tableName.Name))
                {
                    nameTable = _tableName.Name.ToLower();
                }
            }

            string query = $"SHOW COLUMNS FROM `{nameTable}` LIKE '{name}'";

            MySqlConnection databaseConnection = new MySqlConnection(connectionString);

            MySqlCommand cmd = new MySqlCommand(query, databaseConnection);
            cmd.CommandTimeout = 60;
            MySqlDataReader reader;
            int i = 0;
            try
            {
                ErrorMessage = String.Empty;
                databaseConnection.Open();
                reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    string TableName = reader[i].ToString();
                    if (TableName.ToLower() == name.ToLower())
                    {
                        return true;
                    }
                    i++;
                }

                return false;
            }
            catch(Exception ex)
            {
                ErrorMessage = ex.Message;
                return false;
            }
            finally
            {
                databaseConnection.Close();
            }
        }

        public static void CreateTable<T>()
        {
            bool exist = TableExist<T>();
            MySqlConnection con = new MySqlConnection(connectionString);

            T item = default(T);
            item = Activator.CreateInstance<T>();
            string nameTable = Path.GetExtension(item.GetType().ToString());
            PropertyInfo[] fis = typeof(T).GetProperties(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            string query = string.Empty;
            string _oldName = "";
            bool isPrimary = false;

            //Obtebemos el nombre de la tabla.
            nameTable = nameTable.Substring(1).ToLower();

            TypeInfo typeInfo = typeof(T).GetTypeInfo();
            var _tableName = typeInfo.GetCustomAttribute<TableNameAttribute>(true);

            if (_tableName != null)
            {
                if (!string.IsNullOrEmpty(_tableName.Name))
                {
                    nameTable = _tableName.Name.ToLower();
                }
            }
           
            if (exist)
            {
                query = $"ALTER TABLE `{nameTable}` ADD COLUMN (";
            }
            else
            {
                query = $"CREATE TABLE `{nameTable}` (";
            }

            int Colums = fis.Count();

            for (int i = 0; i < Colums; i++)
            {
                string ColumType = "";
                string ColumValue = "";
                object _value = new object();
                PropertyInfo fi = fis[i];

                var attribute = fi.GetCustomAttribute<PrimaryKeyAttribute>(true);
                var skype_field = fi.GetCustomAttribute<FieldOmiteAttribute>(true);
                var custom_field = fi.GetCustomAttribute<FieldTypeAttribute>(true);

                if (skype_field == null)
                {
                    if (attribute != null)
                    {
                        isPrimary = true;
                    }
                    else
                    {
                        isPrimary = false;
                    }

                    if (isPrimary)
                    {
                        if (!exist)
                        {
                            string _typeColum = GetColumType(fi);
                            query += $"`{fi.Name.ToLower() ?? string.Empty}` {_typeColum},";
                            Debug.WriteLine($"[PRIMARY] {query}");
                        }
                    }
                    else
                    {

                        if (custom_field != null && !string.IsNullOrEmpty(custom_field.Data))
                        {
                            ColumType = custom_field.Data.ToUpper();
                        }
                        else
                        {
                            if (fi.PropertyType == typeof(int))
                            {
                                ColumType = "INT NOT NULL";
                            }
                            if (fi.PropertyType == typeof(double))
                            {
                                ColumType = "DOUBLE NOT NULL";
                            }
                            if (fi.PropertyType == typeof(decimal))
                            {
                                ColumType = "DECIMAL(10,3) NOT NULL";
                            }
                            if (fi.PropertyType == typeof(float))
                            {
                                ColumType = "FLOAT NOT NULL";
                            }
                            if (fi.PropertyType == typeof(bool))
                            {
                                ColumType = "INT NOT NULL";
                            }
                            if (fi.PropertyType == typeof(string))
                            {
                                ColumType = "TEXT NOT NULL";
                            }
                            if (fi.PropertyType == typeof(DateTime))
                            {
                                ColumType = "DATETIME NOT NULL DEFAULT '1970-01-01 08:00:00'";
                                ColumValue = "1970-01-01 08:00:00";
                            }
                        }

                        bool columExist = ColumExist<T>(fi.Name);

                        if (!columExist)
                        {
                            query += "`" + fi.Name.ToLower() + "` " + ColumType + ",";
                        }
                        else
                        {
                            string _query1 = "";
                            string _query = "";

                            if (!string.IsNullOrEmpty(ColumValue))
                            {
                                try
                                {
                                    _query1 = $"UPDATE `{nameTable}` SET `{fi.Name.ToLower()}`='{ColumValue}' WHERE `{fi.Name.ToLower()}` IS NULL OR `{fi.Name.ToLower()}`='1970-01-01 08:00:00'";
                                    MySqlCommand pcmd = new MySqlCommand(_query1, con);
                                    con.Open();
                                    pcmd.ExecuteNonQuery();
                                    con.Close();
                                }
                                catch (MySqlException ex)
                                {
                                    Console.WriteLine("QUERY: " + _query1);
                                    Console.WriteLine("[UPDATE SET] " + ex.Message);
                                    con.Close();
                                }
                            }

                            try
                            {
                                _query = $"ALTER TABLE `{nameTable}` MODIFY COLUMN `{fi.Name.ToLower()}` {ColumType};";

                                MySqlCommand _cmd = new MySqlCommand(_query, con);
                                con.Open();
                                _cmd.ExecuteNonQuery();
                                con.Close();
                            }
                            catch (MySqlException ex)
                            {
                                Console.WriteLine("QUERY: " + _query);
                                Console.WriteLine("[MODIFY COLUMN] " + ex.Message);
                                con.Close();
                            }
                        }
                    }
                    _oldName = fi.Name.ToLower();
                }
            }
            if (exist)
            {
                query = query.Remove(query.Length - 1) + ");";
            }
            else
            {
                query += " PRIMARY KEY(id));";
            }

            Debug.WriteLine($"[QUERY] {query}");
            MySqlCommand cmd = new MySqlCommand(query, con);
            cmd.CommandTimeout = 60;

            try
            {
                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();
                return;
            }
            catch (MySqlException ex)
            {
                Debug.WriteLine("[CreateTable]: " + ex);
                con.Close();
                return;
            }
            finally
            {
                con.Close();
            }
        }

        static string GetColumType(PropertyInfo fi)
        {
            if (fi.PropertyType == typeof(int))
            {
                return "INT AUTO_INCREMENT";
            }
            if (fi.PropertyType == typeof(double))
            {
                return "DOUBLE";
            }
            if (fi.PropertyType == typeof(decimal))
            {
                return "DECIMAL";
            }
            if (fi.PropertyType == typeof(float))
            {
                return "FLOAT";
            }
            if (fi.PropertyType == typeof(bool))
            {
                return "INT AUTO_INCREMENT";
            }
            if (fi.PropertyType == typeof(string))
            {
                return "VARCHAR(255)";
            }
            if (fi.PropertyType == typeof(DateTime))
            {
                return "DATETIME";
            }

            return "INT AUTO_INCREMENT";
        }


        static List<T> GenerateList<T>(MySqlDataReader reader)
        {
            T item = default(T);
            PropertyInfo[] fis = typeof(T).GetProperties(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            List<T> items = new List<T>();

            try
            {
                while (reader.Read())
                {
                    item = default(T);
                    item = Activator.CreateInstance<T>();

                    for (int i = 0; i < fis.Count(); i++)
                    {
                        object _value = new object();
                        PropertyInfo fi = fis[i];

                        // var skype_field = fi.GetCustomAttribute<FieldOmiteAttribute>(true);

                        //if (skype_field == null)
                        //{
                        try
                        {
                            if (fi.PropertyType == typeof(int))
                            {
                                try
                                {
                                    if (reader[fi.Name] != DBNull.Value)
                                    {
                                        _value = reader.GetInt32(fi.Name);
                                    }
                                    else
                                    {
                                        _value = 0;
                                    }
                                }
                                catch
                                {
                                    _value = 0;
                                }
                            }
                            if (fi.PropertyType == typeof(double))
                            {
                                try
                                {
                                    if (reader[fi.Name] != DBNull.Value)
                                    {
                                        _value = reader.GetDouble(fi.Name);
                                    }
                                    else
                                    {
                                        _value = 0;
                                    }
                                }
                                catch
                                {
                                    _value = 0;
                                }
                            }
                            if (fi.PropertyType == typeof(decimal))
                            {
                                try
                                {
                                    if (reader[fi.Name] != DBNull.Value)
                                    {
                                        _value = reader.GetDecimal(fi.Name);
                                    }
                                    else
                                    {
                                        _value = 0;
                                    }
                                }
                                catch
                                {
                                    _value = 0;
                                }
                            }
                            if (fi.PropertyType == typeof(float))
                            {
                                try
                                {
                                    if (reader[fi.Name] != DBNull.Value)
                                    {
                                        _value = reader.GetFloat(fi.Name);
                                    }
                                    else
                                    {
                                        _value = 0;
                                    }
                                }
                                catch
                                {
                                    _value = 0;
                                }
                            }
                            if (fi.PropertyType == typeof(bool))
                            {
                                try
                                {
                                    if (reader[fi.Name] != DBNull.Value)
                                    {
                                        _value = reader.GetBoolean(fi.Name);
                                    }
                                    else
                                    {
                                        _value = false;
                                    }
                                }
                                catch
                                {
                                    _value = false;
                                }
                            }
                            if (fi.PropertyType == typeof(string))
                            {
                                try
                                {
                                    if (reader[fi.Name] != DBNull.Value)
                                    {
                                        _value = reader.GetString(fi.Name);
                                    }
                                    else
                                    {
                                        _value = "";
                                    }
                                }
                                catch
                                {
                                    _value = "";
                                }
                            }

                            if (fi.PropertyType == typeof(DateTime))
                            {
                                try
                                {
                                    if (reader[fi.Name] != DBNull.Value)
                                    {
                                        MySqlDateTime date = reader.GetMySqlDateTime(fi.Name);
                                        _value = date.Value;
                                    }
                                    else
                                    {
                                        _value = DateTime.Now;
                                    }
                                }
                                catch
                                {
                                    _value = DateTime.Now;
                                }
                            }

                            fi.SetValue(item, Convert.ChangeType(_value, fi.PropertyType), null);
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine($"[QUERY] {ex}");
                        }
                       // }
                    }
                    items.Add(item);
                }

                return items;
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
                Debug.Write(ex);
                return null;
            }
        }

        public static List<T> Table<T>()
        {
            List<T> items = null;
            T table = default(T);
            table = Activator.CreateInstance<T>();
            PropertyInfo[] fis = typeof(T).GetProperties(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            string nameTable = Path.GetExtension(table.GetType().ToString());

            //Obtebemos el nombre de la tabla.
            nameTable = nameTable.Substring(1).ToLower();

            TypeInfo typeInfo = typeof(T).GetTypeInfo();
            var _tableName = typeInfo.GetCustomAttribute<TableNameAttribute>(true);

            if (_tableName != null)
            {
                if (!string.IsNullOrEmpty(_tableName.Name))
                {
                    nameTable = _tableName.Name.ToLower();
                }
            }

            string query = $"SELECT * FROM `{nameTable}`";

            MySqlConnection databaseConnection = new MySqlConnection(connectionString);

            MySqlCommand cmd = new MySqlCommand(query, databaseConnection);
            cmd.CommandTimeout = 60;
            MySqlDataReader reader;

            try
            {
                ErrorMessage = String.Empty;
                databaseConnection.Open();
                reader = cmd.ExecuteReader();

                items = GenerateList<T>(reader);
                databaseConnection.Close();

                return items;
            }
            catch(Exception ex)
            {
                ErrorMessage = ex.Message;
                items = new List<T>();
                return items;
            }
            finally
            {
                databaseConnection.Close();
            }
        }

        public static bool Remove<T>(T item)
        {
            string nameTable = Path.GetExtension(item.GetType().ToString());
            PropertyInfo[] fis = typeof(T).GetProperties(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            string _field = string.Empty;
            string _value = string.Empty;

            //Obtebemos el nombre de la tabla.
            nameTable = nameTable.Substring(1).ToLower();

            TypeInfo typeInfo = typeof(T).GetTypeInfo();
            var _tableName = typeInfo.GetCustomAttribute<TableNameAttribute>(true);

            if (_tableName != null)
            {
                if (!string.IsNullOrEmpty(_tableName.Name))
                {
                    nameTable = _tableName.Name.ToLower();
                }
            }

            for (int i = 0; i < fis.Count(); i++)
            {
                PropertyInfo fi = fis[i];
                //Atributos
                var isPrimary = fi.GetCustomAttribute<PrimaryKeyAttribute>(true);

                if (isPrimary != null)
                {
                    _field = fi.Name.ToLower();
                    _value = fi.GetValue(item).ToString();
                    //Console.WriteLine($"[Remove] {_field}={_value}");
                    break;
                }
            }

            string query = $"DELETE FROM {nameTable} WHERE {_field}='{_value}'";

            MySqlConnection databaseConnection = new MySqlConnection(connectionString);
            MySqlCommand cmd = new MySqlCommand(query, databaseConnection);
            cmd.CommandTimeout = 60;

            try
            {
                ErrorMessage = String.Empty;
                databaseConnection.Open();
                cmd.ExecuteNonQuery();
                databaseConnection.Close();
                return true;
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
                Console.WriteLine("DeleteTable: " + ex);
                return false;
            }
        }


        public static bool Insert<T>(T table)
        {
            PropertyInfo[] fis = typeof(T).GetProperties(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            string nameTable = Path.GetExtension(table.GetType().ToString());

            //Obtebemos el nombre de la tabla.
            nameTable = nameTable.Substring(1).ToLower();

            TypeInfo typeInfo = typeof(T).GetTypeInfo();
            var _tableName = typeInfo.GetCustomAttribute<TableNameAttribute>(true);

            if (_tableName != null)
            {
                if (!string.IsNullOrEmpty(_tableName.Name))
                {
                    nameTable = _tableName.Name.ToLower();
                }

            }
            string query = $"INSERT INTO `{nameTable}` (";
            string id = "";

            MySqlConnection databaseConnection = new MySqlConnection(connectionString);

            for (int i = 0; i < fis.Count(); i++)
            {
                PropertyInfo fi = fis[i];
                //Atributos
                var isPrimary = fi.GetCustomAttribute<PrimaryKeyAttribute>(true);
                var skype_field = fi.GetCustomAttribute<FieldOmiteAttribute>(true);

                if (isPrimary != null)
                {
                    try
                    {
                        id = fi.GetValue(table)?.ToString() ?? string.Empty;
                    }
                    catch
                    {
                        id = "";
                    }
                }
                if (skype_field == null)
                {
                    query += "`" + fi.Name.ToLower() + "`,";
                }
            }

            query = query.Remove(query.Length - 1) + ") VALUES(";

            for (int i = 0; i < fis.Count(); i++)
            {
                PropertyInfo fi = fis[i];
                var skype_field = fi.GetCustomAttribute<FieldOmiteAttribute>(true);

                if (skype_field == null)
                {
                    if (fi.PropertyType == typeof(DateTime))
                    {
                        query += "'" + DateTime.Parse(fi.GetValue(table).ToString()).ToString("yyyy-MM-dd HH:mm:ss") + "',";
                    }

                    if (fi.PropertyType == typeof(bool))
                    {
                        query += "'" + Convert.ToInt32(fi.GetValue(table)) + "',";
                    }
                    if (fi.PropertyType != typeof(bool) && fi.PropertyType != typeof(DateTime))
                    {
                        string _value = (fi.GetValue(table)?.ToString() ?? string.Empty);
                        if (!string.IsNullOrEmpty(_value))
                        {
                            _value = _value.PROTECT();
                        }
                        query += "'" + _value + "',";
                    }
                }
            }

            query = query.Remove(query.Length - 1) + ")";

            MySqlCommand cmd = new MySqlCommand(query, databaseConnection);
            cmd.CommandTimeout = 60;

            try
            {
                ErrorMessage = String.Empty;
                databaseConnection.Open();
                cmd.ExecuteNonQuery();
                databaseConnection.Close();
                return true;
            }
            catch (MySqlException ex)
            {
                ErrorMessage = ex.Message;
                Debug.WriteLine("Insert: " + ex);
                return false;
            }
        }

        public static bool Update<T>(T table)
        {
            PropertyInfo[] fis = typeof(T).GetProperties(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            string nameTable = Path.GetExtension(table.GetType().ToString());

            //Obtebemos el nombre de la tabla.
            nameTable = nameTable.Substring(1).ToLower();

            TypeInfo typeInfo = typeof(T).GetTypeInfo();
            var _tableName = typeInfo.GetCustomAttribute<TableNameAttribute>(true);

            if (_tableName != null)
            {
                if (!string.IsNullOrEmpty(_tableName.Name))
                {
                    nameTable = _tableName.Name.ToLower();
                }

            }

            string query = $"UPDATE `{nameTable}` SET ";
            string primary = "0";
            string _field = "";
            bool isPrimary = false;
            MySqlConnection databaseConnection = new MySqlConnection(connectionString);

            for (int i = 0; i < fis.Count(); i++)
            {
                PropertyInfo fi = fis[i];
                //Atributos
                var attribute = fi.GetCustomAttribute<PrimaryKeyAttribute>(true);
                var skype_field = fi.GetCustomAttribute<FieldOmiteAttribute>(true);

                if (skype_field == null)
                {
                    if (attribute != null)
                    {
                        isPrimary = true;
                    }
                    else
                    {
                        isPrimary = false;
                    }

                    if (isPrimary)
                    {
                        primary = fi.GetValue(table).ToString();
                        _field = fi.Name.ToLower();
                    }
                    else
                    {
                        if (fi.PropertyType == typeof(DateTime))
                        {
                            query += "`" + fi.Name.ToLower() + "`='" + DateTime.Parse(fi.GetValue(table).ToString()).ToString("yyyy-MM-dd HH:mm:ss") + "',";
                        }
                        if (fi.PropertyType == typeof(bool))
                        {
                            query += "`" + fi.Name.ToLower() + "`='" + Convert.ToInt32(fi.GetValue(table)) + "',";
                        }
                        if (fi.PropertyType != typeof(bool) && fi.PropertyType != typeof(DateTime))
                        {
                            string _value = (fi.GetValue(table)?.ToString() ?? string.Empty);

                            query += "`" + fi.Name.ToLower() + "`='" + _value + "',";
                        }
                    }
                }
            }

            query = query.Remove(query.Length - 1) + " WHERE " + _field + "= '" + primary + "'";

            MySqlCommand cmd = new MySqlCommand(query, databaseConnection);
            cmd.CommandTimeout = 60;

            try
            {
                ErrorMessage = String.Empty;
                databaseConnection.Open();
                cmd.ExecuteNonQuery();
                databaseConnection.Close();
                return true;
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
                Console.WriteLine("[UpdateTable] " + ex);
                return false;
            }
        }

        /* FUNCIONES */

        public static List<T> Query<T>(string query)
        {
            MySqlConnection databaseConnection = new MySqlConnection(connectionString);

            try
            {
                MySqlCommand commandDatabase = new MySqlCommand(query, databaseConnection);
                commandDatabase.CommandTimeout = 60;
                MySqlDataReader reader;


                ErrorMessage = string.Empty;
                databaseConnection.Open();

                reader = commandDatabase.ExecuteReader();

                return GenerateList<T>(reader);
            }
            catch (MySqlException ex)
            {
                ErrorMessage = ex.Message;
                throw new ArgumentOutOfRangeException("[Query]", ex.Message);
            }
            finally
            {

                databaseConnection.Close();
            }
        }

        public static int Execute(string query)
        {
            MySqlConnection databaseConnection = new MySqlConnection(connectionString);
            MySqlCommand commandDatabase = new MySqlCommand(query, databaseConnection);
            commandDatabase.CommandTimeout = 60;

            try
            {
                ErrorMessage = String.Empty;
                databaseConnection.Open();
                commandDatabase.Prepare();
                int ret = commandDatabase.ExecuteNonQuery();

                return ret;
            }
            catch (MySqlException ex)
            {
                ErrorMessage = ex.Message;
                throw new ArgumentOutOfRangeException("[Execute]", ex.Message);
            }
            finally
            {

                databaseConnection.Close();
            }
        }

        public static string ExecuteScalar(string query)
        {
            MySqlConnection databaseConnection = new MySqlConnection(connectionString);
            MySqlCommand commandDatabase = new MySqlCommand(query, databaseConnection);
            commandDatabase.CommandTimeout = 60;

            try
            {
                ErrorMessage = String.Empty;
                databaseConnection.Open();
                commandDatabase.Prepare();
                string ret = commandDatabase.ExecuteScalar().ToString();
                return ret;
            }
            catch (MySqlException ex)
            {
                ErrorMessage = ex.Message;
                throw new ArgumentOutOfRangeException("[ExecuteScalar]", ex.Message);
            }
            finally
            {

                databaseConnection.Close();
            }
        }

    }


}
