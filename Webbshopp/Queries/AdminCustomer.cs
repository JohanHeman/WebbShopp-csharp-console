using Azure.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Common;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;
using Webbshop.Connections;
using Webbshop.Models;
using WindowDemo;

namespace Webbshop.Queries
{
    internal class AdminCustomer
    {

        public static void AdminCustomerOperations()
        {
            while (true)
            {
                Console.Clear();
                List<string> productList = Helpers.EnumsToLists(typeof(Enums.adminCustomerEnums));
                var window = new Window("AdminMenu", 2, 0, productList);
                window.Draw();
                Console.WriteLine("Press 'Q' to go back.");
                ConsoleKeyInfo key = Console.ReadKey(true);
                char inputChar = char.ToUpper(key.KeyChar);

                if (inputChar == 'Q')
                {
                    return;
                }

                if (int.TryParse(inputChar.ToString(), out int input))
                {
                    switch ((Enums.adminCustomerEnums)input)
                    {
                        case Enums.adminCustomerEnums.Change_customer_info:
                            ChangeCustomerInfo();
                            break;
                    }
                }

                else
                {
                    Console.WriteLine("Invalid input, press any key to go back");
                }

            }
        }


        public static void ChangeCustomerInfo()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("press 'S' to search for customer and 'B' to browse customers\npress 'Q' to quit");
                ConsoleKeyInfo key = Console.ReadKey(true);
                char inputChar = char.ToUpper(key.KeyChar);

