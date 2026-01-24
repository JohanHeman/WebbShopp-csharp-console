using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Webbshop
{
    internal class Enums
    {

        public enum HomeEnums
        {
            Customer_menu = 1,
            Admin_menu
        }

        public enum customerEnums
        {
            Shop = 1,
            Shoppingcart,
            Home,
        }

        public enum adminEnums
        {
            Product_categories = 1,
            Add_product,
            Customer_management,
            Statistics,
            supplier,
            Home
        }


        public enum shopEnums
        {
            Search = 1,
            Categories,
            Back
        }


        public enum adminCustomerEnums
        {
            Change_customer_info = 1,
            Delete_customer,
            Order_history
        }

        public enum AdminProductEnums
        {
            name = 1,
            info,
            price,
            category,
            instock,
            change_supplier,
            Is_displayed,
            Delete_product
        }

    }
}
