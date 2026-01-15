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
                    List<string> books = new List<string>();

                    string categoryName = getBooks.First().Category.Name;

                    foreach(var b in getBooks)
                    {
                        books.Add( b.Id + ": " + b.Name);
                    }

                    var window = new Window(categoryName, 1, 1, books);
                    window.Draw();

                    

                    if(int.TryParse(Console.ReadLine(), out int input))
                    {
                        if(getBooks.Any(p => p.Id == input))
                        {
                            InfoBook(input);
                        }
                        else
                        {
                            Console.WriteLine("No book with that id");
                        }
                    }
                    
                }
                else
                {
                    Console.WriteLine("There is no books with that category.");
                }
            }
            Console.ReadKey();
        }

        public static void InfoBook(int id)
        {
            Console.Clear();
            using(var db = new MyAppContext())
            {
                var book = db.Products.Where(b => b.Id == id).FirstOrDefault();
                
                if(book != null)
                {
                    List<string> bookWindow = new List<string> { book.Information , book.Price.ToString() + "$"}
;                   var window = new Window(book.Name, 1, 1, bookWindow);
                    window.Draw();
                }
                else
                {
                    Console.WriteLine("No book with that id found.");
                }
            }
        }
    }
}
