using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Webbshop.Models;
using Webbshop.Queries;
using WindowDemo;

namespace Webbshop
{
    internal class Menu
    {


        public static async Task StartMenu()
        {
            while (true)
            {
                // later in this the names of the books will be from the database, isDeal= true
                    Console.Clear();
                List<string> welcomeText = new List<string> { "Welcome to the Book store. ", "We provide books" };

                var windowStart = new Window("The Book Shop", 37, 0, welcomeText);
                windowStart.Draw();

                List<string> dealOne = new List<string> { "Book name 1", "Author", "Press p to purchase" };
                var dealOneWindow = new Window("Deal 1", 10, 5, dealOne);
                dealOneWindow.Draw();

                List<string> dealTwo = new List<string> { "Book name 2", "Author", "Press p to purchase" };
                var dealTwoWindow = new Window("Deal 2", 40, 5, dealTwo);
                dealTwoWindow.Draw();

                List<string> dealThree = new List<string> { "Book name 3", "Author", "Press p to purchase" };
                var dealThreeWindow = new Window("Deal 3", 70, 5, dealThree);
                dealThreeWindow.Draw();

                List<string> menuChoices = Helpers.EnumsToLists(typeof(Enums.HomeEnums));

                var menuWindow = new Window("Menu", 2, 0, menuChoices);
                menuWindow.Draw();
                Console.WriteLine("Press 'q' to quit");

                ConsoleKeyInfo key = Console.ReadKey(true);

                if (key.KeyChar == 'q') break;

                else
                {
                    if (int.TryParse(key.KeyChar.ToString(), out int input))
                    {

                        switch ((Enums.HomeEnums)input)
                        {
                            case Enums.HomeEnums.Customer_menu:
                                CustomerMenu();
                                break;
                            case Enums.HomeEnums.Admin_menu:
                                await AdminMenu();
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
            Console.Clear();
            Console.WriteLine("Thank you for visiting. Please come again!");

        }

        public static void CustomerMenu()
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
                            Queries.CartQueries.ShowCart();
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
                            AdminQueries.AdminCustomer();
                            break;
                        case Enums.adminEnums.Home:
                            Console.WriteLine("Returning to home...");
                            Thread.Sleep(1000);
                            return;
                        case Enums.adminEnums.supplier:
                            await AdminSupplier.ShowSuppliers();
                            break;
                        case Enums.adminEnums.Add_product:
                            await AdminQueries.AddProduct();
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
    }
}
