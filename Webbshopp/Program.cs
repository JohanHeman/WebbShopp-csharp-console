using Webbshop.Models;
using WindowDemo;

namespace Webbshop
{
    internal class Program
    {
        static async Task Main(string[] args)
        {

            Console.WriteLine("Hello! do you want to login? Y/N");

            ConsoleKeyInfo key = Console.ReadKey(true);

            char inputChar = char.ToUpper(key.KeyChar);

            if(inputChar == 'Y')
            {
                User user = Helpers.SignIn();
                await Menu.StartMenu(user);

            }
            else
            {
                await Menu.StartMenu(null);
            }
            // to dos 
            // make it so admin account can change other users to admin if he wants to ( a new function)
            // find more statistic querries to 
            //customer crud querries
            // make it so only admin menu can be if user is admin
            //mongodb
        }
    }
}
