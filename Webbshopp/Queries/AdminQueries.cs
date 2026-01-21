using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Webbshop.Connections;
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

        public static async Task AdminCategories()
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
                        await ShowCategoryAdmin(db, input);
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

        public static async Task ShowCategoryAdmin(MyAppContext db, int id)
        {
            Console.Clear();
            try
            {
                var getBooks = await db.Products.Include(p => p.Category).Where(p => p.CategoryId == id).ToListAsync();

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
                            await AdminProduct(db, input);
                        }
                        else
                        {
                            Console.WriteLine("No book with that id");
                        }


                    }
                    else
                    {
                        Console.WriteLine("No books with that category");
                    }
                }
            }
            catch (DbException e)
            {
                Console.WriteLine("Something went wrong. " + e.Message);
                Console.WriteLine(e.StackTrace);
            }
            catch (DbUpdateException e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
            }
        }

        public static async Task AdminProduct(MyAppContext db, int id)
        {
            Console.Clear();
            try
            {
                var book = await db.Products.FirstOrDefaultAsync(b => b.Id == id);

                if(book != null)
                {
                    List<string> bookWindow = new List<string> { book.Information, book.Price.ToString() + "$" + " In stock: " + book.InStock + " 'c' ro change product information. and 'b' to go back" };
                    var window = new Window(book.Name, 1, 1, bookWindow);
                    window.Draw();
                    ConsoleKeyInfo key = Console.ReadKey(true);
                    if (key.KeyChar == 'c')
                    {
                       await ChangeProduct(db, id);
                    }
                }
                else
                {
                    Console.WriteLine("No book with that id found.");
                }


            }
            catch (DbException e)
            {
                Console.WriteLine("Something went wrong. " + e.Message);
                Console.WriteLine(e.StackTrace);
            }
            catch (DbUpdateException e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
            }

            Console.ReadKey(true);

        }

        public static async Task ChangeProduct(MyAppContext db, int id)
        {
            Console.Clear();

            try
            {
                var book = await db.Products.FirstOrDefaultAsync(b => b.Id == id);

                Console.WriteLine("What do you want to change for the book? ");

                List<string> options = Helpers.EnumsToLists(typeof(Enums.customerEnums));

                var window = new Window("Options", 2, 0, options);
                window.Draw();

                ConsoleKeyInfo key = Console.ReadKey(true);

                if(int.TryParse(key.KeyChar.ToString(), out int input))
                {
                    switch ((Enums.AdminProductEnums)input)
                    {
                        case Enums.AdminProductEnums.name:
                            //function to change product name 
                            break;
                        case Enums.AdminProductEnums.info:
                            //function to change info
                            break;
                        case Enums.AdminProductEnums.supplier:
                            //function to change supplier 
                            break;
                        case Enums.AdminProductEnums.instock:
                            //function to change in stock on product 
                            break;
                        case Enums.AdminProductEnums.price:
                            //function to change price on the item
                            break;
                        case Enums.AdminProductEnums.category:
                            // function to change the category
                            break;
                    }
                }

            }
            catch(DbUpdateException ex)
            {
                Console.WriteLine("Something went wrong");
                Console.WriteLine(ex.StackTrace);
            }

            



        }
    }
}
