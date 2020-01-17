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

        public static string[][] menuChoices =
        {
            // logga in (0)
            new string[] { "Ange användarID: ", "Ange lösenord: "},
            // main menu (1)
            new string[] { "Användare", "Pizzor", "Pålägg", "Tillbehör", "Gamla ordrar", "Logga ut" },
            // employees (2)
            new string[] { "Lägg till", "Ta bort", "Ändra", "Lista på anställda", "Gå tillbaka" },
            // pizzas (3)
            new string[] { "Lägg till", "Ta bort", "Ändra", "Lista över pizzor", "Gå tillbaka" },
            // ingredients (4)
            new string[] { "Lägg till", "Ta bort", "Ändra", "Lista över pålägg", "Gå tillbaka" },
            // extras (5)
            new string[] { "Lägg till", "Ta bort", "Ändra", "Lista över tillbehör", "Gå tillbaka" },
            // old orders (6)
            new string[] { "Visa gamla ordrar", "Ta bort gamla ordrar", "Gå tillbaka" }

        };




        // FÖLJANDE DEL SKALL TAS BORT? SAMMA SOM OVANSTÅENDE FAST FUNKAR INTE MED GENERISKT VAL FÖR ANVÄNDAREN

        public static string[] logIn = { "Ange användarID: ", "Ange lösenord: " };

        public static string[] mainMenu = { "Användare", "Pizzor", "Pålägg", "Tillbehör", "Gamla ordrar", "Logga ut" };

        // ANVÄNDARE
        public static string[] users = { "Lägg till", "Ta bort", "Ändra", "Lista på anställda", "Gå tillbaka" };


        // PIZZOR
        public static string[] pizzas = { "Lägg till", "Ta bort", "Ändra", "Lista över pizzor", "Gå tillbaka" };


        // PÅLÄGG
        public static string[] ingredients = { "Lägg till", "Ta bort", "Ändra", "Lista över pålägg", "Gå tillbaka" };


        // TILLBEHÖR
        public static string[] extras = { "Lägg till", "Ta bort", "Ändra", "Lista över tillbehör", "Gå tillbaka" };

        // GAMLA ORDRAR
        public static string[] oldOrders = { "Visa gamla ordrar", "Ta bort gamla ordrar", "Gå tillbaka" };

    }
}
