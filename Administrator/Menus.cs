using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using MenuFunctions;

namespace Administrator
{
    public class Menus
    {
        /** 
         * Denna klass innehåller samtliga stringarrayer med menyval för de menyer som tillhör admin-programmet
         **/


        public string[][] menuChoices =
        {
            // main menu (0)
            new string[] { "Anställda", "Pizzor", "Pålägg", "Tillbehör", "Gamla ordrar", "Logga ut" },
            // anställda (1)
            new string[] { "Lägg till anställd", "Uppdatera anställd", "Visa alla anställda", "Ta bort anställd", "Gå tillbaka" },
            // pizzor (2)
            new string[] { "Lägg till pizza", "Uppdatera pizza", "Visa alla pizzor", "Ta bort pizza", "Gå tillbaka" },
            // ingredienser (3)
            new string[] { "Lägg till ingrediens", "Uppdatera ingrediens", "Visa alla ingredienser", "Ta bort ingrediens", "Gå tillbaka" },
            // tillbehör (4)
            new string[] { "Lägg till tillbehör", "Uppdatera tillbehör", "Visa alla tillbehör", "Ta bort tillbehör", "Gå tillbaka" },
            // gamla ordrar (5)
            new string[] { "Visa gamla ordrar", "Ta bort gamla ordrar", "Gå tillbaka" }

        };
    }
}
