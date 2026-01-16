using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore.SqlServer.Storage.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Webbshop.Models;

namespace Webbshop
{
    internal class DapperQueries
    {
        public static List<Category> GetCategories()
        {
            string connectionString = "data source=JohanPC; initial catalog=Webbshopp; Integrated Security=True; TrustServerCertificate=True;";
            SqlConnection connection = new SqlConnection(connectionString);

            string sql = "SELECT * FROM Categories";

            return connection.Query<Category>(sql).ToList();
        }
    }
}
