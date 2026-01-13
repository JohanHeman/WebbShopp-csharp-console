using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Webbshop.Models
{
    internal class Payment
    {
        public int Id { get; set; }
        public decimal Amount { get; set; }
        public string CardholderName { get; set; }
        public string CardLastFour { get; set; }
        public string ExpirationDate { get; set; }
        public int? CheckOutId { get; set; }

        public int PaymentMethodId { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
        public Checkout? CheckOut { get; set; }
    }
}
