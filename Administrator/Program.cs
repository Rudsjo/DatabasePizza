using System;

namespace Administrator
{
    class Program
    {
        static void Main(string[] args)
        {

            int nrOfChoices = PrintMenu(Menus.menuChoices[1]);
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

            for(int i = 1; i <= nrOfChoices; i++)
            {
                if(userChoiceAsInt == i)
                {
                    Console.Clear();
                    PrintMenu(menuChoice[i+1]);
                }
            }
        }
    }
}
