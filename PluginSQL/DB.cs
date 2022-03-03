using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace PluginSQL
{
    public enum OrderBY
    {
        ASC,
        DESC
    }

    public class DBJoin
    {
        public string inner { set; get; }
        public string left { set; get; }
        public string right { set; get; }
    }

    public class DBQuery
    {
        public string table { set; get; }
        public string select { set; get; }
        public string where { set; get; }
        public DBJoin join { set; get; }
        public string groupby { set; get; }
        public string orderby { set; get; }


        public DBQuery()
        {
            join = new DBJoin();
        }

    }

    public class DB
    {
        public DBQuery query { set; get; }

        public DB()
        {
            query = new DBQuery();
        }

        /// <summary>
        /// QUERY
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"{query.select}{query.table}{query.join.inner}{query.where}{query.groupby}{query.orderby}";
        }
    }


    public static class DBExtentions
    {
        /// <summary>
        /// Select a Table
        /// </summary>
        /// <param name="db"></param>
        /// <param name="table"></param>
        /// <returns></returns>
        public static DB Table(this DB db, string table)
        {

            db.query.table = $"FROM {table} ";

            return db;
        }

        /// <summary>
        /// Select
        /// </summary>
        /// <param name="db"></param>
        /// <param name="select"></param>
        /// <returns></returns>
        public static DB Select(this DB db, params string[] selects)
        {
            string select = string.Empty;


            foreach (string _selcet in selects)
            {
                select += $"{_selcet},";
            }

            if (select.Substring(select.Length - 1, 1) == ",")
            {
                select = select.Remove(select.Length - 1, 1);
            }

            db.query.select = $"SELECT {select} ";

            return db;
        }

        /// <summary>
        /// Select Raw Expressions
        /// </summary>
        /// <param name="db"></param>
        /// <param name="select"></param>
        /// <returns></returns>
        public static DB SelectRaw(this DB db, string select = "*")
        {
            db.query.select = $"SELECT {select} ";
            return db;
        }

        /// <summary>
        /// Join To Another Table
        /// </summary>
        /// <param name="db"></param>
        /// <param name="table"></param>
        /// <param name="field"></param>
        /// <param name="expresion"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static DB Join(this DB db, string table, string field, string expresion, string value)
        {
            if (string.IsNullOrEmpty(db.query.join.inner))
            {
                db.query.join.inner = $"INNER JOIN {table} ON {field}{expresion}{value}";
            }
            else
            {
                db.query.join.inner += $"INNER JOIN {table} ON {field}{expresion}{value}";
            }

            db.query.join.inner += " ";
            return db;
        }

        /// <summary>
        /// Join To Another Table
        /// </summary>
        /// <param name="db"></param>
        /// <param name="table"></param>
        /// <param name="field"></param>
        /// <param name="expresion"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static DB JoinRaw(this DB db, string inner_join)
        {
            if (string.IsNullOrEmpty(db.query.join.inner))
            {
                db.query.join.inner = $"INNER JOIN {inner_join}";
            }
            else
            {
                db.query.join.inner += $"INNER JOIN {inner_join}";
            }

            db.query.join.inner += " ";
            return db;
        }

        /// <summary>
        /// Order By
        /// </summary>
        /// <param name="db"></param>
        /// <param name="order"></param>
        /// <param name="by"></param>
        /// <returns></returns>
        public static DB OrderBy(this DB db, string order, OrderBY by = OrderBY.ASC)
        {
            if (string.IsNullOrEmpty(db.query.orderby))
            {
                db.query.orderby = $"ORDER BY {order} {by.ToString()}";
            }
            else
            {
                db.query.orderby += $",{order} {by.ToString()},";
            }

            string[] parameters = db.query.orderby.Split(",");

            if (parameters.Length > 0)
            {
                if (db.query.orderby.Substring(db.query.orderby.Length - 1, 1) == ",")
                {
                    db.query.orderby = db.query.orderby.Remove(db.query.orderby.Length - 1, 1);
                }
            }

            db.query.orderby += " ";

            return db;
        }

        /// <summary>
        /// Conditions
        /// </summary>
        /// <param name="db"></param>
        /// <param name="field"></param>
        /// <param name="expresion"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static DB Where(this DB db, string field, string expresion, string value)
        {
            if (string.IsNullOrEmpty(db.query.where))
            {
                db.query.where = $"WHERE {field}{expresion}{value}";
            }
            else
            {
                db.query.where += $"AND {field}{expresion}{value}";
            }

            db.query.where += " ";

            return db;
        }

        /// <summary>
        /// Where Custom
        /// </summary>
        /// <param name="db"></param>
        /// <param name="where"></param>
        /// <returns></returns>
        public static DB WhereRaw(this DB db, string where)
        {
            if (string.IsNullOrEmpty(db.query.where))
            {
                db.query.where = $"WHERE {where}";
            }
            else
            {
                db.query.where += $"{where}";
            }

            db.query.where += " ";

            return db;
        }

        /// <summary>
        /// Group By
        /// </summary>
        /// <param name="db"></param>
        /// <param name="group"></param>
        /// <returns></returns>
        public static DB GroupBy(this DB db, string group)
        {
            if (string.IsNullOrEmpty(db.query.groupby))
            {
                db.query.groupby = $"GROUP BY {group}";
            }
            else
            {
                db.query.groupby += $",{group},";
            }

            string[] parameters = db.query.groupby.Split(",");

            if (parameters.Length > 0)
            {
                if (db.query.groupby.Substring(db.query.groupby.Length - 1, 1) == ",")
                {
                    db.query.groupby = db.query.groupby.Remove(db.query.groupby.Length - 1, 1);
                }
            }

            db.query.groupby += " ";
            return db;
        }

        /// <summary>
        ///  Get Arrows
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="db"></param>
        /// <returns></returns>
        public static List<T> Get<T>(this DB db)
        {
            try
            {
                return MYSQL.Query<T>(db.ToString());
            }
            catch (MySqlException ex)
            {
               throw new ArgumentOutOfRangeException("[Get]", ex.Message);
            }
        }

        /// <summary>
        /// Count
        /// </summary>
        /// <param name="db"></param>
        /// <returns></returns>
        public static int Count(this DB db)
        {
            try
            {
                return int.Parse(MYSQL.ExecuteScalar($"SELECT COUNT(*) {db.query.table}"));
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[Count] {ex}");
                return 0;
            }
        }
    }
}
