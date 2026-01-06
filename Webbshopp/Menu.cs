using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WindowDemo;

namespace Webbshopp
{
    internal class Menu
    {
        public static void DealMenu()
        {
            List<string> welcomeText = new List<string> { "Welcome to the Book store. ", "We provide books" };

            Window windowStart = new Window("The Book Shop", 37, 0, welcomeText);
            windowStart.Draw();

            List<string> dealOne = new List<string> { "Book name 1", "Author", "Press p to purchase" };
            var dealOneWindow = new Window("Deal 1", 10, 5, dealOne);
            dealOneWindow.Draw();

            List<string> dealTwo = new List<string> { "Book name 2", "Author", "Press p to purchase" };
            var dealTwoWindow = new Window("Deal 2", 40, 5, dealTwo);
            dealTwoWindow.Draw();

            List<string> dealThree = new List<string> { "Book name 3", "Author", "Press p to purchase" };
            var dealThreeWindow = new Window("Deal 3", 70, 5, dealThree);
            dealThreeWindow.Draw();
        }
    }
}
