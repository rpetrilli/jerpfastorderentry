using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Npgsql;
using WebCore.fw;

namespace fastOrderEntry.Models
{
    public class OrdineVenditaModel : DBObject
    {
        public string id_ordine { get; set; }
        public DateTime data_ordine { get; set; }

        public string id_cliente { get; set; }
         
        public string ragione_sociale { get; set; }


        public override void delete(NpgsqlConnection con)
        {
            throw new NotImplementedException();
        }

        public override int getCount(NpgsqlConnection con, Filters filters)
        {
            int count = 0;
            using (var cmd = new NpgsqlCommand())
            {
                cmd.Connection = con;
                cmd.CommandText = "SELECT \r\n" +
                    "      count(*) as cnt \r\n" +
                    "from vo_ordini ";
                cmd.ExecuteNonQuery();

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        count = Convert.ToInt32(reader["cnt"]);
                    }
                }
            }
            return count;

        }

        public override void insert(NpgsqlConnection con)
        {
            throw new NotImplementedException();
        }

        public override List<DBObject> loadPage(NpgsqlConnection con, int first, int pageSize, Filters filters)
        {
            List<DBObject> list = new List<DBObject>();
            using (var cmd = new NpgsqlCommand())
            {
                cmd.Connection = con;
                cmd.CommandText = "SELECT \r\n" +
                    "      * \r\n" +
                    "from vo_ordini \r\n" + this.getLimStr(first, pageSize);
                    
                cmd.ExecuteNonQuery();

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        OrdineVenditaModel item = new OrdineVenditaModel();
                        item.id_cliente = Convert.ToString(reader["id_cliente"]);
                        item.id_ordine = Convert.ToString(reader["id_ordine"]);
                        item.data_ordine = Convert.ToDateTime(reader["data_ordine"]);
                        list.Add(item);
                    }
                }
            }
      


            return list;
        }

        public override void select(NpgsqlConnection con)
        {
            throw new NotImplementedException();
        }

        public override void update(NpgsqlConnection con)
        {
            throw new NotImplementedException();
        }
    }
}