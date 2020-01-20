using System;
using System.Threading;

namespace ArrayOfTuple
{
    class Program
    {
        static void Main(string[] args)
        {

            Menus.logIn();

        }

        public static void RunMainMenu()
        {
            int numberOfChoices = PrintMenu(Menus.menuChoices, "main").Item1;
            Console.Clear();
            string currentPosition = PrintMenu(Menus.menuChoices, "main").Item2;
            string formerPositon = currentPosition;
            int choice = Console.ReadKey(true).KeyChar - '0';

            while (true)
            {
                formerPositon = currentPosition;
                currentPosition = UserChoice(choice, numberOfChoices, Menus.menuChoices, currentPosition);

                if (currentPosition == "error")
                {
                    Console.Clear();
                    Console.WriteLine("Ditt val är felaktigt.");
                    Thread.Sleep(1500);
                    Console.Clear();
                    PrintMenu(Menus.menuChoices, formerPositon);
                    choice = Console.ReadKey(true).KeyChar - '0';
                }

                else if (currentPosition == "logout")
                {
                    Console.Clear();
                    Menus.logIn();
                    break;
                }

                else
                {
                    Console.Clear();
                    numberOfChoices = PrintMenu(Menus.menuChoices, currentPosition).Item1;
                    Console.Clear();
                    currentPosition = PrintMenu(Menus.menuChoices, currentPosition).Item2;
                    choice = Console.ReadKey(true).KeyChar - '0';
                }

            }
        }

        public static (int, string) PrintMenu(string[][] menuChoice, string currentPosition)
        {
            int counter = 1;

            for (int i = 0; i < menuChoice.Length; i++)
            {
                if (menuChoice[i][0] == currentPosition)
                {
                    for (int menuItems = 1; menuItems < menuChoice[i].Length; menuItems++)
                    {
                        Console.WriteLine(counter + ". " + menuChoice[i][menuItems]);
                        counter++;
                    }
                }
            }

            return (counter, currentPosition);
        }

        public static string UserChoice(int userChoice, int nrOfChoices, string[][] menuChoice, string currentPosition)
        {
            string newPosition = currentPosition;

            for (int i = 1; i < nrOfChoices; i++)
            {
                if (userChoice == i && userChoice < (nrOfChoices - 1))
                {
                    newPosition = menuChoice[i][0];
                }

                else if (userChoice == (nrOfChoices - 1) && currentPosition == "main")
                {
                    // kontrollerar om vi befinner oss i huvudmenyn och således om vi ska logga ut, vid övriga går vi tillbaka till huvudmenyn
                    newPosition = "logout";
                }

                else if(userChoice == (nrOfChoices - 1))
                {
                    newPosition = "main";
                }

                else if (userChoice >= nrOfChoices)
                {
                    newPosition = "error";
                }
            }

            return newPosition;
        }
    }
}
