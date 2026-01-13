using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Webbshop.Models
{
    internal class Adress
    {
        public int Id { get; set; }
        public string Street { get; set; }


        public int CityId { get; set; }
        public City City { get; set; }

        public ICollection<Checkout> Checkouts { get; set; } = new List<Checkout>();

        public ICollection<Customer> Customers { get; set; } = new List<Customer>();
    }
}
