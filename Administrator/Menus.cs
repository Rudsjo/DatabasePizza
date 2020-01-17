using System;
using System.Collections.Generic;
using System.Text;

namespace Administrator
{
    public class Menus
    {
        /** 
         * Denna klass innehåller samtliga stringarrayer med menyval för de menyer som tillhör admin-programmet
         **/

        public static string[][] logIn =
        {
            // logga in (0)
            new string[] { "Ange användarID: ", "Ange lösenord: "},
        };

        public static string[][] menuChoices =
        {
            // main menu (0)
            new string[] { "Användare", "Pizzor", "Pålägg", "Tillbehör", "Gamla ordrar", "Logga ut" },
            // employees (1)
            new string[] { "Lägg till", "Ta bort", "Ändra", "Lista på anställda", "Gå tillbaka" },
            // pizzas (2)
            new string[] { "Lägg till", "Ta bort", "Ändra", "Lista över pizzor", "Gå tillbaka" },
            // ingredients (3)
            new string[] { "Lägg till", "Ta bort", "Ändra", "Lista över pålägg", "Gå tillbaka" },
            // extras (4)
            new string[] { "Lägg till", "Ta bort", "Ändra", "Lista över tillbehör", "Gå tillbaka" },
            // old orders (5)
            new string[] { "Visa gamla ordrar", "Ta bort gamla ordrar", "Gå tillbaka" }

        };


    }
}
