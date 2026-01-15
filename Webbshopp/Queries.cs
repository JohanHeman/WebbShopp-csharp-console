using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;
using Webbshop.Models;
using WindowDemo;

namespace Webbshop
{
    internal class Queries
    {

        static int? checkoutIds;

        public static void ShowCategory(int id)
        {
            Console.Clear();
            try
            {
                using (var db = new MyAppContext())
                {
                    // using include to only load data from one category, instead of the entire category table
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
            catch(Exception e) 
            {
                Console.WriteLine("Something went wrong. " + e.Message);
            }
        }

        public static void InfoBook(int id)
        {
            Console.Clear();
            try
            {
                using(var db = new MyAppContext())
                {
                    var book = db.Products.Where(b => b.Id == id).FirstOrDefault();
                
                    if(book != null)
                    {
                        List<string> bookWindow = new List<string> { book.Information , book.Price.ToString() + "$", "C to add to cart."}
    ;                   var window = new Window(book.Name, 1, 1, bookWindow);
                        window.Draw();
                        ConsoleKeyInfo key = Console.ReadKey(true);
                        if (key.KeyChar == 'c')
                        {
                            AddToCart(db,book);
                        }
                    }
                    else
                    {
                        Console.WriteLine("No book with that id found.");
                    }
                }
            }
            catch(Exception e)
            {
                Console.WriteLine("Something went wrong. " + e.Message);
            }
        }


        public static void AddToCart(MyAppContext db, Product product)
        {

            // create a cart when user makes a new purchase 
            // add the product in the parameter to the cart 
            // look for an existing checkoutId before creating a new checkout 
            // if no checkout found, create a new checkout 


            // later make a function to show the cart 
            // in the cart also have an option to remove / go to payment 


            try
            {
                var checkout = db.Checkouts.FirstOrDefault(c => !c.IsPaid);

                if(checkout == null)
                {
                    checkout = new Checkout
                    {
                        // this function is used to check for checkouts 
                        // i realized that i need to add either a cart list for the program to keep track of what user adds to cart 
                        // or option two, create a new table for the cart and restructure the database.
                    }
                }

                
                    
                    // check if a checkout allready exists right now 
                    // if it does, add product to that checkout 
                    // else create a new checkout and add to that. 
                
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
            }


        }
    }
}
