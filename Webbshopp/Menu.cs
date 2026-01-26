using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Webbshop.Connections;
using Webbshop.Models;
using Webbshop.Queries;
using WindowDemo;

namespace Webbshop
{
    internal class Menu
    {


        public static async Task StartMenu(User user)
        {
            

            using(var db = new MyAppContext())
            {
                while (true)
                {
                    var products = await db.Products.Where(p => p.IsDisplayed).Include(p => p.Author).Take(3).ToListAsync();
                    
                    Console.Clear();
                    List<string> welcomeText = new List<string> { "Welcome to the Book store. ", "We provide books" };

                    var windowStart = new Window("The Book Shop", 37, 0, welcomeText);
                    windowStart.Draw();

                    Product bookOne = null;
                    Product bookTwo = null;
                    Product bookThree = null;

                    if (products.Count > 0)
                        bookOne = products[0];

                    if (products.Count > 1)
                        bookTwo = products[1];

                    if (products.Count > 2)
                        bookThree = products[2];


                    List<string> dealOne = new List<string> { bookOne == null ? "No book" : bookOne.Name, bookOne == null ? "No book" : bookOne.Author.Name, "Press 'A' to purchase" };
                    var dealOneWindow = new Window("Deal 1", 10, 5, dealOne);
                    dealOneWindow.Draw();

                    List<string> dealTwo = new List<string> { bookTwo == null ? "No book" : bookTwo.Name, bookTwo == null ? "No book" : bookTwo.Author.Name, "Press 'B' to purchase" };
                    var dealTwoWindow = new Window("Deal 2", 40, 5, dealTwo);
                    dealTwoWindow.Draw();

                    List<string> dealThree = new List<string> { bookThree == null ? "No book": bookThree.Name, bookThree == null ? "No book" : bookThree.Author.Name, "Press 'C' to purchase" };
                    var dealThreeWindow = new Window("Deal 3", 70, 5, dealThree);
                    dealThreeWindow.Draw();

                    List<string> menuChoices = Helpers.EnumsToLists(typeof(Enums.HomeEnums));

                    var menuWindow = new Window("Menu", 2, 0, menuChoices);
                    menuWindow.Draw();
                    Console.WriteLine("Press 'q' to quit");

                    bool validInput = false;

                    ConsoleKeyInfo key = Console.ReadKey(true);
                    char inputChar = char.ToUpper(key.KeyChar);
                    

                    if (inputChar == 'Q') break;


                    else
                    {
                        if (int.TryParse(inputChar.ToString(), out int input))
                        {

                            switch ((Enums.HomeEnums)input)
                            {
                                case Enums.HomeEnums.Customer_menu:
                                    CustomerMenu(user);
                                    validInput = true;
                                    break;
                                case Enums.HomeEnums.Admin_menu:
                                    await AdminMenu();
                                    continue;
                            }
                        }

                        else
                        {
                            switch(inputChar)
                            {
                                case 'A':
                                    ShowBook(bookOne);
                                    validInput = true;
                                    break;
                                case 'B':
                                    ShowBook(bookTwo);
                                    validInput = true;
                                    break;
                                case 'C':
                                    ShowBook(bookThree);
                                    validInput = true;
                                    break;
                            }
                        }

                        if(!validInput)
                        {
                            Console.Clear();
                            Console.WriteLine("Not a valid input please try again");
                            Console.WriteLine("Press any key to go back");

                            Console.ReadKey(true);
                        }
                    }
                }
                Console.Clear();
                Console.WriteLine("Thank you for visiting. Please come again!");
            }
        }

        public static void CustomerMenu(User? currentUser)
        {
            while(true)
            {
                Console.Clear();
                List<string> customerChoices = Helpers.EnumsToLists(typeof(Enums.customerEnums));

                var window = new Window("CustomerMenu", 2, 0, customerChoices);
                window.Draw();

                ConsoleKeyInfo key = Console.ReadKey(true);

                if (int.TryParse(key.KeyChar.ToString(), out int input))
                {
                    switch ((Enums.customerEnums)input)
                    {
                        case Enums.customerEnums.Home:
                            Console.WriteLine("Returning to home...");
                            Thread.Sleep(1000);
                            return;
                        case Enums.customerEnums.Shop:
                            ShopMenu();
                            break;
                        case Enums.customerEnums.Shoppingcart:
                            CartQueries.ShowCart(currentUser);
                            break;
                    }
                }
                else
                {
                    Console.Clear();
                    Console.WriteLine("Not a valid input please try again");
                    Console.WriteLine("Press any key to go back");

                    Console.ReadKey(true);
                }
            }

        }

        public static async Task AdminMenu()
        {
            while(true)
            {
                Console.Clear();
                List<string> adminChoices = Helpers.EnumsToLists(typeof(Enums.adminEnums));

                var window = new Window("AdminMenu", 2, 0, adminChoices);
                window.Draw();

                ConsoleKeyInfo key = Console.ReadKey(true);
                if (int.TryParse(key.KeyChar.ToString(), out int input))
                {
                    switch ((Enums.adminEnums)input)
                    {
                        case Enums.adminEnums.Product_categories:
                           await AdminCategory.AdminCategories();
                            break;
                        case Enums.adminEnums.Customer_management:
                            AdminCustomer.AdminCustomerOperations();
                            break;
                        case Enums.adminEnums.Home:
                            Console.WriteLine("Returning to home...");
                            Thread.Sleep(1000);
                            return;
                        case Enums.adminEnums.supplier:
                            await AdminSupplier.ShowSuppliers();
                            break;
                        case Enums.adminEnums.Add_product:
                            await AdminCategory.AddProduct();
                            break;
                        case Enums.adminEnums.Statistics:
                            StatisticQuerries.ShowStatistics();
                            break;
                    }
                }
                else
                {
                    Console.Clear();
                    Console.WriteLine("Not a valid input please try again");
                    Console.WriteLine("Press any key to go back");

                    Console.ReadKey(true);
                }
            }

        }

        public static void CustomerCategories()
        {
            Console.Clear();
           using(var db = new Connections.MyAppContext())
            {
                List<Category> categories = DapperQueries.GetCategories();


                Window window = Helpers.ShowCategories(categories);
                window.Draw();
                ConsoleKeyInfo key = Console.ReadKey(true);

                if(int.TryParse(key.KeyChar.ToString(), out int input))
                {
                    if(categories.Any(c => c.Id == input))
                    {
                        NavigationQueries.ShowCategory(input);
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

        public static void ShopMenu()
        {
            while(true)
            {
                Console.Clear();
                List<string> shopList = Helpers.EnumsToLists(typeof(Enums.shopEnums));

                var window = new Window("Shop", 2, 0, shopList);

                window.Draw();

                ConsoleKeyInfo key = Console.ReadKey(true);

                if (int.TryParse(key.KeyChar.ToString(), out int input))
                {
                    switch ((Enums.shopEnums)input)
                    {
                        case Enums.shopEnums.Categories:
                            CustomerCategories();
                            break;
                        case Enums.shopEnums.Back:
                            return;
                        case Enums.shopEnums.Search:
                            NavigationQueries.SearchBooks();
                            break;
                    }
                }
                else
                {
                    Console.Clear();
                    Console.WriteLine("Not a valid input please try again");
                    Console.WriteLine("Press any key to go back");

                    Console.ReadKey(true);
                }
            }
        }

        public static void ShowBook(Product book)
        {
            if(book != null)
            {
                NavigationQueries.InfoBook(book.Id);
            }
            else
            {
                Console.WriteLine("There is currently no book here");
                Console.ReadKey(true);
                return;
            }
        }
    }
}
