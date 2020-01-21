using System;
using System.Threading;
using MenuMethods;

namespace Administrator
{
    class Program
    {
        static void Main(string[] args)
        {
            
            bool checkLogIn = Repository.logIn();

            Repository rep = new Repository();

            if(checkLogIn == true)
            {
                string chosenPosition = MenuFuncs.RunMainMenu(Menus.menuChoices, rep, "logIn");

                if(chosenPosition == "users")
                {
                    int userChoice = Console.ReadKey(true).KeyChar - '0';

                    switch (userChoice)
                    {
                        case 1:
                            rep.AddEmployee("AddEmployee");
                            break;
                    }
                }
            }

        }
    }
}
