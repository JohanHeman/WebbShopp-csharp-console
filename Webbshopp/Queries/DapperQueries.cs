using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore.SqlServer.Storage.Internal;
using Microsoft.Extensions.Configuration;
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

namespace Webbshop.Queries
{
    internal class DapperQueries
    {
        public static List<Category> GetCategories()
        {

            var config = new ConfigurationBuilder().AddUserSecrets<Program>().Build();
            var connString = config["MySettings:ConnectionString"];
            try
            {
                using (var connection = new SqlConnection(connString))
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
            var config = new ConfigurationBuilder().AddUserSecrets<Program>().Build();
            var connString = config["MySettings:ConnectionString"];
            try
            {
                using(var connection = new SqlConnection(connString))
                {
                    Console.WriteLine("Bookname: ");
                    string answer = Console.ReadLine();

                
                        string sql = "SELECT * FROM Products WHERE Name LIKE @Search";
                        return connection.Query<Product>(sql, new {Search = "%" + answer + "%"}).ToList();
                }
            }
            catch (DbException e)
            {
                Console.WriteLine(e.Message);
                return new List<Product>();
            }
        }
    }
}
