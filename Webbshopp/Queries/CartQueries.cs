using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Webbshop.Connections;
using Webbshop.Models;
using WindowDemo;

namespace Webbshop.Queries
{
    internal class CartQueries
    {


        public static void AddToCart(MyAppContext db, Product product, User? currentUser) // adds item to cart and either creates a new cart or uses existing, user as parameter for log function at the end (MongoDB)
        {
            if (product.InStock <= 0)
            {
                Console.WriteLine("This product is out of stock.");
                return;
            }

            try
            {
                if (currentUser != null)
                {
                    var customer = db.Customers.FirstOrDefault(c => c.UserId == currentUser.Id);
                    if (customer != null)
                    {
                        MongoQueries.InsertActivityLog(new ModelsMDB.ActivityLog
                        {
                            CustomerId = customer.Id,
                            Date = DateTime.Now,
                            ProductId = product.Id,
                            Activity = $"Added {product.Name} to cart"
                        });
                    }
                }

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
                    
                    db.SaveChanges();
                    Console.Clear();
                    Console.WriteLine("Succesfully added item to cart");
                    Console.ReadLine();
                    
                }
            }
            catch (DbException e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
            }
            catch (DbUpdateException e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
            }

        }

        public static void ShowCart(User? currentUser) 
        {
            Console.Clear();
            try
            {
                while (true)
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
                            cartList.Add($"Total ink moms: {cart.TotalAmount * 1.25m}");
                            var window = new Window("Cart", 2, 0, cartList);
                            window.Draw();

                            Console.WriteLine("press 'e' to empty cart");
                            Console.WriteLine("Press 'c' to go to checkout");
                            Console.WriteLine("Press 'u' to update a product on the cart");
                            Console.WriteLine("Press 'b' to go back");

                            ConsoleKeyInfo key = Console.ReadKey(true);

                            switch (key.KeyChar)
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
                                    PaymentQueries.CartToCheckOut(db, currentUser);
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

            catch (DbException e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
            }
            catch (DbUpdateException e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
            }
        }

        public static void EmptyCart(MyAppContext db, int id) 
        {
            Console.Clear();
            try
            {
                var cart = db.Carts.FirstOrDefault(c => c.Id == id);

                cart.CartProducts.Clear();
                cart.TotalAmount = 0;

                db.SaveChanges();
            }
            catch (DbException e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
            }
            catch (DbUpdateException e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
            }
        }

        public static void UpdateCart(MyAppContext db, int id) // either update the quantity of the item or delete it 
        {
            Console.Clear();

            try
            {
                var cart = db.Carts.Include(c => c.CartProducts).ThenInclude(cp => cp.Product).FirstOrDefault(c => !c.IsCheckedOut);

                while (true)
                {
                    foreach (var item in cart.CartProducts)
                    {
                        Console.WriteLine(item.Id + ": " + item.Product.Name);
                    }
                    Console.WriteLine("Enter the ID of the product you want to update (or 'q' to quit):");
                    string? inputId = Console.ReadLine();

                    if (inputId != null && inputId.Equals("q", StringComparison.OrdinalIgnoreCase))
                    {
                        Console.Clear();
                        return;
                    }

                    if (int.TryParse(inputId, out int answer))
                    {
                        var currentProduct = db.CartProducts.Include(cp => cp.Product).FirstOrDefault(cp => cp.Id == answer);

                        if (currentProduct != null)
                        {
                            Console.WriteLine("What do you want to do? ");
                            Console.WriteLine("1: Update quantity\n2: delete item\n3: go back");
                            ConsoleKeyInfo key = Console.ReadKey(true);

                            if (int.TryParse(key.KeyChar.ToString(), out int input))
                            {
                                switch (input)
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
                                Console.WriteLine("Wrong input. Press any key to try again.");
                                Console.ReadKey(true);
                                Console.Clear();
                               
                            }
                            break; 
                        }
                        else
                        {
                            Console.WriteLine("Product with that ID does not exist in your cart. Press any key to try again.");
                            Console.ReadKey(true);
                            Console.Clear();
                        }
                    }
                    else
                    {
                        Console.WriteLine("Invalid input. Please enter a number or 'q'. Press any key to try again.");
                        Console.ReadKey(true);
                        Console.Clear();
                    }
                } 
            }
            catch (DbException e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
            }
            catch (DbUpdateException e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
            }
        }

        public static void UpdateCartItem(MyAppContext db, CartProduct currentProduct) // Update the quantity of the item 
        {
            Console.Clear();
            Console.WriteLine("Current quantity: " + currentProduct.Quantity);

            while (true) 
            {
                try
                {
                    Console.Write("Enter the amount you would like to change it to (or 'q' to quit): ");
                    string? input = Console.ReadLine();

                    if (input != null && input.Equals("q", StringComparison.OrdinalIgnoreCase))
                    {
                        Console.Clear();
                        return;
                    }

                    if (int.TryParse(input, out int quant))
                    {
                        if (quant <= 0)
                        {
                            Console.WriteLine("Cannot set quantity to less than 1. Press any key to try again.");
                            Console.ReadKey(true);
                            Console.Clear(); 
                            continue;
                        }

                        currentProduct.Product.InStock += currentProduct.Quantity; // restores the instock for product


                        int availableStock = currentProduct.Product.InStock;
                        if (quant > availableStock)
                        {
                            Console.WriteLine($"Cannot set quantity to {quant}. Only {availableStock} available. Press any key to try again.");
                            Console.ReadKey(true);
                            Console.Clear();
                            continue;
                        }

                        currentProduct.Cart.TotalAmount -= currentProduct.Product.Price * currentProduct.Quantity;
                        currentProduct.Quantity = quant; // changes the quantity of the chosen product
                        currentProduct.Product.InStock -= quant; // updates the new instock value
                        currentProduct.Cart.TotalAmount += currentProduct.Product.Price * quant;

                        db.SaveChanges();

                        Console.WriteLine("Cart succesfully updated!");
                        Console.ReadKey(true);
                        Console.Clear();
                        break; 
                    }
                    else
                    {
                        Console.WriteLine("Not a valid value. Press any key to try again.");
                        Console.ReadKey(true);
                        Console.Clear(); 
                    }
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

    }
}
