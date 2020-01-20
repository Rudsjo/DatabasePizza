using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace ArrayOfTuple
{
    public class Menus
    {
        /** 
         * Denna klass innehåller samtliga stringarrayer med menyval för de menyer som tillhör admin-programmet
         **/


        public static string[][] menuChoices =
        {
            // main menu (0)
            new string[] { "main", "Användare", "Pizzor", "Pålägg", "Tillbehör", "Gamla ordrar", "Logga ut" },
            // anställda (1)
            new string[] { "users", "Lägg till", "Ta bort", "Ändra", "Lista på anställda", "Gå tillbaka" },
            // pizzor (2)
            new string[] { "pizzas", "Lägg till", "Ta bort", "Ändra", "Lista över pizzor", "Gå tillbaka" },
            // ingredienser (3)
            new string[] { "ingredients", "Lägg till", "Ta bort", "Ändra", "Lista över pålägg", "Gå tillbaka" },
            // tillbehör (4)
            new string[] { "extras", "Lägg till", "Ta bort", "Ändra", "Lista över tillbehör", "Gå tillbaka" },
            // gamla ordrar (5)
            new string[] { "oldOrders", "Visa gamla ordrar", "Ta bort gamla ordrar", "Gå tillbaka" }

        };

        public static void logIn()
        {
            while (true)
            {
                Console.Write("Användarnamn: ");
                string userName = Console.ReadLine();
                Console.Write("Lösenord: ");
                string password = Console.ReadLine();

                if (userName == "test" && password == "test")
                {
                    Program.RunMainMenu(Menus.menuChoices);
                    break;
                }

                else
                {
                    Console.Clear();
                    Console.WriteLine("Inloggning misslyckades. Försök igen.");
                    Thread.Sleep(1500);
                    Console.Clear();
                    continue;
                }
            }

        }

    }
}
