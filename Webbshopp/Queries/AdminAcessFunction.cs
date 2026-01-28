using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.Marshalling;
using System.Text;
using System.Threading.Tasks;
using Webbshop.Connections;

namespace Webbshop.Queries
{
    internal class AdminAcessFunction
    {
        // just a function that handles the adminacess for users.
        public static void AdminAcess()
        {
            using (var db = new MyAppContext())
            {
                var users = db.Users.ToList();

                foreach(var u in users)
                {
                    Console.WriteLine($"{u.Id}: {u.UserName} {(u.IsAdmin ? "Admin" : "Not Admin")}");
                }

                while (true)
                {
                    Console.WriteLine("Enter the user id to view or 'Q' to quit");
                    var input = Console.ReadLine().ToUpper();

                    if (string.Equals(input, "Q", StringComparison.OrdinalIgnoreCase))
                    {
                        break;
                    }

                    if (int.TryParse(input, out int userId))
                    {
                        var selectedUser = users.FirstOrDefault(u => u.Id == userId);
                        if (selectedUser != null)
                        {
                            Console.Clear();
                            Console.WriteLine($"{selectedUser.Id}: {selectedUser.UserName} {(selectedUser.IsAdmin ? "Admin" : "Not Admin")}");
                            Console.WriteLine("What do you want to do? 'D' to delete user, 'A' to change admin access press 'Q' to quit");
                            ConsoleKeyInfo key = Console.ReadKey(true);
                            char inputChar = char.ToUpper(key.KeyChar);
                            
                            if(inputChar == 'Q')
                            {
                                return;
                            }

                            if(inputChar == 'D')
                            {
                                try
                                {
                                    db.Users.Remove(selectedUser);
                                    db.SaveChanges();
                                    Console.WriteLine($"User {selectedUser.UserName} is deleted.");
                                    break;
                                }
                                catch(DbUpdateException e)
                                {
                                    Console.WriteLine("Something went wrong");
                                    Console.WriteLine(e.StackTrace);
                                }
                            }
                            else if(inputChar == 'A')
                            {
                                if(selectedUser.IsAdmin)
                                {
                                    selectedUser.IsAdmin = false;
                                    db.SaveChanges();
                                    Console.WriteLine($"User {selectedUser.UserName} is now {(selectedUser.IsAdmin ? "Admin" : "Not Admin")}.");

                                }
                                else
                                {
                                    selectedUser.IsAdmin = true;
                                    db.SaveChanges();
                                    Console.WriteLine($"User {selectedUser.UserName} is now {(selectedUser.IsAdmin ? "Admin" : "Not Admin")}.");

                                }
                            }
                            Console.ReadKey(true);
                            break;
                        }
                        else
                        {
                            Console.WriteLine("No user with that id.");
                            Console.ReadKey(true);
                        }
                    }
                    else
                    {
                        Console.WriteLine("Not a valid input.");
                        Console.ReadKey(true);
                    }
                }


            }


        }
    }
}
