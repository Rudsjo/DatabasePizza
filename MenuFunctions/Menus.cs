using System;
using System.Threading;
using System.Text.RegularExpressions;
using BackendHandler;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MenuFunctions
{
    public static class Menus
    {
        public static string[][] MenuChoices { get; } =
        {
            // main menu (0)
            new string[] {"~ HUVUDMENY ~\n", "1. Anställda", "2. Pizzor", "3. Ingredienser", "4. Tillbehör", "5. Gamla ordrar", "6. Logga ut" },
            // anställda (1)
            new string[] {"~ ANSTÄLLDA ~\n", "1. Lägg till anställd", "2. Uppdatera anställd", "3. Visa alla anställda", "4. Ta bort anställd", "5. Gå tillbaka" },
            // pizzor (2)
            new string[] {"~ PIZZOR ~\n", "1. Lägg till pizza", "2. Uppdatera pizza", "3. Visa alla pizzor", "4. Ta bort pizza", "5. Gå tillbaka" },
            // ingredienser (3)
            new string[] {"~ INGREDIENSER ~\n", "1. Lägg till ingrediens", "2. Uppdatera ingrediens", "3. Visa alla ingredienser", "4. Ta bort ingrediens", "5. Gå tillbaka" },
            // tillbehör (4)
            new string[] {"~ TILLBEHÖR ~\n", "1. Lägg till tillbehör", "2. Uppdatera tillbehör", "3. Visa alla tillbehör", "4. Ta bort tillbehör", "5. Gå tillbaka" },
            // gamla ordrar (5)
            new string[] {"~ GAMLA ORDRAR ~\n", "1. Visa gamla ordrar", "2. Ta bort gamla ordrar", "3. Gå tillbaka" }

        };

        public static async Task MessageIfChoiceIsNotRight(params string[] messages)
        {
            StringBuilder message = new StringBuilder();

            Console.Clear();
            foreach (var sentence in messages)
            {
                message.Append($"{sentence}\n");
            }
            Console.WriteLine(message.ToString());
            Thread.Sleep(1000);
            Console.Clear();
        }
        public static async Task ConfirmationScreen(params string[] text)
        {
            Console.Clear();
            foreach (var item in text)
            {
                Console.WriteLine(item);
            }
            Thread.Sleep(1000);

        }
        public static async Task<(bool, int)> PrintMenuAndCheckChoice(string[] MenuChoices)
        {
            StringBuilder menu = new StringBuilder();

            Console.Clear();
            foreach(string option in MenuChoices)
            {
                menu.Append($"{option}\n");
            }

            Console.WriteLine(menu.ToString());
            Console.Write("Ditt val: ");

            bool userChoice = int.TryParse(Console.ReadLine(), out int choice);
            if (userChoice == true)
            {
                return (true, choice);
            }
            else { return (false, 0); }
        }

        public static async Task<bool> PrintAndReturnStateOfLogin(IDatabase database)
        {
            #region Variables
            int userID;
            string password = "", GetRole = "";
            #endregion


            Console.Clear();
            Console.WriteLine("~ VÄLKOMMEN TILL ADMINPANELEN ~");
            Console.WriteLine("Logga in genom att ange ditt ID och lösenord.\n");

            Console.Write("Ange ID: ");
            bool CorrectInput = int.TryParse(await ReadLineWithOptionToGoBack(), out userID);

            // Kollar om ID är inskrivet i rätt format
            if(CorrectInput == true)
            {
                Console.Write("Lösenord: ");
                password = await ShowPasswordAsStarsWithOptionToGoBack();
            }
            else 
            { 
                await MessageIfChoiceIsNotRight("ID är skrivet i fel format.");
                await PrintAndReturnStateOfLogin(database);
            }

            // Kontrollerar om angivet ID och lösenord finns matchat i databasen
            bool CheckForCorrectCredentials = (await database.CheckUserIdAndPassword(userID, password, "CheckPassword")).Item1;

            if (CheckForCorrectCredentials == true)
            {
                // Om inloggning lyckas så kontrolleras den anställdes roll för att se om denne är admin och således har access
                GetRole = (await database.CheckUserIdAndPassword(userID, password, "CheckPassword")).Item2;
            }
            else
            {
                await MessageIfChoiceIsNotRight("AnvändarID eller lösenord är felaktigt. Försök igen.");
                return false;
            }

            
            if(GetRole.ToLower() == "admin") { return true; }
            else
            {
                await MessageIfChoiceIsNotRight("Behörighet saknas.");
                return false;
            }
        }


        //Ny metod för att kolla igenom string input och se om det är siffror i stringen.
        public static async Task<bool> RestrictNumericalInStrings(string input)
        {
            if(Regex.IsMatch(input, @"[a-öA-Ö]$")) { return true; }
            else { return false; }               
        }
        public static async Task<bool> RestrictLettersInNumerical(string input)
        {
                if (Regex.IsMatch(input, @"[0-9]$")) { return true; }
                else { return false; }
        }
        public static async Task<string> ShowPasswordAsStarsWithOptionToGoBack()
        {
            {
                string input = "";
                int stringIndex = 0;

                do
                {
                    ConsoleKeyInfo readKeyResult = Console.ReadKey(true);

                    if (readKeyResult.Key == ConsoleKey.Escape)
                    {
                        return null;
                    }

                    // handle Enter
                    if (readKeyResult.Key == ConsoleKey.Enter)
                    {
                        return input;
                    }

                    if (readKeyResult.Key == ConsoleKey.Backspace)
                    {
                        if (stringIndex > 0)
                        {
                            input = input.Remove(input.Length - 1);
                            Console.Write(readKeyResult.KeyChar);
                            Console.Write(' ');
                            Console.Write(readKeyResult.KeyChar);
                            stringIndex--;
                        }
                    }

                    else
                    {
                        input += readKeyResult.KeyChar;
                        Console.Write("*");
                        stringIndex++;
                    }
                } while (true);
            }
        }
        public static async Task<string> ReadLineWithOptionToGoBack()
        {
            {
                string input = "";
                int stringIndex = 0;

                do
                {
                    ConsoleKeyInfo readKeyResult = Console.ReadKey(true);

                    if (readKeyResult.Key == ConsoleKey.Escape) return null;

                    // handle Enter
                    if (readKeyResult.Key == ConsoleKey.Enter) { Console.WriteLine(); return input; }

                    if (readKeyResult.Key == ConsoleKey.Backspace)
                    {
                        if (stringIndex > 0)
                        {
                            input = input.Remove(input.Length - 1);
                            Console.Write(readKeyResult.KeyChar);
                            Console.Write(' ');
                            Console.Write(readKeyResult.KeyChar);
                            stringIndex--;
                        }
                    }

                    else
                    {
                        input += readKeyResult.KeyChar;
                        Console.Write(readKeyResult.KeyChar);
                        stringIndex++;
                    }
                } while (true);
            }
        }

    }

}
