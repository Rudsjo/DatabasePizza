using System;

namespace Administrator
{
    class Program
    {
        static void Main(string[] args)
        {

            int nrOfChoices = PrintMenu(Menus.menuChoices[0]);
            char key = Console.ReadKey(true).KeyChar;
            UserChoice(key, nrOfChoices, Menus.menuChoices);

        }

        public static int PrintMenu(string[] menuItems)
        {
            int counter = 1;

            for (int i = 0; i < menuItems.Length; i++)
            {
                Console.WriteLine(counter + ". " + menuItems[i]);
                counter++;
            }

            return counter;
        }

        public static void UserChoice(char userChoice, int nrOfChoices, string[][] menuChoice)
        {
            int userChoiceAsInt = userChoice - '0';

            for(int i = 1; i < nrOfChoices; i++)
            {
                if(userChoiceAsInt == i && userChoiceAsInt < nrOfChoices - 1)
                {
                    Console.Clear();
                    PrintMenu(menuChoice[i]);
                }

                else if(userChoiceAsInt == nrOfChoices - 1)
                {
                    Console.Clear();
                    return;
                    //PrintMenu(menuChoice[userChoiceAsInt - 1]);
                }

                else if(userChoiceAsInt >= nrOfChoices)
                {
                    Console.Clear();
                    Console.WriteLine("Ditt val är felaktigt. Försök igen.");
                    System.Threading.Thread.Sleep(1500);
                    Console.Clear();
                    nrOfChoices = PrintMenu(menuChoice[0]);
                    userChoice = Console.ReadKey().KeyChar;
                    Console.Clear();
                    UserChoice(userChoice, nrOfChoices, menuChoice);
                }
            }
        }
    }
}
