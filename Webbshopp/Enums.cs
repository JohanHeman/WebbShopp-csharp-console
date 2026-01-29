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
            Admin_access,
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
            Change_customer_info = 1
        }

        public enum adminProductEnums
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


        public enum adminSatistics
        {
            Most_sold_products = 1,
            sold_past_hour,
            biggest_customer_groups,
            Best_selling_categories
        }

        public enum ChangeCustomerInfo
        {
            Name = 1,
            Phone_number,
            Email,
            Age,
            address,
            Order_history,
            Delete_customer
        }
    }
}
