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
            Product_management = 1,
            Product_categories,
            Customer_management,
            Statistics,
            Home
        }

        public enum productEnums
        {
            Add_product = 1,
            Delete_product,
            Update_product,
            Home
        }

        public enum categoryEnums
        {
            Adventure = 1,
            Fantasy,
            Mystery,
            Thriller,
            History,
            Self_help
        }

        public enum shopEnums
        {
            Search = 1,
            Categories,
            Home
        }


        public enum adminCustomerEnums
        {
            // no add customer here, because the customers are added when they are doing their orders. 
            // what infor should admin be able to edit for the customer ? 
            Change_customer_info = 1,
            Order_history
        }

    }
}
