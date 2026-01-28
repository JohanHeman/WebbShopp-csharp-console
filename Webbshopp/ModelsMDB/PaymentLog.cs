using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Webbshop.ModelsMDB
{
    internal class PaymentLog
    {

        public string Id { get; set; } = Guid.NewGuid().ToString();
        public int PaymentId { get; set; }
        public decimal Amount { get; set; }
        public int CustomerId { get; set; }
        public string CustomerName { get; set; }
        public DateTime LoggedAt { get; set; }



    }
}
