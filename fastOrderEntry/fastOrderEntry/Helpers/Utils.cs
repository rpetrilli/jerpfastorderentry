using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace fastOrderEntry.Helpers
{
    public static class Utils
    {
        public static string Right(this string str, int length)
        {
            return str.Substring(str.Length - length, length);
        }
    }
}