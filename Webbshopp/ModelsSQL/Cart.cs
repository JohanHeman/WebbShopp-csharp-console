using Microsoft.EntityFrameworkCore.Diagnostics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Webbshop.Models
{
    internal class Cart
    {
        public int Id { get; set; }
        public decimal TotalAmount { get; set; }
        public bool IsCheckedOut { get; set; } = false;
        public ICollection<CartProduct> CartProducts { get; set; } = new List<CartProduct>();
    }
}
