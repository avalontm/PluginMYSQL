using System;
using System.Collections.Generic;
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
            return Regex.Replace(str, @"[\x00'""\b\n\r\t\cZ\\%_]",
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
    }
}
