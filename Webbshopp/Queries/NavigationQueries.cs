using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.Extensions.Primitives;
using Microsoft.Identity.Client;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Diagnostics;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.InteropServices.Marshalling;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using System.Xml;
using Webbshop.Connections;
using Webbshop.Migrations;
using Webbshop.Models;
using WindowDemo;

namespace Webbshop.Queries
{
    internal class NavigationQueries
    {

        public static void ShowCategory(int id, User? currentUser) // Shows the books inside one category
        {
            Console.Clear();
            try
            {
                using (var db = new MyAppContext())
                {
                    var getBooks = db.Products.Include(p => p.Category).Where(p => p.CategoryId == id).ToList();

                    if (getBooks.Count > 0)
                    {
                        List<string> books = new List<string>();

                        string categoryName = getBooks.First().Category.Name;

                        foreach (var b in getBooks)
                        {
                            books.Add(b.Id + ": " + b.Name);
                        }

                        var window = new Window(categoryName, 1, 1, books);
                        window.Draw();

                        if (int.TryParse(Console.ReadLine(), out int input))
                        {
                            if (getBooks.Any(p => p.Id == input))
                            {
                                InfoBook(input, currentUser);
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
            catch(DbException e) 
            {
                Console.WriteLine("Something went wrong. " + e.Message);
                Console.WriteLine(e.StackTrace);
            }
            catch(DbUpdateException e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
            }
        }

        public static void InfoBook(int id, User? currentUser) // This function shows the information to a user when they choose a book and gives options to the customer
        {
            Console.Clear();
            try
            {
                using(var db = new MyAppContext())
                {
                    var book = db.Products.Where(b => b.Id == id).FirstOrDefault();
                
                    if(book != null)
                    {
                        List<string> bookWindow = new List<string> { book.Information , book.Price.ToString() + "$" + " In stock: " + book.InStock + " press C to add to cart."}
    ;                   var window = new Window(book.Name, 1, 1, bookWindow);
                        window.Draw();
                        Console.WriteLine("Press any key to go back");
                        ConsoleKeyInfo key = Console.ReadKey(true);
                        char inputChar = char.ToUpper(key.KeyChar);
                        if (inputChar == 'C')
                        {
                           CartQueries.AddToCart(db,book, currentUser);
                        }
                    }
                    else
                    {
                        Console.WriteLine("No book with that id found.");
                    }
                }
            }
            catch(DbException e)
            {
                Console.WriteLine("Something went wrong. " + e.Message);
                Console.WriteLine(e.StackTrace);
            }
            catch(DbUpdateException e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
            }
        }

        public static void SearchBooks() 
        {
            Console.Clear();
            List<Product> books = DapperQueries.GetBooks();
            Console.WriteLine("Searching...");
            Thread.Sleep(1000);
            Console.Clear();


            if(books != null && books.Count > 0)
            {
                foreach (var b in books)
                {
                    Console.WriteLine($"{b.Id}: {b.Name}");
                }

                Console.WriteLine("Enter id of the book you are interested in");
                if(int.TryParse(Console.ReadLine(), out int id))
                {
                    InfoBook(id, null);
                }
            }

            else
            {
                Console.WriteLine("No books found");
            }
                Console.ReadKey(true);
        }
    }
}




