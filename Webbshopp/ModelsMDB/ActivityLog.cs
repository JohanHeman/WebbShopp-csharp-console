using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Webbshop.ModelsMDB
{
    internal class ActivityLog
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Activity { get; set; }
        public int CustomerId { get; set; }
        public DateTime Date { get; set; }
        public int? ProductId { get; set; }
    }
}
