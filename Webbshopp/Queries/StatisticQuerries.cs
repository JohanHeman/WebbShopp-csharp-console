using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Webbshop.Connections;
using WindowDemo;

namespace Webbshop.Queries
{
    internal class StatisticQuerries
    {

        public static void ShowStatistics()
        {
            Console.Clear();

            List<string> statistics = Helpers.EnumsToLists(typeof(Enums.admingSatistics));

            var window = new Window("Statistics", 0, 2, statistics);
            window.Draw();

            ConsoleKeyInfo key = Console.ReadKey(true);

            if(int.TryParse(key.KeyChar.ToString(), out int input))
            {
                switch((Enums.admingSatistics)input)
                {
                    case Enums.admingSatistics.Most_sold_products:
                        TopFiveProducts();
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
    }
}
