using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Web;
using MySql.Data.MySqlClient;

namespace LabamotoLaundryShop.Data
{
    public class DapperContext
    {
        private readonly string _connectionString;

        public DapperContext()
        {
            _connectionString = ConfigurationManager.ConnectionStrings["MySqlConnection"].ConnectionString;
        }

        public IDbConnection CreateConnection() => new MySqlConnection(_connectionString);
    }
}