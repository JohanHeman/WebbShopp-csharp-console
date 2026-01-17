using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore.SqlServer.Storage.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Data.Common;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Webbshop.Models;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace Webbshop
{
    internal class DapperQueries
    {
        public static List<Category> GetCategories()
        {
            try
            {
                string connectionString = "data source=JohanPC; initial catalog=Webbshopp; Integrated Security=True; TrustServerCertificate=True;";
                using (var connection = new SqlConnection(connectionString))
                {
                    string sql = "SELECT * FROM Categories";

                    return connection.Query<Category>(sql).ToList();
                }
            }
            catch(DbException e)
            {
                Console.WriteLine("Something went wrong " + e.Message);
                return null;
            }
        }


        public static List<Product> GetBooks()
        {
            string connectionString = "data source=JohanPC; initial catalog=Webbshopp; Integrated Security=True; TrustServerCertificate=True;";
            using(var connection = new SqlConnection(connectionString))
            {
                Console.WriteLine("Bookname: ");
                string answer = Console.ReadLine();

                try
                {
                    string sql = "SELECT * FROM Products WHERE Name LIKE @Search";
                    return connection.Query<Product>(sql, new {Search = "%" + answer + "%"}).ToList();
                }
                catch (DbException e)
                {
                    Console.WriteLine(e.Message);
                    return new List<Product>();
                }
            }
        }
    }
}
