using Webbshop.Models;
using WindowDemo;

namespace Webbshop
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            while(true)
            {
                Console.Clear();
                Console.WriteLine("Hello! do you want to login? Y/N");

                ConsoleKeyInfo key = Console.ReadKey(true);

                char inputChar = char.ToUpper(key.KeyChar);

                if(inputChar == 'Y')
                {
                    User user = Helpers.SignIn();

                    if (user == null)
                    {
                        continue;
                    }

                    await Menu.StartMenu(user);
                    break;

                }
                else if (inputChar == 'N')
                {
                    await Menu.StartMenu(null);
                    break;
                }

                Console.WriteLine("That is not a valid option! ");
                Console.ReadKey(true);
                continue;
                    
            }
        }
    }
}
