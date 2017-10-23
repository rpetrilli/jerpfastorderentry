using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;
using Npgsql;
using System.ComponentModel.DataAnnotations.Schema;

namespace fastOrderEntry.Models
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

    [Table("sy_users", Schema = "public")]
    public class UtenteModel
    {
        [Key]
        public string user_name { get; set; }

        public string first_name { get; set; }

        public string last_name { get; set; }

        public bool login(NpgsqlConnection conn, string user_name, string user_pass)
        {
            bool logged = false;
            using (var cmd = new NpgsqlCommand())
            {
                cmd.Connection = conn;
                cmd.CommandText = "SELECT * from sy_users where user_name = @user_name and user_pass = @user_pass";
                cmd.Parameters.AddWithValue("user_name", user_name);
                cmd.Parameters.AddWithValue("user_pass", user_pass);
                cmd.ExecuteNonQuery();

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        first_name = Convert.ToString(reader["first_name"]);
                        last_name = Convert.ToString(reader["last_name"]);
                        user_name = Convert.ToString(reader["user_name"]);
                        logged = true;
                    }
                }
            }

            return logged;


        }
    }
}