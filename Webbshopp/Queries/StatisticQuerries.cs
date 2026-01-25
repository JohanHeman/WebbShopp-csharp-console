using Microsoft.EntityFrameworkCore;
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
                        TopFiveProducts();
                        break;
                    case Enums.adminSatistics.sold_past_hour:
                        LastHourOverview();
                        break;

                }
            }
        }


        public static void TopFiveProducts() // includes product with checkoutproduct and groups by productId, then selects the first product in the group, and the count of that group. Takes the top 5 of them and displays them
        {
            using(var db = new MyAppContext())
            {
                var products = db.CheckoutProducts.Include(cp => cp.Product).GroupBy(cp => cp.ProductId)
                    .Select(g => new
                    {
                        Product = g.FirstOrDefault().Product,
                        SoldCount = g.Count()
                    })
                    .OrderByDescending(p => p.SoldCount).Take(5).ToList();


                foreach(var item in products)
                {
                    Console.WriteLine($"{item.Product.Id} {item.Product.Name} Sold amount: {item.SoldCount}");
                }

                Console.ReadKey(true);
            }
        }

        public static void LastHourOverview()
        {
            using(var db = new MyAppContext())
            {
                DateTime oneHourAgo = DateTime.Now.AddHours(-1); // takes away one hour from the current time

                // groups by whats products from checkoutproducts and only shows the products sold the last hour
                var products = db.CheckoutProducts.Include(cp => cp.Product).Where(cp => cp.SoldAt >= oneHourAgo).GroupBy(cp => cp.ProductId).Select(g => new
                {
                    Product = g.FirstOrDefault().Product,
                    SoldCount = g.Count()

                }).OrderByDescending(p => p.SoldCount).ToList();

                List<string> theList = new List<string>();

                foreach(var item in products)
                {
                    theList.Add($"{item.Product.Name} sold amount {item.SoldCount}");
                }

                var window = new Window("Sold past hour", 0, 2, theList);
                Console.Clear();

                Console.WriteLine("These products have been sold for the past hour.");
                window.Draw();

                Console.ReadLine();
            }
        }

        public static void CustomerGroups()
        {
            using( var db = new MyAppContext())
            {
                
            }
        }



    }
}
