using Npgsql;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace fastOrderEntry.Helpers
{
    public static class DbUtils
    {
        internal static NpgsqlConnection GetDefaultConnection()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnectionString"].ConnectionString;
            var connection = new NpgsqlConnection(connectionString);
            return connection;
        }
    }
}