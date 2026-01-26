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
        }
    }
}
