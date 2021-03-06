﻿using Npgsql;
using System.Configuration;

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