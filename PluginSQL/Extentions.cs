using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PluginSQL
{
    public static class Extentions
    {
        /* *******************************************************************************************************************************************/
        /* Extenciones para el manejo de MYSQL */
        /* *******************************************************************************************************************************************/

        public static bool Insert<T>(this T item)
        {
            return MYSQL.Insert<T>(item);
        }

        public static bool Update<T>(this T item)
        {
            return MYSQL.Update<T>(item);
        }

        public static bool Delete<T>(this T item)
        {
            return MYSQL.Remove<T>(item);
        }

        /* *******************************************************************************************************************************************/
        /* Extenciones de funciones */
        /* *******************************************************************************************************************************************/

        public static string PROTECT(this string query)
        {
            if (string.IsNullOrEmpty(query))
            {
                return string.Empty;
            }
            var replacements = new Dictionary<string, string> { { "`", "" }, { "'", "" }, { " SELECT ", "" }, { " WHERE ", "" }, { " AND ", "" }, { " OR ", "" } };
            var output = replacements.Aggregate(query, (current, replacement) => current.Replace(replacement.Key, replacement.Value));
            return output;
        }

        public static string ToJson(this List<TableObject> items)
        {
            List<object> _items = new List<object>();

            foreach (var item in items)
            {
                _items.Add(item.Get);
            }
            return JsonConvert.SerializeObject(_items, Formatting.Indented);
        }
    }
}
