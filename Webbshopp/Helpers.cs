using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }
}
