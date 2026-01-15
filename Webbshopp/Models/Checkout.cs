using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Webbshop.Models
{
    internal class Checkout
    {
        public int Id { get; set; }
        public decimal TotalAmount { get; set; }
        public int ShippingMethodId { get; set; }
        public bool IsPaid { get; set; }
        public int AdressId { get; set; }
        public Address Adress { get; set; }
        public ShippingMethod ShippingMethod { get; set; }
        public Payment? Payment { get; set; } // navigation for payment one to one relationship
        public ICollection<CheckoutProduct> CheckoutProducts { get; set; } = new List<CheckoutProduct>();
    }
}
