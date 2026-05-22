using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Npgsql;

namespace RetailSync.Helpers
{
    static class DatabaseHelper
    {
        private static string connString =
            "Host=localhost;Port=5432;Database=Final_Project_PSQL;Username=postgres;Password=nadiv17!";
        public static NpgsqlConnection GetConnection()
        {
            return new NpgsqlConnection(connString);
        }

    }
}