                switch (inputChar)
                {
                    case 'Q':
                        return;
                    case 'S':
                        SearchCustomer();
                        break;
                    case 'B':
                        BrowseCustomers();
                        break;
                    default:
                        Console.WriteLine("Invalid input. ");
                        continue;
                }
            }
        }

        public static void SearchCustomer()
        {
            try
            {
                using (var db = new MyAppContext())
                {
                    while (true)
                    {
                        Console.Clear();
                        Console.WriteLine("Search for the customer by the name: press 'Q' to quit");
                        string search = Console.ReadLine();
                        // handles uppercase / lowercase input for 'Q'
                        if (search.Equals("Q", StringComparison.OrdinalIgnoreCase))
                        {
                            return;
                        }

                        var customersResult = db.Customers.Where(c => c.Name.Contains(search)).ToList();

                        if (customersResult.Count == 0)
                        {
                            Console.WriteLine("No cusatomer with that name found.");
                        }

                        foreach (var c in customersResult)
                        {
                            Console.WriteLine($"{c.Id}: {c.Name}");
                        }

                        ChooseCustomer(db);
                    }
                }
            }
            catch (DbException e)
            {
                Console.WriteLine("Something went wrong");
                Console.WriteLine(e.StackTrace);
            }
        }

        public static Customer ChangeName(Customer customer, MyAppContext db)
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("Enter the new name for the customer or press 'Q' to quit");
                string name = Console.ReadLine();

                if (name.Equals("Q", StringComparison.OrdinalIgnoreCase))
                {
                    return customer;
                }
                try
                {
                    customer.Name = name;
                    db.SaveChanges();
                    Console.WriteLine("The customer is succesfully updated.");
                    Console.ReadKey(true);
                    return customer;
                }
                catch (DbUpdateException ex)
                {
                    Console.WriteLine("Something went wrong");
                    Console.WriteLine(ex.StackTrace);
                }
            }
        }

        public static Customer ChangePhonenUmber(Customer customer, MyAppContext db)
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("Enter the new number for the customer or press 'Q' to quit");
                string num = Console.ReadLine();

                if (num.Equals("Q", StringComparison.OrdinalIgnoreCase))
                {
                    return customer;
                }
                try
                {
                    customer.PhoneNumber = num;
                    db.SaveChanges();
                    Console.WriteLine("The customer is succesfully updated.");
                    Console.ReadKey(true);
                    return customer;
                }
                catch (DbUpdateException ex)
                {
                    Console.WriteLine("Something went wrong");
                    Console.WriteLine(ex.StackTrace);
                }
            }
        }

        public static Customer ChangeEmail(Customer customer, MyAppContext db)
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("Enter the new email for the customer or press 'Q' to quit");
                string email = Console.ReadLine();

                if (email.Equals("Q", StringComparison.OrdinalIgnoreCase))
                {
                    return customer;
                }
                try
                {
                    customer.Email = email;
                    db.SaveChanges();
                    Console.WriteLine("The customer is succesfully updated.");
                    Console.ReadKey(true);
                    return customer;
                }
                catch (DbUpdateException ex)
                {
                    Console.WriteLine("Something went wrong");
                    Console.WriteLine(ex.StackTrace);
                }
            }
        }


        public static Customer ChangeAge(Customer customer, MyAppContext db)
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("Enter the new age for the customer or press 'Q' to quit");
                string age = Console.ReadLine();

                if (age.Equals("Q", StringComparison.OrdinalIgnoreCase))
                {
                    return customer;
                }
                try
                {
                    if(int.TryParse(age, out int customerAge))
                    {
                        customer.Age = customerAge;
                        db.SaveChanges();
                        Console.WriteLine("The customer is succesfully updated.");
                        Console.ReadKey(true);
                        return customer;
                    }
                }
                catch (DbUpdateException ex)
                {
                    Console.WriteLine("Something went wrong");
                    Console.WriteLine(ex.StackTrace);
                }
            }
        }

        public static Customer ChangeAddress(Customer customer, MyAppContext db)
        {
            Address address = new Address();


            Console.WriteLine("What do you want to update? ");
            Console.WriteLine("1. Country / city");
            Console.WriteLine("2. street");

            ConsoleKeyInfo key = Console.ReadKey(true);
            char inputChar = char.ToUpper(key.KeyChar);
            if (inputChar == 'Q') return customer;

            if (int.TryParse(inputChar.ToString(), out int input))
            {
                switch (input)
                {
                    case 1:
                        ChangeCountry(customer, db);
                        break;
                    case 2:
                        ChangeStreet(customer, db);
                        break;
                }
            }
            return customer;
        }

        public static Customer ChangeCountry(Customer customer, MyAppContext db)
        {
            var address = customer.Adresses.FirstOrDefault();
            var addresses = customer.Adresses.ToList();
            if(addresses.Count > 1)
            {
                while(true)
                {
                    Console.Clear();
                    List<string> options = new List<string>();
                    foreach(var item in addresses)
                    {
                        options.Add(item.Id.ToString() + ": " + item.Street);
                    }
                    var window = new Window("Options", 0, 2, options);
                    window.Draw();

                    Console.WriteLine("What address do you want to change? press 'Q' to quit");
                    string answer = Console.ReadLine().ToUpper();
                    if (answer == "Q")
                        return customer;

                    if(int.TryParse(answer, out int id))
                    {
                        var checkAddress = db.Addresses.FirstOrDefault(a => a.Id == id);
                        if(address != null)
                        {
                            address = checkAddress;
                            break;
                        }
                        else
                        {
                            Console.WriteLine("No address with that id");
                            Console.ReadKey(true);
                            continue;
                        }
                    }
                }

            }

            while (true)
            {
                Console.Clear();
                Console.Write("Enter the country: ");
                string country = Console.ReadLine();
                var existingCountry = db.Countries
                    .Include(c => c.Cities)
                    .FirstOrDefault(c => c.Name == country);

                if (existingCountry != null)
                {
                    foreach (var c in existingCountry.Cities)
                    {
                        Console.WriteLine($"{c.Id}: {c.Name}");
                    }

                    Console.WriteLine("Either choose a city here or press 'N' to add new city");
                    string input = Console.ReadLine().ToUpper();

                    if (input == "Q") return customer;

                    if (input == "N")
                    {
                        try
                        {
                            Console.WriteLine("Enter the city name");
                            string name = Console.ReadLine();
                            City city = new City
                            {
                                Name = name,
                                Country = existingCountry
                            };
                            db.Cities.Add(city);
                            db.SaveChanges();
                            address.CityId = city.Id;
                            db.SaveChanges();

                            Console.WriteLine("succesfully added " + city.Name + " to customer");
                            Console.ReadKey(true);
                            return customer;
                        }
                        catch (DbUpdateException e)
                        {
                            Console.WriteLine("Something went wrong ");
                            Console.WriteLine(e.StackTrace);
                        }
                    }

                    if (int.TryParse(input, out int id))
                    {
                        var city = existingCountry.Cities.FirstOrDefault(c => c.Id == id);
                        if (city != null)
                        {
                            address.CityId = city.Id;
                            db.SaveChanges();
                            Console.WriteLine("Updated address for customer");
                            Console.ReadKey(true);
                            return customer;
                        }
                        else
                        {
                            Console.WriteLine("No city with that id");
                            Console.ReadKey(true);
                            continue;
                        }
                    }
                }
                else
                {
                    Console.WriteLine("That country does not exist yet.");
                    Console.WriteLine("Do you want to add it? (Y/N) or press 'Q' to quit");
                    string choice = Console.ReadLine().ToUpper();

                    if (choice == "Q")
                        return customer;

                    if (choice == "Y")
                    {
                        try
                        {
                            Country newCountry = new Country
                            {
                                Name = country
                            };
                            db.Countries.Add(newCountry);
                            db.SaveChanges();

                            Console.WriteLine("Enter the name of a city in this country:");
                            string cityName = Console.ReadLine();

                            City city = new City
                            {
                                Name = cityName,
                                Country = newCountry
                            };
                            db.Cities.Add(city);
                            db.SaveChanges();

                            address.CityId = city.Id;
                            db.SaveChanges();

                            Console.WriteLine($"Successfully added {newCountry.Name} / {city.Name} to customer.");
                            Console.ReadKey(true);
                            return customer;
                        }
                        catch (DbUpdateException e)
                        {
                            Console.WriteLine("Something went wrong when adding the country/city.");
                            Console.WriteLine(e.StackTrace);
                            Console.ReadKey(true);
                        }
                    }
                    else
                    {
                        Console.WriteLine("Country not added. Press any key to try again.");
                        Console.ReadKey(true);
                    }
                }
            }
        }

        public static Customer ChangeStreet(Customer customer, MyAppContext db)
        {
            var address = customer.Adresses.FirstOrDefault();
            var addresses = customer.Adresses.ToList();
            if (addresses.Count > 1)
            {
                while (true)
                {
                    List<string> options = new List<string>();
                    foreach (var item in addresses)
                    {
                        options.Add(item.Id.ToString() + ": " + item.Street);
                    }
                    var window = new Window("Options", 0, 2, options);
                    window.Draw();

                    Console.WriteLine("What address do you want to change? press 'Q' to quit");
                    string answer = Console.ReadLine().ToUpper();
                    if (answer == "Q")
                        return customer;

                    if (int.TryParse(answer, out int id))
                    {
                        var checkAddress = db.Addresses.FirstOrDefault(a => a.Id == id);
                        if (address != null)
                        {
                            address = checkAddress;
                            break;
                        }
                        else
                        {
                            Console.WriteLine("No address with that id");
                            Console.ReadKey(true);
                            continue;
                        }
                    }
                }
            }

            Console.Clear();

            while(true)
            {
                Console.WriteLine("Enter the new address name");
                string name = Console.ReadLine();

                address.Street = name;
                db.SaveChanges();
                Console.WriteLine("Updated street name to " + name);
                Console.ReadKey(true);
                return customer;
            }

        }

        public static void BrowseCustomers()
        {
            using(var db = new MyAppContext())
            {
                while(true)
                {
                    var customers = db.Customers.ToList();
                    foreach (var customer in customers)
                    {
                        Console.WriteLine($"{customer.Id}: {customer.Name}");
                    }
                    ChooseCustomer(db);
                    break;
                }
            }
        }

        public static Customer ChooseCustomer(MyAppContext db)
        {
            while(true)
            {
                Console.WriteLine("Enter the customer by choosing the id or 'Q' to quit");
                string inputLine = Console.ReadLine();

                if (inputLine.Equals("Q", StringComparison.OrdinalIgnoreCase))
                {
                    return null;
                }

                if (int.TryParse(inputLine, out int id))
                {
                    var customer = db.Customers.Include(c => c.Adresses).FirstOrDefault(c => c.Id == id);
                    if (customer != null)
                    {
                        Console.Clear();
                        Console.WriteLine("What do you want to change on the customer? ");

                        List<string> options = Helpers.EnumsToLists(typeof(Enums.ChangeCustomerInfo));
                        var window = new Window("Options", 0, 2, options);
                        Console.Clear();
                        window.Draw();


                        ConsoleKeyInfo key = Console.ReadKey(true);
                        char inputChar = char.ToUpper(key.KeyChar);

                        if (int.TryParse(inputChar.ToString(), out int input))
                        {
                            switch ((Enums.ChangeCustomerInfo)input)
                            {
                                case Enums.ChangeCustomerInfo.Name:
                                    customer = ChangeName(customer, db);
                                    break;
                                case Enums.ChangeCustomerInfo.Phone_number:
                                    customer = ChangePhonenUmber(customer, db);
                                    break;
                                case Enums.ChangeCustomerInfo.Email:
                                    customer = ChangeEmail(customer, db);
                                    break;
                                case Enums.ChangeCustomerInfo.Age:
                                    customer = ChangeAge(customer, db);
                                    break;
                                case Enums.ChangeCustomerInfo.address:
                                    customer = ChangeAddress(customer, db);
                                    break;
                                case Enums.ChangeCustomerInfo.Delete_customer:
                                    DeleteCustomer(customer, db);
                                    break;
                            }
                        }
                        return customer; 
                    }
                    else
                    {
                        Console.WriteLine("No customer found with that ID. Press any key to try again.");
                        Console.ReadKey(true);
                    }
                }
                else
                {
                    Console.WriteLine("Invalid input. Please enter a number or 'Q'. Press any key to try again.");
                    Console.ReadKey(true);
                }
            }
        }
        public static void DeleteCustomer(Customer customer,  MyAppContext db)
        {
            if(customer != null)
            {
                Console.WriteLine("Are you sure you want to delete customer " + customer.Name + "? All the data will be removed related to this customer.");
                Console.WriteLine("Y/N");

                ConsoleKeyInfo key = Console.ReadKey(true);
                char inputChar = char.ToUpper(key.KeyChar);

                if(inputChar == 'Y')
                {
                    try
                    {
                        db.Customers.Remove(customer);
                        db.SaveChanges();
                        Console.WriteLine("The customer has been deleted");
                        Console.ReadKey(true);
                        return;
                    }
                    catch(DbUpdateException e)
                    {
                        Console.WriteLine("Something went wrong");
                        Console.WriteLine(e.StackTrace);
                    }
                }
                else
                {
                    return;
                }
            }
        }
    }
}

