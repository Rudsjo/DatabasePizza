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

        #region // Undermenyer

        public static async Task<Employee> AddEmployeeMenu()        // Funktion som ritar ut menyn för att lägga till en ny anställd. 
        {                                                                        // Returnernas en instans av ny användare.
            Employee emp = new Employee();

            Console.Clear();
            Console.WriteLine("~~ LÄGG TILL ANSTÄLLD ~~");
            Console.WriteLine("Klicka på ESC för att gå tillbaka.");

            Console.Write("Ange den anställdes lösenord: ");
            emp.Password = await ReadLineWithOptionToGoBack();

            if(emp.Password == null) { return (emp); }       // om ESC klickats så returneras null som sedan kan kontrolleras i ProgramState
            else if(emp.Password.Length < 1)                // om lösenordet innehåller felaktigt antal tecken så körs funktionen om från början
            {
                await MessageIfChoiceIsNotRight("Lösenordet måste innehålla tecken.");
                await AddEmployeeMenu();
            }
            else                                            // om lösenordet är korrekt så går programmet vidare till att fråga om roll
            {
                Console.Write("Ange den anställdes roll (admin, bagare, kassör): ");
                emp.Role = await ReadLineWithOptionToGoBack();
                if(emp.Role == null) { return emp; }        // om ESC klickas så returneras null som sedan kan kontrolleras i ProgramState
                else if(emp.Role.ToLower() == "admin" || emp.Role.ToLower() == "bagare" || emp.Role.ToLower() == "kassör")
                {                                           // sedan kontrolleras så den angivna rollen uppfyller någon av de rollerna som finns
                    return emp;
                }
                else
                {
                    await MessageIfChoiceIsNotRight("Den angivna rollen finns inte.");
                    await AddEmployeeMenu();
                }
            }

            return emp;
        }
        public static async Task<Employee> CheckEmployeeIDMenu(IDatabase database) // Funktion som ritar ut menyn för att kontrollera om en angiven anställd finns
        {
            Employee emp = new Employee();

            Console.Clear();
            Console.WriteLine("~~ UPPDATERA ANSTÄLLD ~~");
            Console.WriteLine("Klicka på ESC för att gå tillbaka.");

            Console.Write("Ange ID på den anställde som du vill uppdatera: ");
            string IDOfEmployeeToUpdate = await ReadLineWithOptionToGoBack();
            if(IDOfEmployeeToUpdate == null) { return emp; }                // om ESC klickas så returneras en instans av en Employee med null värden som sedan kan kontrolleras
            else
            {
                int.TryParse(IDOfEmployeeToUpdate, out int ID);
                bool CheckIfUserExists = await database.CheckIfUserIDExists(ID); // kontrollerar så att det angivna ID:t finns i databasen

                if(CheckIfUserExists == true)
                {
                    emp = await database.GetSingleEmployee(ID);                 // om inte ESC klickats och om ID:t finns i databasen så laddas den data som finns kopplat till angivet ID in till det instansierade objektet
                }
                else
                {
                    await MessageIfChoiceIsNotRight("Det angivna ID:t finns inte.");
                    await CheckEmployeeIDMenu(database);
                }

                return emp;
            }           
        }
        public static async Task<Employee> UpdateEmployeeMenu(Employee emp) // Funktion som uppdaterar en angiven användare. Det objekt som skickas in här bör ha kontrolleras i CheckEmployeeID tididgare.
        {
            Employee UpdatedEmployee = new Employee();

            Console.Clear();
            Console.WriteLine("Klicka ESC för att gå tillbaka.");
            Console.WriteLine("Ange vad du vill ändra hos användaren:");
            Console.WriteLine("1. Lösenord");
            Console.WriteLine("2. Roll");

            string whatToUpdate = await ReadLineWithOptionToGoBack();
            if(whatToUpdate == null) { return UpdatedEmployee; }    // om ESC klickas så returneras en instans med nullvärden som sedan kan kontrolleras
            else
            {
                UpdatedEmployee = emp;              // annars så får instansen UpdatedEmployee samma värden som det medskickade objektet
                bool CheckIfUserInputIsCorrect = int.TryParse(whatToUpdate, out int choice);

                if(CheckIfUserInputIsCorrect == true && choice == 1)
                {
                    Console.Clear();
                    Console.WriteLine($"~~ ÄNDRA LÖSENORD FÖR ANVÄNDARE {UpdatedEmployee.UserID}");
                    Console.WriteLine("Klicka ESC för att gå tillbaka");

                    Console.Write("Ange det nya lösenordet: ");
                    UpdatedEmployee.Password = await ReadLineWithOptionToGoBack();
                    if(UpdatedEmployee.Password.Length < 1)
                    {
                        await MessageIfChoiceIsNotRight("Lösenordet måste innehålla tecken.");
                        await UpdateEmployeeMenu(emp);
                    }
                    else { return UpdatedEmployee; } // om ESC klickats så kommer Password att vara null vilket kan kontrolleras i ProgramState
                }

                else if(CheckIfUserInputIsCorrect == true && choice == 2)
                {
                    Console.Clear();
                    Console.WriteLine($"~~ ÄNDRA ROLL FÖR ANVÄNDARE {UpdatedEmployee.UserID}");
                    Console.WriteLine("Klicka ESC för att gå tillbaka");

                    Console.Write("Ange den nya rollen: ");
                    UpdatedEmployee.Role = await ReadLineWithOptionToGoBack();
                    if(UpdatedEmployee.Role.ToLower() == "admin" || UpdatedEmployee.Role.ToLower() == "bagare" || UpdatedEmployee.Role.ToLower() == "kassör" || UpdatedEmployee.Role == null)
                    {
                        return UpdatedEmployee;
                    }
                    else
                    {
                        await MessageIfChoiceIsNotRight("Den angivna rollen finns inte.");
                        await UpdateEmployeeMenu(emp);
                    }
                }

                else
                {
                    await MessageIfChoiceIsNotRight("Felaktig inmatning.");
                    await UpdateEmployeeMenu(emp);
                }

                return UpdatedEmployee;
            }
        }
        public static async Task<Employee> DeleteEmployeeMenu(IDatabase database) // funktion som returnernar ett objekt av en anställd som ska raderas
        {
            Employee employeeToDelete = new Employee();

            Console.Clear();
            Console.WriteLine("~~ TA BORT ANSTÄLLD ~~");
            Console.WriteLine("Klicka på ESC för att gå tillbaka.");
            Console.WriteLine();

            Console.Write("Ange ID för den anställda som du vill ta bort: ");
            string IDOfUserToDelete = await ReadLineWithOptionToGoBack();

            if(IDOfUserToDelete == null) { return employeeToDelete; } // vid klick på ESC så returneras instansen med nullvärden som kan kontrolleras i ProgramState
            else
            {
                bool CheckIfInputIsCorrect = int.TryParse(IDOfUserToDelete, out int ID);
                if(CheckIfInputIsCorrect == true)
                {
                    bool CheckIfUserIDExists = await database.CheckIfUserIDExists(ID);
                    if(CheckIfUserIDExists == true)
                    {
                        Console.WriteLine();
                        Console.WriteLine($"Är du säker på att du vill radera användare med ID {ID}? y/n");
                        Console.Write("Ditt svar: ");
                        string choiceToDeleteOrNot = await ReadLineWithOptionToGoBack();
                        if(choiceToDeleteOrNot == null) { return employeeToDelete; }
                        else if(choiceToDeleteOrNot.ToLower() == "y")
                        {
                            employeeToDelete = await database.GetSingleEmployee(ID);
                            return employeeToDelete;
                        }
                        else if(choiceToDeleteOrNot.ToLower() == "n")
                        {
                            await ConfirmationScreen("Avbrutet.");
                            await DeleteEmployeeMenu(database);
                        }
                        else { await MessageIfChoiceIsNotRight("Vänligen svara y eller n"); }

                    }
                    else
                    {
                        await MessageIfChoiceIsNotRight("Angivet ID finns inte.");
                        await DeleteEmployeeMenu(database);
                    }
                }
                else
                {
                    await MessageIfChoiceIsNotRight("Felaktig inmatning.");
                    await DeleteEmployeeMenu(database);
                }

                return employeeToDelete;
            }
        }
        #endregion


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
