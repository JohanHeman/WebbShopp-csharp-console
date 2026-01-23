using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WindowDemo;

namespace Webbshop.Queries
{
    internal class AdminCustomer
    {

        public static void AdminCustomerOperations()
        {
            Console.Clear();
            List<string> productList = Helpers.EnumsToLists(typeof(Enums.adminCustomerEnums));
            var window = new Window("AdminMenu", 2, 0, productList);
            window.Draw();
            // add customer functions here for admin (update, delete)
            // ask message, deleting this customer will delete all orders tracked with this customer, continue? y / n 
        }

    }
}
