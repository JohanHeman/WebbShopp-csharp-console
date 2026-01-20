using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Webbshop.Models;

namespace Webbshop.Connections
{
    internal class MyAppContext : DbContext
    {

        public DbSet<Address> Addresses { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Checkout> Checkouts { get; set; }
        public DbSet<CheckoutProduct> CheckoutProducts { get; set; }
        public DbSet<City> Cities { get; set; }
        public DbSet<Country> Countries { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<PaymentMethod> PaymentMethods { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ShippingMethod> ShippingMethods { get; set; }
        public DbSet<Supplier> Suppliers { get; set; }
        public DbSet<Author> Authors { get; set; }

        public DbSet<Cart> Carts { get; set; }

        public DbSet<CartProduct> CartProducts { get; set; }


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server=JOHANPC;Database=Webbshopp;Trusted_Connection=True;TrustServerCertificate=true");
        }
    }
}
