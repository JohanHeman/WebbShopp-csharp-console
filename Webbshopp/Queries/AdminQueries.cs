using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Webbshop.Models;
using WindowDemo;

namespace Webbshop.Queries
{
    internal class AdminQueries
    {


        public static void AdminCustomer()
        {
            Console.Clear();
            List<string> productList = Helpers.EnumsToLists(typeof(Enums.adminCustomerEnums));
            var window = new Window("AdminMenu", 2, 0, productList);
            window.Draw();

        }

        public static void AdminCategories()
        {
            Console.Clear();

            using (var db = new Connections.MyAppContext())
            {
                List<Category> categories = DapperQueries.GetCategories(); // gets the items from dapper querry into a list 

                Window window = Helpers.ShowCategories(categories);
                window.Draw();

                ConsoleKeyInfo key = Console.ReadKey(true);

                if(int.TryParse(key.KeyChar.ToString(), out int input))
                {
                    if (categories.Any(c => c.Id == input))
                    {
                        NavigationQueries.ShowCategory(input);
                    }
                    else
                    {
                        Console.WriteLine("No category with that Id.");
                    }
                }
                else
                {
                    Console.WriteLine("Not a valid input");
                }
            }
        }
    }
}
