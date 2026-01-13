using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Webbshop.Models
{
    internal class CheckoutProduct
    {
        public int Id { get; set; }
        public int Quantity { get; set; }
        public int CheckoutId { get; set; }
        public Checkout Checkout { get; set; }
        public int ProductId { get; set; }
        public Product Product { get; set; }
    }
}
