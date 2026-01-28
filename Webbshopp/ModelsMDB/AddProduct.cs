using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Webbshop.ModelsMDB
{
    internal class AddProduct
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public int ProductId { get; set; }
        public DateTime Date { get; set; }
    }
}
