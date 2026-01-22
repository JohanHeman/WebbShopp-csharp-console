using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
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
                    List<string> bookWindow = new List<string> { book.Information, book.Price.ToString() + "$" + " In stock: " + book.InStock + " 'c' ro change product information. Any oyher key to go back" };
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

                List<string> options = Helpers.EnumsToLists(typeof(Enums.AdminProductEnums));

                var window = new Window("Options", 2, 0, options);
                window.Draw();
                Console.WriteLine("What do you want to change for the book? ");
                ConsoleKeyInfo key = Console.ReadKey(true);

                if(int.TryParse(key.KeyChar.ToString(), out int input))
                {
                    switch ((Enums.AdminProductEnums)input)
                    {
                        case Enums.AdminProductEnums.name:
                            await ChangeName(db, id); 
                            break;
                        case Enums.AdminProductEnums.info:
                            await ChangeInfo(db, id);
                            break;
                        case Enums.AdminProductEnums.change_supplier:
                            await ChangeSupplierBook(db, id);
                            break;
                        case Enums.AdminProductEnums.instock:
                            await InStockProduct(db, id); 
                            break;
                        case Enums.AdminProductEnums.price:
                            await ChangePriceProduct(db, id);
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


        public static async Task ChangeName(MyAppContext db, int id)
        {
            Console.Clear();
            Console.Write("Enter the new name for the book");
            string answer = Console.ReadLine();
            try
            {
                var book = await db.Products.FirstOrDefaultAsync(b => b.Id == id);
                if(answer != null)
                {
                    book.Name = answer;
                    await db.SaveChangesAsync();
                }
                else
                {
                    Console.WriteLine("Not a valid name returning to menu");
                }
            }

            catch(DbUpdateException ex)
            {
                Console.WriteLine("SOmething went wrong ");
                Console.WriteLine(ex.StackTrace);
            }
        }

        public static async Task ChangeInfo(MyAppContext db, int id)
        {
            Console.Clear();
            Console.Write("Enter what the new information should be: ");
            string answer = Console.ReadLine();

            try
            {
                var book = await db.Products.FirstOrDefaultAsync(b => b.Id == id);
                if(answer != null)
                {
                    book.Information = answer;
                    await db.SaveChangesAsync();
                }
            }
            catch (DbUpdateException ex)
            {
                Console.WriteLine("SOmething went wrong ");
                Console.WriteLine(ex.StackTrace);
            }
        }


        public static async Task ShowSuppliers()
        {
            Console.Clear();
            using(var db = new MyAppContext())
            {
                var suppliers = await db.Suppliers.ToListAsync();
                var window = GetSuppliers(db, suppliers);

                while (true)
                {
                    window.Draw();

                    if (int.TryParse(Console.ReadLine(), out int input))
                    {
                        if (suppliers.Any(s => s.Id == input))
                        {
                            Supplier supplier = await db.Suppliers.FirstOrDefaultAsync(s => s.Id == input);
                            await ChangeSupplier(db, supplier);
                        }
                        else
                        {
                            Console.WriteLine("No supplier found with that id");
                            continue;
                        }
                    }
                }
            }
        }


        public static async Task ChangeSupplier(MyAppContext db, Supplier supplier)
        {
            Console.WriteLine("What do you want to do? ");
            Console.WriteLine("'n' for updating name\n'd' for deleting supplier");



            ConsoleKeyInfo key = Console.ReadKey(true);
            switch (key.KeyChar)
            {
                case 'n':
                    await ChangeNameSupplier(db, supplier);
                    break;
                case 'd':
                    await DeleteSupplier(db, supplier);
                    break;
            }
        }


        public static async Task ChangeNameSupplier(MyAppContext db, Supplier supplier)
        {
            Console.Clear();
            try
            {
                Console.Write("Enter the name you want the supplier to have: ");
                string name = Console.ReadLine();
                if(name != null)
                {
                    supplier.Name = name;
                    await db.SaveChangesAsync();
                }
                else
                {
                    Console.WriteLine("Name cant be null. Going back to menu.");
                }
            }
            catch(DbUpdateException ex)
            {
                Console.WriteLine("Something went wrong");
                Console.WriteLine(ex.StackTrace);
            }
        }


        public static async Task DeleteSupplier(MyAppContext db, Supplier supplier)
        {
            try
            {
                db.Suppliers.Remove(supplier);
                await db.SaveChangesAsync();
                Console.WriteLine("Succesfully deleted.");
            }
            catch(DbUpdateException ex)
            {
                Console.WriteLine("Something went wrong");
                Console.WriteLine(ex.StackTrace);
            }
            Console.ReadKey(true);
        }



        public static Window GetSuppliers(MyAppContext db, List<Supplier> suppliers)
        {
            Window window;

            List<string> windowList = new();
            foreach (var item in suppliers)
            {
                windowList.Add(item.Id + " :" + item.Name.ToString());
            }

            window = new Window("Suppliers", 0, 2, windowList);
            
            return window;
        }
            
        public static async Task ChangeSupplierBook(MyAppContext db, int id)
        {
            Console.Clear();
            var suppliers = await db.Suppliers.ToListAsync();
            var window = GetSuppliers(db, suppliers);
            var book = await db.Products.FirstOrDefaultAsync(p => p.Id == id);
            while(true)
            {
                window.Draw();
                Console.Write("Choose the new supplier for the book: press any key to quit");
                if(int.TryParse(Console.ReadLine(), out int input))
                {
                    if(suppliers.Any(s => s.Id == input))
                    {
                        try
                        {
                            book.Supplier = await db.Suppliers.FirstOrDefaultAsync(s => s.Id == input);
                            await db.SaveChangesAsync();
                            Console.WriteLine("Succesfully changed the supplier");
                            Console.Clear();
                            break;
                        }
                        catch(DbUpdateException ex)
                        {
                            Console.WriteLine("Something went wrong");
                            Console.WriteLine(ex.StackTrace);
                            break;
                        }
                    }
                }
            }
        }

        public static async Task InStockProduct(MyAppContext db, int id)
        {
            var book = await db.Products.FirstOrDefaultAsync(p => p.Id == id);

            Console.WriteLine("Currently we have " + book.InStock + "in stock");

            Console.Write("Would you like to change it ? y/n");

            ConsoleKeyInfo key = Console.ReadKey(true);
            try
            {
                if (key.KeyChar == 'y')
                {
                    Console.Clear();
                    while(true)
                    {
                        Console.WriteLine("Enter the new amount ");
                        if(int.TryParse(Console.ReadLine(), out int num))
                        {
                            book.InStock = num;
                            await db.SaveChangesAsync();
                            Console.WriteLine("Updated succeesfully");
                            Console.ReadKey(true);
                            break;
                        }
                        else
                        {
                            Console.WriteLine("Not a valid number please try again.");
                            continue;
                        }
                    }
                }
                else
                {
                    Console.WriteLine("ok returning back");
                    await Task.Delay(1000);
                }

            }   
            catch(DbUpdateException ex)
            {
                Console.WriteLine("Somethjing went wrong");
                Console.WriteLine(ex.StackTrace);
            }

        }

        public static async Task ChangePriceProduct(MyAppContext db, int id)
        {
            Console.Clear();

            var book = await db.Products.FirstOrDefaultAsync(p => p.Id == id);
            Console.WriteLine("The current price is " + book.Price);

            while(true)
            {
                Console.WriteLine("What would you like to change it too? ");

                if (decimal.TryParse(Console.ReadLine(), out decimal price))
                {
                    book.Price = price;
                    await db.SaveChangesAsync();
                    Console.WriteLine("Successfully updated the price.");
                    Console.ReadKey(true);
                    break;
                }
                else
                {
                    Console.WriteLine("invalid input");
                    continue;
                }
            }



        }

    }
}
