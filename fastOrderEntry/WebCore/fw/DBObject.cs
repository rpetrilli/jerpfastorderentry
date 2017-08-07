using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebCore.fw
{
    
    public abstract class DBObject
    {
        public string db_obj_ack { get; set; }
        public string db_obj_message { get; set; }

        public List<Message> controlla()
        {
            List<Message> messaggi = new List<Message>();
            return messaggi;
        }
        abstract public void select(NpgsqlConnection con);

        abstract public void insert(NpgsqlConnection con);

        abstract public void update(NpgsqlConnection con);

        abstract public void delete(NpgsqlConnection con);


    }



    public class Message
    {
        public string messaggio { get; set; }
        public int gravity { get; set; }
    }
}