using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Webbshop.Models
{
    internal class PaymentMethod
    {

        public int Id { get; set; }
        public string Name { get; set; }

        public ICollection<Payment> Payments { get; set; } = new List<Payment>();
    }
}
