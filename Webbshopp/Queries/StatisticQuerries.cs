using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Webbshop.Connections;
using Webbshop.Models;
using WindowDemo;

namespace Webbshop.Queries
{
    internal class StatisticQuerries
    {

        public static void ShowStatistics()
        {
            Console.Clear();

            List<string> statistics = Helpers.EnumsToLists(typeof(Enums.adminSatistics));

            var window = new Window("Statistics", 0, 2, statistics);
            window.Draw();

            ConsoleKeyInfo key = Console.ReadKey(true);

            if(int.TryParse(key.KeyChar.ToString(), out int input))
            {
                switch((Enums.adminSatistics)input)
                {
                    case Enums.adminSatistics.Most_sold_products:
                        TopThirtyProducts();
                        break;
                    case Enums.adminSatistics.sold_past_hour:
                        LastHourOverview();
                        break;
                    case Enums.adminSatistics.biggest_customer_groups:
                        CustomerGroups();
                        break;

                }
            }
        }


        public static void TopThirtyProducts() // includes product with checkoutproduct and groups by productId, then selects the first product in the group, and the count of that group. Takes the top 5 of them and displays them
        {
            Console.Clear();
            using(var db = new MyAppContext())
            {
                var products = db.CheckoutProducts.GroupBy(cp => cp.Product).Select(g => new
                {
                    Product = g.Key,
                    SoldCount = g.Count()
                }).OrderByDescending(p => p.SoldCount).Take(30).ToList();

                foreach(var item in products)
                {
                    Console.WriteLine($"{item.Product.Id} {item.Product.Name} Sold amount: {item.SoldCount}");
                }

                Console.ReadKey(true);
            }
        }

        public static void LastHourOverview()
        {
            Console.Clear();
            using (var db = new MyAppContext())
            {
                DateTime oneHourAgo = DateTime.Now.AddHours(-1); // takes away one hour from the current time

                // groups by whats products from checkoutproducts and only shows the products sold the last hour
                var products = db.CheckoutProducts.Where(cp => cp.SoldAt >= oneHourAgo).GroupBy(cp => cp.Product).Select(g => new
                {
                    Product = g.Key,
                    SoldCount = g.Count()

                }).OrderByDescending(p => p.SoldCount).ToList();

                List<string> theList = new List<string>();

                foreach(var item in products)
                {
                    theList.Add($"{item.Product.Name} sold amount {item.SoldCount}");
                }

                if(theList.Count > 0)
                {
                    var window = new Window("Sold past hour", 0, 2, theList);
                    Console.Clear();

                    Console.WriteLine("These products have been sold for the past hour.");
                    window.Draw();
                    Console.ReadKey(true);
                }
                else
                {
                    Console.WriteLine("No books have been sold the past hour.");
                    Console.ReadKey(true);
                }
                                
            }
        }

        public static void CustomerGroups()
        {

            using( var db = new MyAppContext())
            {
                while(true)
                {
                    Console.Clear();
                    Console.WriteLine("What do you want to see? \n1 country\n2 city");
                    Console.WriteLine("press 'Q' to quit");

                    ConsoleKeyInfo answer = Console.ReadKey(true);
                    char answerUpper = char.ToUpper(answer.KeyChar);

                    if (answerUpper == 'Q')
                    {
                        return;
                    }

                    if(int.TryParse(answerUpper.ToString(), out int input))
                    {
                        if(input == 1)
                        {
                            Console.Clear();
                            var customerCountry = db.Countries.GroupBy(c => c.Name).Select(g => new
                            {
                                Country = g.Key,
                                SoldCount = g.SelectMany(c => c.Cities).SelectMany(c => c.Addresses).SelectMany(a => a.Checkouts).Count()
                            }).OrderByDescending(c => c.SoldCount).Take(10).ToList();

                            List<string> list = new();
                            foreach(var item in customerCountry)
                            {
                                list.Add(item.Country + ": " + item.SoldCount.ToString());
                            }


                            var window = new Window("Countries", 0, 2, list);
                            window.Draw();
                            Console.ReadKey(true);
                            return;

                        }
                        else if(input == 2)
                        {
                            Console.Clear();

                            var customerCity = db.Cities.GroupBy(c => c.Name).Select(g => new
                            {
                                City = g.Key,
                                SoldCount = g.SelectMany(c => c.Addresses).SelectMany(a => a.Checkouts).Count()
                            }).OrderByDescending(c => c.SoldCount).Take(10).ToList();


                            List<string> list = new();
                            foreach (var item in customerCity)
                            {
                                list.Add(item.City + ": " + item.SoldCount.ToString());
                            }
                            var window = new Window("Countries", 0, 2, list);
                            window.Draw();
                            Console.ReadKey(true);
                            return;
                        }

                        else
                        {
                            Console.WriteLine("Not a valid input!");
                            continue;
                        }
                    }
                }
            }
        }

        // add function for best selling category here 


        // after that add crud operations for customer

        // after that add option to sign up and login, and new table called users for erd and set  it up with efcore 
        // IMPORTANT COMMIT BEFORE ADDING THE NEW TABLE ETC 

    }
}
