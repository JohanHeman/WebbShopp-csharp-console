using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
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
    internal class PaymentQueries
    {


        public static void CartToCheckOut(MyAppContext db, User? currentUser)
        {
            try
            {
                var cart = db.Carts.Include(c => c.CartProducts).ThenInclude(cp => cp.Product).FirstOrDefault(c => !c.IsCheckedOut);

                if (cart == null || !cart.CartProducts.Any())
                {
                    Console.WriteLine("no items in cart, cant checkout");
                    return;
                }

                Customer customer = Helpers.PromptCustomer(db, currentUser);
                Address address = customer.Adresses.First();

                    Checkout checkout = new Checkout()
                    {
                        TotalAmount = cart.TotalAmount,
                        IsPaid = false,
                        Address = address,
                        Cart = cart,

                    };

                foreach (var cartProduct in cart.CartProducts)
                {
                    checkout.CheckoutProducts.Add(new CheckoutProduct
                    {
                        Quantity = cartProduct.Quantity,
                        Product = cartProduct.Product,
                        SoldAt = DateTime.Now
                    });
                }

                db.Checkouts.Add(checkout);
                db.SaveChanges(); 

                Console.Clear();

                checkout = db.Checkouts.Include(c => c.CheckoutProducts).ThenInclude(cp => cp.Product).First(c => c.Id == checkout.Id);


                Console.WriteLine("Your checkout contains: ");
                checkout.TotalAmount = checkout.TotalAmount * 1.25m;
                foreach (var item in checkout.CheckoutProducts)
                {
                    Console.WriteLine($"{item.Product.Name} Quantity: {item.Quantity} Price: {item.Product.Price}");
                }
                Console.WriteLine($"Total amount ink moms: {checkout.TotalAmount}");

                Console.WriteLine("'P' for payment options 'q' to quit");

                ConsoleKeyInfo key = Console.ReadKey(true);
                switch (key.KeyChar)
                {
                    case 'q':
                        Console.Clear();
                        return;
                    case 'p':
                        DeliveryAndPayment(db, checkout, cart, customer);
                        break;
                }
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


        public static void DeliveryAndPayment(MyAppContext db, Checkout checkout, Cart cart, Customer customer)
        {
            bool isPaid = false;
            bool hasFee = false;

            Console.Clear();
            try
            {
                List<ShippingMethod> deliveryOptions = db.ShippingMethods.ToList();

                List<string> windowList = new();
                foreach (var item in deliveryOptions)
                {
                    windowList.Add(item.Id.ToString() + item.Name.ToString() + ", price: " + item.Price.ToString());
                }

                var window = new Window("Delivery options", 1, 0, windowList);
                window.Draw();
                Console.WriteLine("Press '3' to go back.");
                ShippingMethod choseMethod;
                while (!isPaid)
                {

                    ConsoleKeyInfo key = Console.ReadKey();
                    if (int.TryParse(key.KeyChar.ToString(), out int input))
                    {
                        switch (input)
                        {
                            case 1:
                                choseMethod = db.ShippingMethods.Where(s => s.Id == input).FirstOrDefault();
                                if (!hasFee)
                                {
                                    checkout.TotalAmount += choseMethod.Price;

                                    hasFee = true;
                                }
                                break;
                            case 2:
                                choseMethod = db.ShippingMethods.Where(s => s.Id == input).FirstOrDefault();
                                if (!hasFee)
                                {
                                    checkout.TotalAmount += choseMethod.Price;
                                    checkout.ShippingMethod = choseMethod;
                                    hasFee = true;
                                }
                                break;
                            case 3:
                                Console.Clear();
                                return;
                        }
                    }
                    Console.Clear();

                    foreach (var item in checkout.CheckoutProducts)
                    {
                        Console.WriteLine($"{item.Product.Name} Quantity: {item.Quantity} Price: {item.Product.Price}");
                    }
                    Console.WriteLine($"Total amount ink moms and delivery fee: {checkout.TotalAmount}");

                    Console.WriteLine("'P' for payment options 'q' to quit");

                    ConsoleKeyInfo keyForPayment = Console.ReadKey(true);
                    switch (keyForPayment.KeyChar)
                    {
                        case 'q':
                            Console.Clear();
                            return;
                        case 'p':
                            Console.Clear();
                            if (Payment(db, checkout, cart, customer))
                                isPaid = true;
                            break;
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

        public static bool Payment(MyAppContext db, Checkout checkout, Cart cart, Customer customer)
        {
            try
            {
                PaymentMethod paymentMethod = new();
                List<PaymentMethod> options = db.PaymentMethods.ToList();
                List<string> windowList = new();
                foreach (var item in options)
                {
                    windowList.Add(item.Id.ToString() + " :" + item.Name.ToString());
                }

                var window = new Window("Payment options", 1, 0, windowList);
                window.Draw();

                ConsoleKeyInfo key = Console.ReadKey(true);

                Payment? createdPayment = null;

                if (int.TryParse(key.KeyChar.ToString(), out int input))
                {
                    switch (input)
                    {
                        case 1:
                            paymentMethod = db.PaymentMethods.Where(p => p.Id == input).FirstOrDefault();
                            Payment creditPayment = CreditCardPayment(checkout, customer, paymentMethod);
                            if (creditPayment != null)
                            {
                                db.Payments.Add(creditPayment);
                                CartQueries.EmptyCart(db, cart.Id);
                                createdPayment = creditPayment;
                            }
                            else
                            {
                                Console.WriteLine("Payment failed. ");
                                return false;
                            }

                            break;
                        case 2:
                            paymentMethod = db.PaymentMethods.Where(p => p.Id == input).FirstOrDefault();
                            Payment klarnaPayment = KlarnaPayment(checkout, customer, paymentMethod);
                            db.Payments.Add(klarnaPayment);
                            CartQueries.EmptyCart(db, cart.Id);
                            Console.WriteLine("The user is putting in Bank Id information...");
                            Thread.Sleep(1000);
                            createdPayment = klarnaPayment;
                            break;
                    }
                }



                db.SaveChanges();

                if(createdPayment != null)
                {
                    var log = new ModelsMDB.PaymentLog
                    {
                        PaymentId = createdPayment.Id,
                        Amount = createdPayment.Amount,
                        CustomerId = customer.Id,
                        CustomerName = customer.Name,
                        LoggedAt = DateTime.UtcNow
                    };
                    MongoQueries.InsertPaymentLog(log);
                }

                Console.WriteLine($"Thank you for your purchase {customer.Name}");
                Console.ReadKey();

                return true;

            }
            catch (DbException e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
                return false;
            }
        }


        public static Payment CreditCardPayment(Checkout checkout, Customer customer, PaymentMethod paymentMethod)
        {
            try
            {
                Payment payment = new();
                payment.Amount = checkout.TotalAmount;
                payment.CardholderName = customer.Name;
                payment.PaymentMethod = paymentMethod;
                payment.CheckOut = checkout;
                checkout.IsPaid = true;
                while (true)
                {

                    Console.WriteLine("Enter your last 4 credit card numbers");

                    string input = Console.ReadLine();
                    if (input.All(char.IsDigit))
                    {
                        payment.CardLastFour = input;
                    }
                    else
                    {
                        Console.WriteLine("Must be a valid credit card number");
                        continue;
                    }
                    break;
                }
                while (true)
                {
                    Console.Write("Enter the expirationdate: ");
                    string answer = Console.ReadLine();
                    if (answer.All(char.IsDigit) && answer.Length == 4)
                    {
                        payment.ExpirationDate = answer;
                    }
                    else
                    {
                        Console.WriteLine("Must be a valid month / day");
                        continue;
                    }

                    break;
                }

                return payment;
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
            return null;
        }

        public static Payment KlarnaPayment(Checkout checkout, Customer customer, PaymentMethod paymentMethod)
        {
            Payment payment = new Payment();
            payment.Amount = checkout.TotalAmount;
            payment.CheckOut = checkout;
            payment.PaymentMethod = paymentMethod;
            checkout.IsPaid = true;

            return payment;

        }


    }
}
