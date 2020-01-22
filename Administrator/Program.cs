using System;
using System.Threading;
using MenuFunctions;
using MSSQLRepository;

namespace Administrator
{
    class Program
    {
        static void Main(string[] args)
        {
            MenuFuncs menu = new MenuFuncs();
            MSSQL rep = new MSSQL();

            bool checkLogIn = Repository.logIn(menu, "RunMainMenu", Menus.menuChoices, rep, "logIn");



            if (checkLogIn == true)
            {
                string chosenPosition = MenuFuncs.RunMainMenu(Menus.menuChoices, rep, "logIn").Item1;
                //Console.Clear();
                //int userChoice = MenuFuncs.RunMainMenu(Menus.menuChoices, rep, "logIn").Item2;

                if (chosenPosition == "users")
                {
                    //Console.Clear();
                    //int userChoice = MenuFuncs.RunMainMenu(Menus.menuChoices, rep, "logIn").Item2;

                    int userChoice = Console.ReadKey(true).KeyChar - '0';
                    switch (userChoice)
                    {
                        case 1:
                            {
                                rep.AddEmployee("AddEmployee");                               
                                break;
                            }
                        case 4:
                            {                                
                                Console.Clear();
                                foreach (var employee in rep.ShowEmployees("ShowEmployees"))
                                {
                                    Console.WriteLine(employee);
                                }
                                Console.ReadKey();

                                break;
                            }

                    }
                }
            }

        }
    }
}
