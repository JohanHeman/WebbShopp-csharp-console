using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Webbshop.Models;
using Webbshop.Queries;
using WindowDemo;

namespace Webbshop
{
    internal class Helpers
    {
        public static List<string> EnumsToLists(Type e) // using type as parameter 
        {
            List<string> theList = new List<string>();

            if(!e.IsEnum)
            {
                throw new ArgumentException("The function expects an enum type!");
            }

            var enumValues = Enum.GetValues(e);
            
            foreach ( var value in enumValues )
            {
                string name = value.ToString().Replace("_", " ");

                theList.Add((int)value + ": " + value.ToString());
            }
            return theList;   
        }

        public static Window ShowCategories(List<Category> cList)
        {
            cList = DapperQueries.GetCategories();
            Console.Clear();

            List<string> windowList = new List<string>();

            foreach (var c in cList)
            {
                windowList.Add(c.Id + ": " + c.Name);
            }

            var window = new Window("Categories", 1, 0, windowList);
            return window;
        }



    }
}
