using Npgsql;
using System;
using System.ComponentModel.DataAnnotations;

namespace portaleAgenti.Models
{
    public class LoginModel
    {
        [Required]
        [DataType(DataType.Text)]
        public string nomeUtente { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string password { get; set; }
        public string ReturnUrl { get; set; }
    }

    public class UtenteModel
    {
        public string ragione_sociale { get; set; }

        public string id_agente { get; set; }
        public string partita_iva { get; set; }


        public bool login(NpgsqlConnection conn, string user_name, string user_pass)
        {
            bool logged = false;
            using (var cmd = new NpgsqlCommand())
            {
                string id_agente = "0000000000" + user_name;
                id_agente = id_agente.Substring(id_agente.Length - 10);
                cmd.Connection = conn;
                cmd.CommandText = "SELECT * from va_agenti where id_agente = @id_agente and passwd = @passwd";
                cmd.Parameters.AddWithValue("id_agente", id_agente);
                cmd.Parameters.AddWithValue("passwd", user_pass);
                cmd.ExecuteNonQuery();

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        id_agente = Convert.ToString(reader["id_agente"]);
                        ragione_sociale = Convert.ToString(reader["ragione_sociale"]);
                        partita_iva = Convert.ToString(reader["partita_iva"]);
                        logged = true;
                    }
                }
            }

            return logged;


        }
    }
}