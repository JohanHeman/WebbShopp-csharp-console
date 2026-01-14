using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Webbshop.Models;

namespace Webbshop
{
    internal class Queries
    {
        public static void ShowCategories(List<Category> categories)
        {
                Console.Clear();

                foreach (var c in categories)
                {
                    Console.WriteLine($"{c.Id}: {c.Name}");
                }
        }
    }
}
