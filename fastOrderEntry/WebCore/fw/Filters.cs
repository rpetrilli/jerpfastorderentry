using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;

namespace WebCore.fw
{
    abstract public class Filters
    {
        public int page_number { get; set; }

        public string toScopeVariables()
        {
            string scope_var = "";
            PropertyInfo[] propertyInfo;
            propertyInfo = this.GetType().GetProperties();
            foreach (PropertyInfo p in propertyInfo)
            {
                scope_var += "$scope." + p.Name + "; \r\n";
            }
            return scope_var;
        }

        public abstract string toWhereConditions();


    }
}