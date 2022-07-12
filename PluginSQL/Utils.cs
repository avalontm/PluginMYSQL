using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace PluginSQL
{
    public static class Utils
    {
        // Html encode/decode
        public static string HtmDecode(this string htmlEncodedString)
        {
            if (htmlEncodedString.Length > 0)
            {
                return System.Net.WebUtility.HtmlDecode(htmlEncodedString);
            }
            else
            {
                return htmlEncodedString;
            }
        }

        public static string HtmEncode(this string htmlDecodedString)
        {
            if (htmlDecodedString.Length > 0)
            {
                return System.Net.WebUtility.HtmlEncode(htmlDecodedString);
            }
            else
            {
                return htmlDecodedString;
            }
        }

        public static string MySQLEscape(this string str)
        {
            return Regex.Replace(str, @"[\x00'""\b\n\r\t\cZ\\%]",
                delegate (Match match)
                {
                    string v = match.Value;
                    switch (v)
                    {
                        case "\x00":            // ASCII NUL (0x00) character
                    return "\\0";
                        case "\b":              // BACKSPACE character
                    return "\\b";
                        case "\n":              // NEWLINE (linefeed) character
                    return "\\n";
                        case "\r":              // CARRIAGE RETURN character
                    return "\\r";
                        case "\t":              // TAB
                    return "\\t";
                        case "\u001A":          // Ctrl-Z
                    return "\\Z";
                        default:
                            return "\\" + v;
                    }
                });
        }

        ///<summary>
        ///Set fields to omite.
        ///</summary>
        ///<param name="omiteFields">filed1, field2, field2</param>
        public static string GetFields<T>(string prefix, string omiteFields)
        {
            string fields = string.Empty;
     
            string[] omitefields = omiteFields.Replace(" ", "").ToLower().Split(",");
            T item = default(T);
            item = Activator.CreateInstance<T>();
            PropertyInfo[] fis = item.GetType().GetProperties(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);


            for (int i = 0; i < fis.Length; i++)
            {
                PropertyInfo fi = fis[i];
                var skype_field = fi.GetCustomAttribute<FieldOmiteAttribute>(true);

                bool isOmite = false;

                if (omitefields.Length > 0)
                {
                    isOmite = omitefields.Contains(fi.Name.ToLower());
                }

                if (!isOmite && skype_field == null)
                {
                    if (string.IsNullOrEmpty(prefix))
                    {
                        fields += $"{fi.Name.ToLower()},";
                    }
                    else
                    {
                        fields += $"{prefix}.{fi.Name.ToLower()}, ";
                    }
                }
            }

            if (!string.IsNullOrEmpty(fields))
            {
                fields =  fields.Substring(0, fields.Length - 1);
            }

            return fields;
        }

        ///<summary>
        ///Set fields to omite.
        ///</summary>
        ///<param name="omiteFields">filed1, field2, field2</param>
        public static string GetFields<T>(string omiteFields = "")
        {
            string fields = string.Empty;

            string[] omitefields = omiteFields.Replace(" ", "").ToLower().Split(",");
            T item = default(T);
            item = Activator.CreateInstance<T>();
            PropertyInfo[] fis = item.GetType().GetProperties(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);


            for (int i = 0; i < fis.Length; i++)
            {
                PropertyInfo fi = fis[i];
                var skype_field = fi.GetCustomAttribute<FieldOmiteAttribute>(true);

                bool isOmite = false;

                if (omitefields.Length > 0)
                {
                    isOmite = omitefields.Contains(fi.Name.ToLower());
                }

                if (!isOmite && skype_field == null)
                {
                    fields += $"{fi.Name.ToLower()},";
                }
            }

            if (!string.IsNullOrEmpty(fields))
            {
                fields = fields.Substring(0, fields.Length - 1);
            }

            return fields;
        }

        public static dynamic GetDynamicObject(Dictionary<string, object> properties)
        {
            return new TableObject(properties);
        }

    }
}
