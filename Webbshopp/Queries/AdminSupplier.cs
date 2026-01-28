using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Webbshop.Connections;
using Webbshop.Models;

namespace Webbshop.Queries
{
    internal class AdminSupplier
    {
        public static async Task ShowSuppliers() 
        {
            Console.Clear();
            using (var db = new MyAppContext())
            {
                var suppliers = await db.Suppliers.ToListAsync();
                var window = Helpers.GetSuppliers(db, suppliers);

                while (true)
                {
                    Console.Clear();
                    window.Draw();
                    Console.WriteLine("Choose a supplier or press q to quit or press 'a' to add a supplier");
                    string answer = Console.ReadLine();
                    if (answer != "q")
                    {
                        if (answer == "a")
                        {
                            await AddSupplier(db);
                            Console.Clear();
                            break;
                        }

                        if (int.TryParse(answer, out int input))
                        {
                            if (suppliers.Any(s => s.Id == input))
                            {
                                Supplier supplier = await db.Suppliers.FirstOrDefaultAsync(s => s.Id == input);
                                await ChangeSupplier(db, supplier);
                                break;
                            }
                        }
                        else
                        {
                            Console.WriteLine("No supplier found with that id press 'q' to quit");
                            Console.ReadKey(true);
                            continue;
                        }
                    }

                    else
                    {
                        break;
                    }
                }
            }
        }

        public static async Task ChangeSupplier(MyAppContext db, Supplier supplier) // change supplier info either change name or delete
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

        public static async Task AddSupplier(MyAppContext db) 
        {

            try
            {
                while (true)
                {
                    Console.Clear();
                    Console.WriteLine("Enter the name of the supplier or press 'q' to quit");
                    string? input = Console.ReadLine();

                    if (input == "q")
                    {
                        Console.Clear();
                        break;
                    }

                    if (!string.IsNullOrWhiteSpace(input) && !db.Suppliers.Any(c => c.Name == input))
                    {
                        Supplier supplier = new Supplier
                        {
                            Name = input
                        };
                        await db.Suppliers.AddAsync(supplier);
                        await db.SaveChangesAsync();
                        Console.WriteLine("Succesfully added new supplier");
                        Console.ReadKey(true);
                        break;
                    }
                }
            }
            catch (DbUpdateException ex)
            {
                Console.WriteLine("Something went wrong");
                Console.WriteLine(ex.StackTrace);
            }
        }

        public static async Task ChangeNameSupplier(MyAppContext db, Supplier supplier) 
        {
            Console.Clear();
            try
            {
                Console.Write("Enter the name you want the supplier to have: ");
                string name = Console.ReadLine();
                if (name != null)
                {
                    supplier.Name = name;
                    await db.SaveChangesAsync();
                }
                else
                {
                    Console.WriteLine("Name cant be null. Going back to menu.");
                }
            }
            catch (DbUpdateException ex)
            {
                Console.WriteLine("Something went wrong");
                Console.WriteLine(ex.StackTrace);
            }
        }

        public static async Task DeleteSupplier(MyAppContext db, Supplier supplier) 
        {
            Console.Clear();
            try
            {
                var books = await db.Products
                    .Where(b => b.SupplierId == supplier.Id)
                    .ToListAsync();
               
                if (!books.Any())
                {
                    db.Suppliers.Remove(supplier);
                    await db.SaveChangesAsync();
                    Console.WriteLine("Supplier had no products and was deleted.");
                    Console.ReadKey(true);
                    return;
                }

                var suppliers = await db.Suppliers.ToListAsync();
                var window = Helpers.GetSuppliers(db, suppliers);
                while (true)
                {
                    Console.Clear();
                    window.Draw();

                    Console.WriteLine("Choose a supplier to move the products to from the removed supplier. press 'q' to quit");
                    string input = Console.ReadLine();
                    if (input == "q") break;

                    if (int.TryParse(input, out int id))
                    {
                        if (id == supplier.Id)
                        {
                            Console.WriteLine("You cant choose the supplier your about to delete.");
                            Console.ReadKey(true);
                            continue;
                        }
                        var newSupplier = await db.Suppliers.FirstOrDefaultAsync(s => s.Id == id);
                        if (newSupplier != null)
                        {
                            foreach (var item in books)
                            {
                                item.Supplier = newSupplier;
                            }
                            db.Suppliers.Remove(supplier);
                            await db.SaveChangesAsync();
                            Console.WriteLine("Succesfully deleted.");
                            Console.ReadKey(true);
                            break;
                        }
                    }
                }
            }
            catch (DbUpdateException ex)
            {
                Console.WriteLine("Something went wrong");
                Console.WriteLine(ex.StackTrace);
            }
            Console.ReadKey(true);
        }




    }
}
