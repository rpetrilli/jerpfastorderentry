using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace fastOrderEntry.Helpers
{
    public static class Utils
    {
        public static string Right(this string str, int length)
        {
            return str.Substring(str.Length - length, length);
        }

        internal static string Encoding(string value)
        {
            byte[] utf8Bytes = System.Text.Encoding.Unicode.GetBytes(value);
            string s_unicode2 = System.Text.Encoding.UTF8.GetString(utf8Bytes);
            return s_unicode2;            
        }
    }
}