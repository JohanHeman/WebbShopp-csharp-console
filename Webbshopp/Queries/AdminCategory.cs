using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Webbshop.Connections;
using Webbshop.Models;
using WindowDemo;

namespace Webbshop.Queries
{
    internal class AdminCategory
    {
        // This class contains all the product operations for the admin
        // i made them async to show that i understand how to impliment async functions in my code
        public static async Task AdminCategories() 
        {
            Console.Clear();

            using (var db = new Connections.MyAppContext())
            {
                while (true)
                {
                    List<Category> categories = DapperQueries.GetCategories(); 

                    Window window = Helpers.ShowCategories(categories);
                    Console.Clear();
                    window.Draw();
                    Console.WriteLine("Press 'd' to delete 'a' to add a category or 'q' to quit");
                    string answer = Console.ReadLine().ToUpper();

                    switch (answer)
                    {
                        case "Q":
                            Console.Clear();
                            return;
                        case "D":
                            var catToRemove = await GetCatToRemove(db);
                            if (catToRemove != null)
                            {
                                await DeleteCategory(db, catToRemove);
                                continue;
                            }
                            break;
                        case "A":
                            await AddCategory(db);
                            continue;

                        default:
                            if (int.TryParse(answer, out int input))
                            {
                                if (categories.Any(c => c.Id == input))
                                {
                                    await ShowCategoryAdmin(db, input);
                                    break;
                                }
                                else
                                {
                                    Console.WriteLine("No category with that Id.");
                                    Console.ReadKey(true);
                                    
                                }
                            }
                            else
                            {
                                Console.WriteLine("Not a valid input");
                                Console.ReadKey(true);
                                

                            }
                            Console.ReadKey(true);
                            Console.Clear();
                            break;
                    }
                }
            }
        }


        public static async Task AddCategory(MyAppContext db)
        {
            try
            {
                while (true)
                {
                    Console.Clear();
                    Console.WriteLine("Enter the name of the new category or press 'q' to quit");
                    string? input = Console.ReadLine();
                    if (input == "q")
                    {
                        Console.Clear();
                        break;
                    }

                    if (!string.IsNullOrWhiteSpace(input) && !db.Categories.Any(c => c.Name == input))
                    {
                        Category category = new Category
                        {
                            Name = input
                        };

                        await db.Categories.AddAsync(category);
                        await db.SaveChangesAsync();

                        Console.WriteLine("Succesfully added category");
                        break;
                    }
                    else
                    {
                        Console.WriteLine("Not a valid input. please try again");
                    }
                }
            }
            catch (DbUpdateException ex)
            {
                Console.WriteLine("Something went wrong");
                Console.WriteLine(ex.StackTrace);
            }
        }


