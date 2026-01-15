using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Webbshop.Models;
using WindowDemo;

namespace Webbshop
{
    internal class Queries
    {
        
        public static void ShowCategory(int id)
        {
            Console.Clear();
            using(var db = new MyAppContext())
            {
                // using include to only load data from one category, instead of the entire category table
                var getBooks = db.Products.Include(p => p.Category).Where(p => p.CategoryId == id).ToList();

                if(getBooks.Count > 0)
                {
                    List<string> Books = new List<string>();

                    string name = getBooks.First().Category.Name;
                    int i = 1;

                    foreach(var b in getBooks)
                    {
                        Books.Add( i + ": " + b.Name);
                        i++;
                    }

                    var window = new Window(name, 1, 1, Books);
                    window.Draw();
                }
                else
                {
                    Console.WriteLine("There is no books with that category.");
                }
            }

            Console.ReadKey();

        }
    }
}
