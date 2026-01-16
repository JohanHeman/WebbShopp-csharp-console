using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.InteropServices.Marshalling;
using System.Text;
using System.Threading.Tasks;
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
            catch(Exception e)
            {
                Console.WriteLine("Something went wrong. " + e.Message);
            }
        }


        public static void AddToCart(MyAppContext db, Product product)
        {

            

            if(product.InStock > 0)
            {
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

                        db.SaveChanges();

                        checkCart.CartProducts.Add(new CartProduct
                        {
                            ProductId = product.Id,
                            Quantity = 1
                        });

                        db.SaveChanges();
                        Console.Clear();
                        Console.WriteLine("Succesfully added item to cart");
                        Console.ReadLine();
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
            else
            {
                Console.WriteLine("This product is out of stock.");
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

                            ConsoleKeyInfo key = Console.ReadKey(true);
                            if (key.KeyChar == 'e')
                            {
                                EmptyCart(db,cart.Id);
                                Console.Clear();
                                Console.WriteLine("The cart has been cleared.");
                                break;
                            }
                            else
                            {
                                Console.Clear();
                                Console.WriteLine("Wrong input, try again ? ");
                                Console.Clear();
                            }
                        }
                    }
                    Console.ReadKey(true);
                }
            }

            catch(Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }


        public static void EmptyCart(MyAppContext db, int id)
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
    }
}
