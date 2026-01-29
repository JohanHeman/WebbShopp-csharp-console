using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Identity.Client;
using Microsoft.SqlServer.Server;
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
    internal class Helpers
    {
        public static List<string> EnumsToLists(Type e) 
        {
            List<string> theList = new List<string>();

            if(!e.IsEnum)
            {
                throw new ArgumentException("The function expects an enum type!");
            }

            var enumValues = Enum.GetValues(e);
            
            foreach ( var value in enumValues )
            {
                theList.Add((int)value + ": " + value.ToString());
            }
            return theList;   
        }

        public static Window ShowCategories(List<Category> cList) 
        {
            Console.Clear();
            cList = DapperQueries.GetCategories();
            Console.Clear();

            List<string> windowList = new List<string>();

            foreach (var c in cList)
            {
                windowList.Add(c.Id + ": " + c.Name);
            }

            var window = new Window("Categories", 1, 0, windowList);
            return window;
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


        public static User? SignIn() // this function either lets a user sign in to existing account or register an account.
        {
            using (var db = new MyAppContext())
            {
                while(true)
                {
                    Console.Clear();
                    Console.WriteLine("Press 'L' to login and 'R' to register");

                    ConsoleKeyInfo MenuKey = Console.ReadKey(true);

                    char menuInput = char.ToUpper(MenuKey.KeyChar);

                    if (menuInput == 'L')
                    {
                        Console.WriteLine("Press q to quit at any time ");
                        while (true)
                        {
                            Console.Write("Username: ");
                            string? userName = Console.ReadLine();
                            if(userName != null && userName.Equals("q", StringComparison.OrdinalIgnoreCase))
                            {
                                return null;
                            }

                            Console.Write("Password: ");
                            string? password = Console.ReadLine();

                            if(password != null && password.Equals("q", StringComparison.OrdinalIgnoreCase))     
                            {
                                return null;
                            }
                            
                            var user = db.Users.FirstOrDefault(u => u.UserName == userName && u.Password == password);

                            if(user != null)
                            {
                                MongoQueries.InsertUserLog(new ModelsMDB.UserLog
                                {
                                    Username = user.UserName,
                                    UserId = user.Id,
                                    CreatedAt = DateTime.Now,
                                    Activity = user.IsAdmin ? "Admin logged in" : "User logged in"
                                });
                                Console.WriteLine("Welcome back " + user.UserName);
                                Console.ReadKey(true);
                                return user;
                            }
                            else
                            {
                                Console.WriteLine("No user found. ");
                                continue;
                            }
                        }
                    }

                    else if(menuInput == 'R')
                    {
                        while (true)
                        {
                            Console.WriteLine("Press 'q' to quit");
                            Console.Write("Enter your Username: ");
                            string? username = Console.ReadLine();
                            if (username == "q") break;

                            Console.Write("Enter your password: ");
                            string? password = Console.ReadLine();
                            if (password == "q") break;
                            var existingUser = db.Users.FirstOrDefault(u => u.UserName == username);

                            if (existingUser != null)
                            {
                                Console.WriteLine("The user allready exists. try with a different username");
                                Console.ReadKey(true);
                                continue;
                            }
                            else
                            {
                                if (username != null && password != null)
                                {
                                    User user = new User
                                    {
                                    
                                    UserName = username,
                                    Password = password,
                                    IsAdmin = false
                                    };
                                    db.Users.Add(user);
                                    db.SaveChanges();
                                    Console.WriteLine("user " + username + " has been registered.");
                                    Console.ReadKey(true);
                                    MongoQueries.InsertUserLog(new ModelsMDB.UserLog
                                    {
                                        Username = user.UserName,
                                        UserId = user.Id,
                                        CreatedAt = DateTime.Now,
                                        Activity = "Created account"
                                    });
                                    return user;
                                }
                            }
                        }
                        break;
                    }
                }
                return null;
            }

        }

        public static Customer PromptCustomer(MyAppContext db, User? currentUser)
        {
            Console.Clear();

            Customer customer;
            Address address;

            if (currentUser != null)
            {
                customer = db.Customers.Include(c => c.Adresses).ThenInclude(a => a.City).ThenInclude(cy => cy.Country)
                             .FirstOrDefault(c => c.UserId == currentUser.Id);


                if (customer != null && customer.Adresses.Any())
                {
                    return customer;
                }

                if(customer == null)
                {
                    customer = new Customer
                    {
                        UserId = currentUser.Id
                    };
                }

            }
            else
            {
                customer = new Customer();
            }

           
            while (true)
            {
                Console.Write("Enter your name: ");
                string? name = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(name))
                {
                    customer.Name = name;
                    break;
                }

                Console.WriteLine("Name cant be empty");
            }

            
            while (true)
            {
                Console.Write("Enter your phoneNumber: ");
                string? num = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(num) || !num.All(char.IsDigit))
                {
                    Console.WriteLine("Must be a valid number");
                    continue;
                }
                customer.PhoneNumber = num;
                break;
            }

            
            while (true)
            {
                Console.Write("Enter your email: ");
                string? email = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(email) || !email.Contains('@'))
                {
                    Console.WriteLine("Not a valid email");
                    continue;
                }
                customer.Email = email;
                break;
            }

           
            while (true)
            {
                Console.Write("Enter your age: ");
                string? ageInput = Console.ReadLine();
                if (!int.TryParse(ageInput, out int age) || age <= 14)
                {
                    Console.WriteLine("Not a valid age");
                    continue;
                }
                customer.Age = age;
                break;
            }

            
            string? country;
            while (true)
            {
                Console.Write("Enter country: ");
                country = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(country)) break;
                Console.WriteLine("Country cant be empty");
            }

            string? city;
            while (true)
            {
                Console.Write("Enter city: ");
                city = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(city)) break;
                Console.WriteLine("City cant be empty");
            }

            string? street;
            while (true)
            {
                Console.Write("Enter street: ");
                street = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(street)) break;
                Console.WriteLine("Street cant be empty");
            }


            address = new Address { Street = street };

            var existingCountry = db.Countries.FirstOrDefault(c => c.Name == country);
            if (existingCountry == null)
            {
                existingCountry = new Country { Name = country };
                db.Countries.Add(existingCountry);
                db.SaveChanges();
            }

            var existingCity = db.Cities.FirstOrDefault(c => c.Name == city && c.CountryId == existingCountry.Id);
            if (existingCity == null)
            {
                existingCity = new City { Name = city, CountryId = existingCountry.Id };
                db.Cities.Add(existingCity);
                db.SaveChanges();
            }

            address.City = existingCity;
            customer.Adresses.Add(address);


            if (!db.Customers.Any(c => c.Id == customer.Id))
                db.Customers.Add(customer);

            db.SaveChanges();

            MongoQueries.InsertActivityLog(new ModelsMDB.ActivityLog
            {
                CustomerId = customer.Id,
                Date = DateTime.Now,
                Activity = "New Customer"
            });

            return customer;
        }
    }
}