        public static async Task ShowCategoryAdmin(MyAppContext db, int id) 
        {
            Console.Clear();
            try
            {
                while(true)
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
                        Console.WriteLine("Press 'q' to quit ");

                        string answer = Console.ReadLine();
                        if(answer == "q")
                        {
                            return;
                        }

                        if (int.TryParse(answer, out int input))
                        {
                            if (getBooks.Any(p => p.Id == input))
                            {
                                await AdminProduct(db, input);
                                break;
                            }
                            else
                            {
                                Console.WriteLine("No book with that id");
                                Console.ReadKey(true);
                                continue;
                            }
                        }
                        else
                        {
                            Console.WriteLine("No books with that category");
                            Console.ReadKey(true);
                            continue;
                        }
                    }
                    else
                    {
                        Console.WriteLine("There are no books in this category.");
                        Console.ReadKey(true);
                        break;
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

        public static async Task<Category?> GetCatToRemove(MyAppContext db) // this function gets the category the user wants to delete 
        {
            Category? catToDelete = null;

            while (true)
            {
                Console.Clear();
                List<Category> categories = DapperQueries.GetCategories();

                Window window = Helpers.ShowCategories(categories);
                Console.Clear();
                window.Draw();
                Console.WriteLine("Choose a category to delete press 'q' to quit");
                string? input = Console.ReadLine();

                if (input == "q")
                {
                    Console.Clear();
                    return null;

                }

                if (int.TryParse(input, out int categoryId))
                {
                    catToDelete = await db.Categories.FirstOrDefaultAsync(c => c.Id == categoryId);
                    if (catToDelete != null)
                    {
                        break;
                    }
                    else
                    {
                        Console.WriteLine("Not a valid input please try again");
                        Console.ReadKey(true);
                        continue;
                    }
                }
                else
                {
                    Console.WriteLine("Not a valid input");
                    Console.ReadKey(true);
                    continue;
                }
            }

            return catToDelete;
        }

        public static async Task DeleteCategory(MyAppContext db, Category category) // this function deletes the category that the user wants to delete
        {
            var books = await db.Products
                .Where(b => b.CategoryId == category.Id)
                .ToListAsync();
          
            if (!books.Any())
            {
                db.Categories.Remove(category);
                await db.SaveChangesAsync();
                Console.Clear();
                Console.WriteLine("Category had no products and was deleted.");
                Console.ReadKey(true);
                return;
            }

            while (true)
            {
                List<Category> categories = DapperQueries.GetCategories();

                Window window = Helpers.ShowCategories(categories);
                window.Draw();

                Console.WriteLine("Choose a category to move the products to from the removed category. press 'q' to quit");

                string input = Console.ReadLine();

                if (input == "q")
                {
                    Console.Clear();
                    break;
                }

                if (category != null)
                {
                    if (int.TryParse(input, out int categoryId))
                    {
                        if (categoryId == category.Id)
                        {
                            Console.WriteLine("You cant move items to a category you are about to delete. ");
                            Console.ReadKey(true);
                            continue;
                        }
                        var newCategory = await db.Categories.FirstOrDefaultAsync(c => c.Id == categoryId);
                        if (newCategory != null)
                        {
                            foreach (var item in books)
                            {
                                item.CategoryId = newCategory.Id;
                            }
                            try
                            {
                                db.Categories.Remove(category);
                                await db.SaveChangesAsync();
                                Console.Clear();
                                Console.WriteLine("succesfully delete category and moved books to new category.");
                                Console.ReadKey(true);
                                break;
                            }
                            catch (DbUpdateException ex)
                            {
                                Console.WriteLine("Something went wrong ");
                                Console.WriteLine(ex.StackTrace);
                            }
                        }
                        else
                        {
                            Console.WriteLine("Cant be null"); 
                        }
                    }
                }
            }
        }

        public static async Task ChangeSupplierBook(MyAppContext db, Product book) 
        {
            Console.Clear();
            var suppliers = await db.Suppliers.ToListAsync();
            var window = Helpers.GetSuppliers(db, suppliers);
            while (true)
            {
                window.Draw();
                Console.Write("Choose the new supplier for the book: press 'q' to quit");
                string? answer = Console.ReadLine();

                if (answer == "q")
                {
                    Console.Clear();
                    return;
                }

                if (int.TryParse(answer, out int input))
                {
                    if (suppliers.Any(s => s.Id == input))
                    {
                        try
                        {
                            book.Supplier = await db.Suppliers.FirstOrDefaultAsync(s => s.Id == input);
                            await db.SaveChangesAsync();
                            Console.WriteLine("Succesfully changed the supplier");
                            Console.Clear();
                            break;
                        }
                        catch (DbUpdateException ex)
                        {
                            Console.WriteLine("Something went wrong");
                            Console.WriteLine(ex.StackTrace);
                            continue; // Continue instead of break on exception to re-prompt
                        }
                    }
                    else
                    {
                        Console.WriteLine("No supplier with that ID. Please try again.");
                        Console.ReadKey(true);
                        continue;
                    }
                }
                else
                {
                    Console.WriteLine("Invalid input. Please enter a number or 'q'.");
                    Console.ReadKey(true);
                    continue;
                }
            }
        }

        public static async Task InStockProduct(MyAppContext db, Product book) 
        {
            

            Console.WriteLine("Currently we have " + book.InStock + "in stock");

            Console.Write("Would you like to change it ? y/n");

            ConsoleKeyInfo key = Console.ReadKey(true);
            try
            {
                if (key.KeyChar == 'y')
                {
                    Console.Clear();
                    while (true)
                    {
                        Console.WriteLine("Enter the new amount ");
                        if (int.TryParse(Console.ReadLine(), out int num))
                        {
                            if(num <= 0)
                            {
                                Console.WriteLine("Stock quantity cannot be less than 1. Please try again.");
                                continue;
                            }
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
            catch (DbUpdateException ex)
            {
                Console.WriteLine("Something went wrong");
                Console.WriteLine(ex.StackTrace);
            }

        }

        public static async Task ChangePriceProduct(MyAppContext db, Product book) 
        {
            Console.Clear();

            
            Console.WriteLine("The current price is " + book.Price);

            while (true)
            {
                Console.WriteLine("What would you like to change it too? (press 'q' to quit)");
                string? answer = Console.ReadLine();

                if (answer == "q")
                {
                    Console.Clear();
                    return;
                }

                if (decimal.TryParse(answer, out decimal price))
                {
                    book.Price = price;
                    await db.SaveChangesAsync();
                    Console.WriteLine("Successfully updated the price.");
                    Console.ReadKey(true);
                    break;
                }
                else
                {
                    Console.WriteLine("Invalid input. Please enter a valid decimal number or 'q'.");
                    continue;
                }
            }
        }

        public static async Task ChangeCategoryProduct(MyAppContext db, Product book) 
        {
            Console.Clear();

            List<Category> categories = DapperQueries.GetCategories();

            Window window = Helpers.ShowCategories(categories);
            window.Draw();

            while (true)
            {
                Console.WriteLine("Choose a category to put the book in");

                string input = Console.ReadLine();

                if (input == "q")
                {
                    Console.WriteLine("Exiting...");
                    await Task.Delay(1000);
                    break;
                }

                if (int.TryParse(input, out int catId))
                {
                    if (categories.Any(c => c.Id == catId))
                    {
                        var category = await db.Categories.FirstOrDefaultAsync(c => c.Id == catId);
                        book.Category = category;
                        await db.SaveChangesAsync();
                        Console.WriteLine("The category successfully updated.");
                        Console.ReadKey(true);
                        break;
                    }
                    else
                    {
                        Console.WriteLine("No category found with that Id. Please try again.");
                        Console.ReadKey(true);
                        continue;
                    }
                }
                else
                {
                    Console.WriteLine("Invalid input. Please enter a number or 'q'.");
                    Console.ReadKey(true);
                    continue;
                }
            }
        }

        public static void DeleteProduct(MyAppContext db, int id) 
        {
            var book = db.Products.FirstOrDefault(p => p.Id == id);

            try
            {
                db.Products.Remove(book);
                db.SaveChanges();
                Console.WriteLine("Successfully deleted product.");
                Console.ReadKey(true);
                return;
            }
            catch (DbUpdateException ex)
            {
                Console.WriteLine("Something went wrong..");
                Console.WriteLine(ex.StackTrace);
            }
        }

        public static async Task ChangeInfo(MyAppContext db, Product book) 
        {
            Console.Clear();

            while (true)
            {
                Console.Write("Enter what the new information should be: ");
                string answer = Console.ReadLine();

                if (answer != "q")
                {
                    try
                    {
                        if (answer != null)
                        {
                            book.Information = answer;
                            await db.SaveChangesAsync();
                            break;
                        }
                    }
                    catch (DbUpdateException ex)
                    {
                        Console.WriteLine("SOmething went wrong ");
                        Console.WriteLine(ex.StackTrace);
                    }
                }
                else
                {
                    return;
                }

            }
        }

        public static async Task ChangeName(MyAppContext db, Product book) 
        {
            Console.Clear();
            while (true)
            {
                Console.Write("Enter the new name for the book");
                string answer = Console.ReadLine();
                if (answer == "q")
                {
                    Console.WriteLine("Exiting...");
                    await Task.Delay(1000);
                    break;
                }

                try
                {
                    if (answer != null)
                    {
                        book.Name = answer;
                        await db.SaveChangesAsync();
                        break;
                    }
                    else
                    {
                        Console.WriteLine("Not a valid name");
                        continue;
                    }
                }

                catch (DbUpdateException ex)
                {
                    Console.WriteLine("SOmething went wrong ");
                    Console.WriteLine(ex.StackTrace);
                }
            }

        }

        public static async Task ChangeProduct(MyAppContext db, int id) 
        {
            Console.Clear();

            try
            {
                var book = await db.Products.FirstOrDefaultAsync(b => b.Id == id);
                if (book == null)
                {
                    Console.WriteLine("The book cannot be found.");
                    Console.ReadKey(true);
                    return;
                }

                while (true)
                {
                    List<string> options = Helpers.EnumsToLists(typeof(Enums.adminProductEnums));
                    var window = new Window("Options", 2, 0, options);
                    Console.Clear(); 
                    window.Draw();
                    Console.WriteLine("What do you want to change for the book? (Press 'q' to go back)");
                    ConsoleKeyInfo key = Console.ReadKey(true);

                    if (key.KeyChar == 'q')
                    {
                        Console.Clear();
                        return;
                    }

                    if (int.TryParse(key.KeyChar.ToString(), out int input))
                    {
                        if (Enum.IsDefined(typeof(Enums.adminProductEnums), input))
                        {
                            switch ((Enums.adminProductEnums)input)
                            {
                                case Enums.adminProductEnums.name:
                                    await ChangeName(db, book);
                                    break;
                                case Enums.adminProductEnums.info:
                                    await ChangeInfo(db, book);
                                    break;
                                case Enums.adminProductEnums.change_supplier:
                                    await ChangeSupplierBook(db, book);
                                    break;
                                case Enums.adminProductEnums.instock:
                                    await InStockProduct(db, book);
                                    break;
                                case Enums.adminProductEnums.price:
                                    await ChangePriceProduct(db, book);
                                    break;
                                case Enums.adminProductEnums.category:
                                    await ChangeCategoryProduct(db, book);
                                    break;
                                case Enums.adminProductEnums.Delete_product:
                                    DeleteProduct(db, id);
                                    return;
                                case Enums.adminProductEnums.Is_displayed:
                                    await FrontPageProduct(db, book);
                                    break;
                            }
                        }
                        else
                        {
                            Console.WriteLine("Invalid option, please try again.");
                            Console.ReadKey(true);
                        }
                    }
                    else
                    {
                        Console.WriteLine("invalid input. Please enter a number from the options or 'q'.");
                        Console.ReadKey(true);
                    }
                }
            }
            catch (DbUpdateException ex)
            {
                Console.WriteLine("Something went wrong");
                Console.WriteLine(ex.StackTrace);
            }

        }
        public static async Task AdminProduct(MyAppContext db, int id) 
        {
            Console.Clear();
            try
            {
                var book = await db.Products.FirstOrDefaultAsync(b => b.Id == id);

                if (book != null)
                {
                    List<string> bookWindow = new List<string> { book.Information, book.Price.ToString() + "$" + " In stock: " + book.InStock + " 'c' to change product information. Any other key to go back" };
                    var window = new Window(book.Name, 1, 1, bookWindow);
                    window.Draw();
                    ConsoleKeyInfo key = Console.ReadKey(true);
                    if (key.KeyChar == 'c')
                    {
                        await ChangeProduct(db, id);
                        return;
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


        
        public static async Task AddProduct() // long function, but all it does is add a new product, and log it to mongodb 
        {
            using (MyAppContext db = new MyAppContext())
            {
                Product book = new();
                try
                {
                    while (true)
                    {
                        Console.Clear();
                        Console.Write("Enter the name of the new product press 'q' to quit");
                        string? name = Console.ReadLine();

                        if (name == "q")
                        {
                            return;
                        }

                        if (!string.IsNullOrWhiteSpace(name))
                        {
                            book.Name = name;
                            break;
                        }

                        Console.WriteLine("The book cant be null");
                        Console.ReadKey(true);
                    }

                    while (true)
                    {
                        Console.Clear();
                        Console.Write("Enter the Price of the book");
                        string? answer = Console.ReadLine();

                        if (answer == "q")
                        {
                            return;
                        }
                        if(!string.IsNullOrWhiteSpace(answer))
                        {
                            if (decimal.TryParse(answer, out decimal price))
                            {
                                book.Price = price;
                                break;
                            }
                        }

                        Console.WriteLine("Must be a valid decimal value");
                        Console.ReadKey(true);
                    }

                    while (true)
                    {
                        Console.Clear();
                        Console.Write("How many books in stock? ");
                        string? answer = Console.ReadLine();

                        if (answer == "q")
                        {
                            return;
                        }
                            if (!string.IsNullOrWhiteSpace(answer))
                            {
                                if (int.TryParse(answer, out int num))
                            {
                                book.InStock = num;
                                break;
                            }
                        }

                        Console.WriteLine("Must be a valid number");
                        Console.ReadKey(true);
                        continue;
                    }

                    while (true)
                    {
                        Console.Clear();
                        Console.Write("Enter a short description of the book");
                        string? desc = Console.ReadLine();

                        if (desc == "q")
                        {
                            return;
                        }

                        if (!string.IsNullOrWhiteSpace(desc))
                        {
                            book.Information = desc;
                            break;
                        }

                        Console.WriteLine("Cant be null");
                        Console.ReadKey(true);
                        continue;
                    }
                   
                    while (true)
                    {
                        List<Category> categories = DapperQueries.GetCategories();
                        Window window = Helpers.ShowCategories(categories);
                        Console.Clear();
                        window.Draw();
                        Console.WriteLine("Enter the category of the book by the id 'n' to add a new category ");

                        string? answer = Console.ReadLine();
                        if (answer == "q")
                        {
                            return;
                        }

                        if(answer == "n")
                        {

                            await AddCategory(db);
                            continue;
                        }

                        if (!string.IsNullOrWhiteSpace(answer))
                        {
                            if (int.TryParse(answer, out int id))
                            {
                                var category = await db.Categories.FirstOrDefaultAsync(c => c.Id == id);
                                if (category != null)
                                {
                                    book.Category = category;
                                    break;
                                }
                                else
                                {
                                    Console.WriteLine("category cant be null");
                                    Console.ReadKey(true);
                                    continue;
                                }
                            }
                            Console.WriteLine("Must be a valid id");
                            Console.ReadKey(true);
                            continue;
                        }
                    }

                    while (true)
                    {
                        var suppliers = await db.Suppliers.ToListAsync();
                        var windowSuppliers = Helpers.GetSuppliers(db, suppliers);
                        Console.Clear();
                        windowSuppliers.Draw();
                        Console.WriteLine("Who is the supplier of the book? press 'n' to add new supplier");
                        string? answer = Console.ReadLine();
                        if (answer == "q")
                        {
                            return;
                        }

                        if(answer == "n")
                        {
                            await AdminSupplier.AddSupplier(db);
                            continue;
                        }

                        if(!string.IsNullOrWhiteSpace(answer))
                        {
                            if (int.TryParse(answer, out int id))
                            {
                                    var supplier = await db.Suppliers.FirstOrDefaultAsync(s => s.Id == id);
                                    if (supplier != null)
                                    {
                                        book.Supplier = supplier;
                                        break;
                                    }
                                    else
                                    {
                                        Console.WriteLine("Supplier cant be null");
                                        Console.ReadKey(true);
                                        continue;
                                    }
                                }
                            }
                        Console.WriteLine("Id must be a valid number");
                        Console.ReadKey(true);
                        continue;
                    }

                    while (true)
                    {
                        Console.Clear();
                        Console.WriteLine("Who is the autohor of the book? press 'n' to add a new author");

                        var authors = await db.Authors.ToListAsync();
                       
                        foreach (var a in authors)
                        {
                            Console.WriteLine($"{a.Id}: {a.Name}");
                        }

                        string? answer = Console.ReadLine();
                        if (answer == "q")
                        {
                            return;
                        }

                        if(answer == "n")
                        {
                            Console.Clear();
                            Console.WriteLine("What is the authors name? ");
                            string? name = Console.ReadLine();

                            if(name != null)
                            {
                                Author author = new Author{ Name = name };
                                db.Authors.Add(author);
                                book.Author = author;
                                await db.SaveChangesAsync();
                                continue;
                            }
                        }


                        if (int.TryParse(answer, out int id))
                        {
                            var author = await db.Authors.FirstOrDefaultAsync(a => a.Id == id);
                            if (author != null)
                            {
                                book.Author = author;
                                break;
                            }
                            else
                            {
                                Console.WriteLine("Author cant be null");
                                Console.ReadKey(true);
                                continue;
                            }
                        }
                        else
                        {
                            Console.WriteLine("must be a valid id");
                            Console.ReadKey(true);
                            continue;
                        }
                    }

                    book.IsDisplayed = false;

                    db.Products.Add(book);

                    await db.SaveChangesAsync();
                    MongoQueries.InsertNewProductLog(new ModelsMDB.AddProduct
                    {
                        ProductId = book.Id,
                        Date = DateTime.Now
                    });
                    Console.WriteLine("Book added succesfully");
                    Console.ReadKey(true);
                }
                catch (DbUpdateException ex)
                {
                    Console.WriteLine("Something went wrong");
                    Console.WriteLine(ex.StackTrace);
                }
            }
        }


        public static async Task FrontPageProduct(MyAppContext db, Product book) 
        {
            try
            {
                if(book.IsDisplayed)
                {
                    while(true)
                    {
                        Console.WriteLine("This book is displayed do you want to change that? y/n 'q' to quit");
                        ConsoleKeyInfo key = Console.ReadKey(true);

                        if(key.KeyChar == 'q')
                        {
                            return;
                        }

                        if(key.KeyChar == 'y')
                        {
                            book.IsDisplayed = false;
                            await db.SaveChangesAsync();
                            Console.WriteLine("Okay the book is not displayed anymore.");
                            Console.ReadKey(true);
                            break;
                        }
                        else
                        {
                            Console.WriteLine("Okay! going back...");
                            Thread.Sleep(1000);
                            return;
                        }
                    }
                }

                if(!book.IsDisplayed)
                {
                    while(true)
                    {
                        Console.WriteLine("Do you want to display this book in the front page? y/n press 'q' to quit");

                        ConsoleKeyInfo key = Console.ReadKey(true);

                        if (key.KeyChar == 'q')
                        {
                            return;
                        }
                    
                        if(key.KeyChar == 'y')
                        {
                            var theList = await db.Products.Where(p => p.IsDisplayed).ToListAsync();

                            if(theList.Count < 3)
                            {
                                book.IsDisplayed = true;
                                await db.SaveChangesAsync();
                                Console.WriteLine("The book has been updated.");
                                Console.ReadKey(true);
                                break;
                            }
                            Console.WriteLine("There are too many books displayed on the frontpage. ");
                            Console.ReadKey(true);
                            return;
                        }
                    }
                }
            }
            catch(DbUpdateException ex)
            {
                Console.WriteLine("SOmething went wrong..");
                Console.WriteLine(ex.StackTrace);
            }
        }
    }
}
