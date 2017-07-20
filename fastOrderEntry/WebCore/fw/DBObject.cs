using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebCore.fw
{
    public abstract class DBObject
    {
        public List<Message> controlla()
        {
            List<Message> messaggi = new List<Message>();
            return messaggi;
        }

        abstract public void select(NpgsqlConnection con);

        abstract public void insert(NpgsqlConnection con);

        abstract public void update(NpgsqlConnection con);

        abstract public void delete(NpgsqlConnection con);

        abstract public List<DBObject> loadPage(NpgsqlConnection con, int first, int pageSize, Filters filters);

        abstract public int getCount(NpgsqlConnection con, Filters filters);

        protected string getLimStr(int pag_corrente, int nr_reg_x_pagina)
        {
            return " LIMIT " + nr_reg_x_pagina + " OFFSET " + pag_corrente * nr_reg_x_pagina;
        }
    }



    public class Message
    {
        public string messaggio { get; set; }
        public int gravity { get; set; }
    }
}