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
using Webbshop.Migrations;
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
                        List<string> bookWindow = new List<string> { book.Information , book.Price.ToString() + "$" + " In stock: " + book.InStock + " press c to add to cart."}
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


        public static void AddToCart(MyAppContext db, Product product)
        {
            if (product.InStock <= 0)
            {
                Console.WriteLine("This product is out of stock.");
                return;
            }
            
            try
            {
                var checkCart = db.Carts.Include(c => c.CartProducts).FirstOrDefault(c => !c.IsCheckedOut);

                if (checkCart != null)
                {
                    var cartProduct = checkCart.CartProducts.FirstOrDefault(cp => cp.ProductId == product.Id);

                    if (cartProduct != null)
                    {
                        cartProduct.Quantity++;
                    }
                    else
                    {
                        checkCart.CartProducts.Add(new CartProduct
                        {
                            ProductId = product.Id,
                            Quantity = 1
                        });
                    }

                    checkCart.TotalAmount += product.Price;
                    product.InStock--;
                    db.SaveChanges();
                    Console.Clear();
                    Console.WriteLine("Succesfully added item to cart");
                    Console.ReadLine();
                }
                else
                {

                    checkCart = new Cart();
                    db.Carts.Add(checkCart);
                    checkCart.TotalAmount += product.Price;
                    product.InStock--;

                    checkCart.CartProducts.Add(new CartProduct
                    {
                        ProductId = product.Id,
                        Quantity = 1
                    }); 
                    {
                        db.SaveChanges();
                        Console.Clear();
                        Console.WriteLine("Succesfully added item to cart");
                        Console.ReadLine();
                    }
                }
            }
            catch(DbException e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
            }
            catch(DbUpdateException e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
            }
          
        }

        public static void ShowCart()
        {
            Console.Clear();
            try
            {
                while(true)
                {
                    using (var db = new MyAppContext())
                    {
                        var cart = db.Carts.Include(c => c.CartProducts).ThenInclude(cp => cp.Product).FirstOrDefault(c => !c.IsCheckedOut);

                        if (cart == null || !cart.CartProducts.Any())
                        {
                            Console.WriteLine("The cart is empty");
                            Console.ReadKey();
                            break;
                        }
                        else
                        {
                            List<string> cartList = new();
                            foreach (var item in cart.CartProducts)
                            {
                                cartList.Add($"{item.Product.Name} | Price: {item.Product.Price}$ | Quantity: {item.Quantity}");

                            }
                            cartList.Add($"Total: {cart.TotalAmount}");
                            var window = new Window("Cart", 2, 0, cartList);
                            window.Draw();

                            Console.WriteLine("press 'e' to empty cart");
                            Console.WriteLine("Press 'c' to go to checkout");
                            Console.WriteLine("Press 'u' to update a product on the cart");
                            Console.WriteLine("Press 'b' to go back");

                            ConsoleKeyInfo key = Console.ReadKey(true);

                            switch(key.KeyChar)
                            {
                                case 'e':
                                    EmptyCart(db, cart.Id);
                                    Console.Clear();
                                    Console.WriteLine("The cart has been cleared.");
                                    break;
                                case 'u':
                                    UpdateCart(db, cart.Id);
                                    Console.Clear();
                                    break;
                                case 'b':
                                    return;
                                case 'c':
                                    Console.Clear();
                                    CartToCheckOut(db);
                                    break;

                                default:
                                    Console.Clear();
                                    Console.WriteLine("Wrong input, try again ? ");
                                    Console.ReadKey(true);
                                    Console.Clear();
                                    break;
                            }
                        }
                    }
                    Console.ReadKey(true);
                }
            }

            catch(DbException e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
            }
            catch(DbUpdateException e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
            }
        }

        public static void EmptyCart(MyAppContext db, int id)
        {
            try
            {
                var cart = db.Carts.FirstOrDefault(c => c.Id == id);

                if (cart != null)
                {
                    db.Carts.Remove(cart);
                    db.SaveChanges();
                    Console.Clear();
                }
                else
                {
                    Console.Clear();
                    Console.WriteLine("Something went wrong...");
                }
            }
            catch(DbException e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
            }
            catch(DbUpdateException e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace); 
            }
        }

        public static void UpdateCart(MyAppContext db, int id)
        {
            Console.Clear();

            try
            {
                var cart = db.Carts.Include(c => c.CartProducts).ThenInclude(cp => cp.Product).FirstOrDefault(c => !c.IsCheckedOut);

                foreach(var item in cart.CartProducts)
                {
                    Console.WriteLine( item.Id +": " +  item.Product.Name);
                }

                Console.WriteLine("enter the id of the product you want to update");
                if (int.TryParse(Console.ReadLine(), out int answer))
                {
                    // change the product matching the id 
                    // update properties like the quantity in the cart, the products in stock value, and the price in the cart


                    var currentProduct = db.CartProducts.Include(cp => cp.Product).FirstOrDefault(cp => cp.Id == answer);

                    

                    if(currentProduct != null)
                    {
                        Console.WriteLine("What do you want to do? ");
                        Console.WriteLine("1: Update quantity\n2: delete item\n3: go back");
                        ConsoleKeyInfo key = Console.ReadKey(true);

                        if(int.TryParse(key.KeyChar.ToString(), out int input))
                        {
                            switch(input)
                            {
                                case 1:
                                    UpdateCartItem(db, currentProduct);
                                    break;
                                case 2:
                                    DeleteCartItem(db, currentProduct);
                                    break;
                                case 3:
                                    return;
                            }
                        }
                        else
                        {
                            Console.WriteLine("Wrong input");
                        }
                    }
                }
            }
            catch(DbException e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
            }
            catch(DbUpdateException e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
            }
        }

        public static void UpdateCartItem(MyAppContext db , CartProduct currentProduct)
        {
            Console.Clear();
            Console.WriteLine("Current quantity: " + currentProduct.Quantity);


            try
            {



                Console.Write("Enter the amount you would like to change it to: ");
                if (int.TryParse(Console.ReadLine(), out int quant))
                {

                    currentProduct.Product.InStock += currentProduct.Quantity; // restores the instock for product


                    int availableStock = currentProduct.Product.InStock;
                    if (quant > availableStock)
                    {
                        Console.WriteLine($"Cannot set quantity to {quant}. Only {availableStock} available.");
                        return;
                    }

                    currentProduct.Cart.TotalAmount -= currentProduct.Product.Price * currentProduct.Quantity;
                    currentProduct.Quantity = quant; // changes the quantity of the chosen product
                    currentProduct.Product.InStock -= quant; // updates the new instock value 
                    currentProduct.Cart.TotalAmount += currentProduct.Product.Price * quant;

                    db.SaveChanges();

                    Console.WriteLine("Cart succesfully update!");
                    Console.ReadKey(true);
                    Console.Clear();
                }
                else
                {
                    Console.WriteLine("Not valid value");
                }
            }
            catch (DbUpdateException e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
            }
            catch(DbException e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
            }
        }


        public static void DeleteCartItem(MyAppContext db, CartProduct currentProduct)
        {
            Console.Clear();
            try
            {
                currentProduct.Product.InStock += currentProduct.Quantity;

                currentProduct.Cart.TotalAmount -= currentProduct.Product.Price * currentProduct.Quantity;

                db.CartProducts.Remove(currentProduct);
                db.SaveChanges();
                Console.WriteLine("Succesfully deleted item from cart");
                Console.ReadKey(true);
            }
            catch (DbUpdateException e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
            }
            catch (DbException e)
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
                    InfoBook(id);
                }
            }

            else
            {
                Console.WriteLine("No books found");
            }
                Console.ReadKey(true);
        }

        public static void CartToCheckOut(MyAppContext db)
        {
            try
            {
                var cart = db.Carts.Include(c => c.CartProducts).FirstOrDefault(c => !c.IsCheckedOut);

                Customer customer;
                Address address;

                Console.Write("Enter your name: ");
                string name = Console.ReadLine();

                Console.Write("Enter country: ");
                string country = Console.ReadLine().Trim();

                Console.Write("Enter city: ");
                string city = Console.ReadLine().Trim();

                Console.Write("Enter city street");
                string street = Console.ReadLine().Trim();

                var existingUser = db.Customers.Include(c => c.Adresses).FirstOrDefault(c => c.Name == name && c.Adresses.Any(a => a.Street == street));

                if (existingUser == null)
                {
                    while (true)
                    {
                        customer = new Customer();
                        customer.Name = name;

                        Console.Write("Enter your phoneNumber:");
                        string num = Console.ReadLine();

                        if (!num.All(char.IsDigit))
                        {
                            Console.WriteLine("must be a valid number");
                            continue;

                        }
                        customer.PhoneNumber = num;

                        Console.Write("Enter your email: ");
                        string email = Console.ReadLine();
                        if (!email.Contains('@'))
                        {
                            Console.WriteLine("Not a valid email. ");
                            continue;

                        }
                        customer.Email = email;


                        Console.Write("Enter your age");
                        if (!int.TryParse(Console.ReadLine(), out int age))
                        {
                            Console.WriteLine("Not a valid age");
                            continue;

                        }
                        customer.Age = age;

                        address = new Address();
                        address.Street = street;

                        var existingCountry = db.Countries.FirstOrDefault(c => c.Name == country);

                        if (existingCountry == null)
                        {
                            existingCountry = new Country { Name = country };
                            db.Countries.Add(existingCountry);
                            db.SaveChanges();
                        }

                        var existingCity = db.Cities.FirstOrDefault(c => c.Name == city.Trim() && c.CountryId == existingCountry.Id);
                        if (existingCity == null)
                        {
                            existingCity = new City { Name = city.Trim(), CountryId = existingCountry.Id };
                            db.Cities.Add(existingCity);
                            db.SaveChanges();
                        }

                        address.City = existingCity;
                        customer.Adresses.Add(address);
                        db.Customers.Add(customer);
                        db.SaveChanges();
                        Console.WriteLine("Customer is succesfully added! Now moving on to the address");
                        break;
                    }
                }
                else
                {
                    customer = existingUser;
                    address = existingUser.Adresses.First(a => a.Street == street);
                    Console.WriteLine("Existing customer found. Moving to checkout...");
                    Thread.Sleep(1000);
                }
            

            // now create a new checkout 
                Checkout checkout = new Checkout()
                {
                    Cart = cart,
                    Address = address,
                    IsPaid = false,
                    TotalAmount = cart.TotalAmount
                };


                foreach (var cp in cart.CartProducts)
                {
                    checkout.CheckoutProducts.Add(new CheckoutProduct
                    {
                        Product = cp.Product, 
                        Quantity = cp.Quantity,
                        ProductId = cp.ProductId
                    });
                }

                db.Checkouts.Add(checkout);

                EmptyCart(db, cart.Id);
                cart.IsCheckedOut = true;

                db.SaveChanges();

                Console.WriteLine("Yopur checkout contains: ");

                foreach (var item in checkout.CheckoutProducts)
                {
                    Console.WriteLine($"{item.Product.Name} Quantity: {item.Quantity} Price: {item.Product.Price}");
                }
                Console.WriteLine($"Total amount: {checkout.TotalAmount:C}");

                Console.ReadKey(true);
            }
            catch(DbException e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
            }
            catch(DbUpdateException e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
            }
        }
    }
}








// go through all the try catches and see if they are correct 
// understand how to use the try catch blocks propperly 
// continue with the checkout / payment query 
// start doing admin query but do them with async 
// then focus in making mongodb 

// CHECK ALL TRY CATCHES AND SUROUND THEM IN MINIMAL BLOCKS IMPORTANT


