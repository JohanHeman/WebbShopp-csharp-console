using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Webbshop.Models
{
    internal class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public int InStock { get; set; }
        public string Information { get; set; }
        public bool IsDisplayed { get; set; }
        public int CategoryId { get; set; }
        public Category Category { get; set; }
        public int SupplierId { get; set; }
        public Supplier Supplier { get; set; }
        public int AuthorId { get; set; }
        public Author Author { get; set; }



        public ICollection<CheckoutProduct> CheckoutProducts { get; set; } = new List<CheckoutProduct>();
        public ICollection<CartProduct> CartProducts { get; set; } = new List<CartProduct>();

    }
}
