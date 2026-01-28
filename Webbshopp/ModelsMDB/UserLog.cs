using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace Webbshop.ModelsMDB
{
    internal class UserLog
    {

        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Username { get; set; }
        public int UserId { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Activity { get; set; }
    }
}
