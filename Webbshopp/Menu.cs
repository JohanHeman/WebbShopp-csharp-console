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
        public static void StartMenu()
        {
            List<string> welcomeText = new List<string> { "Welcome to the Book store. ", "We provide books" };

            Window windowStart = new Window("The Book Shop", 2, 1, welcomeText);
            windowStart.Draw();

            List<string> dealOne = new List<string> { "Book name", "Author", "Press p to purchase" };
            var dealOneWindow = new Window("Deal 1", 30, 5, dealOne);
            dealOneWindow.Draw();

        }

    }
}
