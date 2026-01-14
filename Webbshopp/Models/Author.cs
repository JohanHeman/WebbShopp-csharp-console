using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Webbshop.Models
{
    internal class Author
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public ICollection<Product> products { get; set; } = new List<Product>();
    }
}
