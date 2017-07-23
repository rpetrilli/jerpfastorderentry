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
        List<ICondWhere> m_filtri_sql = new List<ICondWhere>();

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
        protected abstract void buildWhere();

        public string toWhereConditions()
        {

            buildWhere();
            string sql = "where 1=1 \r\n";
            foreach (ICondWhere cond  in m_filtri_sql)
            {
                if (cond.isApplicable()) {
                    sql += " and " + cond.toSQL() + "\r\n";
                }
            }

            return sql;
        }

        public void addUncontraint(string sql)
        {
            m_filtri_sql.Add(new Uncontraint(sql));
        }

        public void addStringExactValue(string field_name, string value, Boolean no_case = false)
        {
            m_filtri_sql.Add(new StringValue(field_name, value, Mode.Equals, no_case));
        }
        public void addStringStartWith(string field_name, string value, Boolean no_case = false)
        {
            m_filtri_sql.Add(new StringValue(field_name, value, Mode.StartWith, no_case));
        }

        public void addStringContains(string field_name, string value, Boolean no_case = false)
        {
            m_filtri_sql.Add(new StringValue(field_name, value, Mode.Contains, no_case));
        }

        public void addStringRange(string field_name, string from, string to, Boolean no_case = false)
        {
            m_filtri_sql.Add(new StringRange(field_name, from, to, no_case));
        }

        public void addDateRange(string field_name, DateTime? from, DateTime? to)
        {
            m_filtri_sql.Add(new DateRange(field_name, from, to));
        }

        public void addDateValue(string field_name, DateTime? value)
        {
            m_filtri_sql.Add(new DateValue(field_name, value));
        }

        public void addLongValue(string field_name, long? value)
        {
            m_filtri_sql.Add(new LongValue(field_name, value));
        }
    }
    
    public abstract class ICondWhere
    {
        abstract public string toSQL();
        abstract public Boolean isApplicable();

        public string escape(string str)
        {
            if (string.IsNullOrEmpty(str))
                return "";
            string res = str;
            res.Replace("'", "''");
            return res;
        }
    }

    public class Uncontraint : ICondWhere
    {
        string sql;
        public Uncontraint(string sql)
        {
            this.sql = sql;
        }

        public override bool isApplicable()
        {
            return true;
        }

        public override string toSQL()
        {
            return sql;
        }
    }

    public enum Mode
    {
        Equals,
        StartWith,
        Contains
    }

    public class StringValue: ICondWhere
    {
        string name;
        string value;
        Mode mode;
        Boolean no_case = false;

        public StringValue(string name, string value, Mode mode, Boolean no_case = false)
        {
            this.name = name;
            this.value = value;
            this.mode = mode;
            this.no_case = no_case;
        }


        public override bool isApplicable()
        {
            return !string.IsNullOrEmpty(value);
        }

        public override string toSQL()
        {
            string name_mod = name;
            string value_mod = value;

            if (no_case)
            {
                name_mod = "upper(" + name_mod + ")";
                if (!string.IsNullOrEmpty(value_mod))
                {
                    value_mod = value_mod.ToUpper();
                }
            }

            switch (mode)
            {
                case Mode.Equals:
                    return name_mod + "= '" +  escape(value_mod) + "'";
                case Mode.StartWith:
                    return name_mod + " like ('" + escape(value_mod + "%") + "')" ;
                case Mode.Contains:
                    return name_mod + " like ('" + escape("%" + value_mod + "%") + "')";
                default:
                    return "0=1";
            }
        }
    }

    public class StringRange : ICondWhere
    {
        string name;
        string from;
        string to;
        Boolean no_case = false;

        public StringRange(string name, string from, string to, bool no_case = false)
        {
            this.name = name;
            this.from = from;
            this.to = to;
            this.no_case = no_case;
        }

        public override bool isApplicable()
        {
            return !string.IsNullOrEmpty(from) || !string.IsNullOrEmpty(to);
        }

        public override string toSQL()
        {
            string name_mod = name;
            string from_mod = from;
            string to_mod = to;

            if (no_case)
            {
                name_mod = "upper(" + name_mod + ")";
                if (!string.IsNullOrEmpty(from_mod))
                {
                    from_mod = from_mod.ToUpper();
                }
                if (!string.IsNullOrEmpty(to_mod))
                {
                    to_mod = to_mod.ToUpper();
                }
            }

            if (!string.IsNullOrEmpty(from) && !string.IsNullOrEmpty(to))
            {
                return name_mod + " between '" + escape(from_mod) + "' and '" + escape(to_mod) + "' ";
            }
            else if (string.IsNullOrEmpty(from) && !string.IsNullOrEmpty(to))
            {
                return name_mod + " <= '" + escape(to_mod) + "' ";
            }
            else if (!string.IsNullOrEmpty(from) && string.IsNullOrEmpty(to))
            {
                return name_mod + " >= '" + escape(from_mod) + "' ";
            } else
            {
                return "0=1";
            }

        }

    }

    public class DateRange : ICondWhere
    {
        string name;
        DateTime? from;
        DateTime? to;

        public DateRange(string name, DateTime? from, DateTime? to)
        {
            this.name = name;
            this.from = from;
            this.to = to;
        }

        public override bool isApplicable()
        {
            return from != null || to != null;
        }

        public override string toSQL()
        {

            if ( from != null && to != null)
            {
                return name + " between '" + from?.ToString("yyyyMMdd") + "' and '" + to?.ToString("yyyyMMdd") + "' ";
            }
            else if (from == null && to != null)
            {
                return name + " <= '" + to?.ToString("yyyyMMdd") + "' ";
            }
            else if (from != null && to == null)
            {
                return name + " >= '" + from?.ToString("yyyyMMdd") + "' ";
            }
            else
            {
                return "0=1";
            }
        }
    }

    public class DateValue : ICondWhere
    {
        string name;
        DateTime? value;

        public DateValue(string name, DateTime? value)
        {
            this.name = name;
            this.value = value;
        }

        public override bool isApplicable()
        {
            return value != null;
        }

        public override string toSQL()
        {
            return name + " = " + value?.ToString("yyyyMMdd");
        }
    }

    public class LongValue: ICondWhere
    {
        string name;
        long? value;

        public LongValue(string name, long? value)
        {
            this.name = name;
            this.value = value;
        }

        public override bool isApplicable()
        {
            if (value == null)
                return false;

            return value > 0;
        }

        public override string toSQL()
        {
            return name + " = " + value;
        }
    }

}