using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace PluginSQL
{
    public class Table : ViewModelBase
    {
        public static string Name<T>()
        {
            T table = default(T);
            table = Activator.CreateInstance<T>();
            TypeInfo typeInfo = table.GetType().GetTypeInfo();
            string tbl_name = typeInfo.Name.ToLower();

            var _tableName = typeInfo.GetCustomAttribute<TableNameAttribute>(true);

            if (_tableName != null)
            {
                if (!string.IsNullOrEmpty(_tableName.Name))
                {
                    tbl_name = _tableName.Name.ToLower();
                    return tbl_name;
                }
            }

            return tbl_name.ToLower();
        }

        public static string Primary<T>()
        {
            PropertyInfo[] fis = typeof(T).GetProperties(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            int Colums = fis.Count();
            string primary_key = string.Empty;

            for (int i = 0; i < Colums; i++)
            {
                PropertyInfo fi = fis[i];
                var attribute = fi.GetCustomAttribute<PrimaryKeyAttribute>(true);
                var skype_field = fi.GetCustomAttribute<FieldOmiteAttribute>(true);

                if (skype_field == null)
                {
                    if (attribute != null)
                    {
                        primary_key = fi.Name.ToLower();
                        break;
                    }
                }
            }

            return primary_key;
        }

        public static T Find<T>(int id)
        {
            string primary = Primary<T>();
            string name = Name<T>();

            return MYSQL.Query<T>($"SELECT * FROM `{name}` WHERE `{primary}`='{id}' LIMIT 1").FirstOrDefault();
        }

        public static T Find<T>(string id)
        {
            string primary = Primary<T>();
            string name = Name<T>();

            return MYSQL.Query<T>($"SELECT * FROM `{name}` WHERE `{primary}`='{id}' LIMIT 1").FirstOrDefault();
        }


        public static List<T> GetAll<T>()
        {
            string name = Name<T>();
            return MYSQL.Query<T>($"SELECT * FROM `{name}`");
        }
    }
}
