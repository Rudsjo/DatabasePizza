using System;
using System.Threading;
using System.Text.RegularExpressions;
using BackendHandler;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Linq;

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
            start:
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

                if (CheckIfUserInputIsCorrect == true && choice == 1)
                {
                changePassword:
                    {
                        Console.Clear();
                        Console.WriteLine($"~~ ÄNDRA LÖSENORD FÖR ANVÄNDARE {UpdatedEmployee.UserID}");
                        Console.WriteLine("Klicka ESC för att gå tillbaka");

                        Console.Write("Ange det nya lösenordet: ");
                        UpdatedEmployee.Password = await ReadLineWithOptionToGoBack();
                        if(UpdatedEmployee.Password == null) { UpdatedEmployee = null; return UpdatedEmployee; }
                        else if (UpdatedEmployee.Password.Length < 1)
                        {
                            await MessageIfChoiceIsNotRight("Lösenordet måste innehålla tecken.");
                            goto changePassword;
                        }
                        else { return UpdatedEmployee; } // om ESC klickats så kommer Password att vara null vilket kan kontrolleras i ProgramState
                    }
                }

                else if (CheckIfUserInputIsCorrect == true && choice == 2)
                {
                changeRole:
                    {
                        Console.Clear();
                        Console.WriteLine($"~~ ÄNDRA ROLL FÖR ANVÄNDARE {UpdatedEmployee.UserID}");
                        Console.WriteLine("Klicka ESC för att gå tillbaka");

                        Console.Write("Ange den nya rollen: ");
                        UpdatedEmployee.Role = await ReadLineWithOptionToGoBack();
                        if(UpdatedEmployee.Role == null) { UpdatedEmployee = null; return UpdatedEmployee; }
                        else if (UpdatedEmployee.Role.ToLower() == "admin" || UpdatedEmployee.Role.ToLower() == "bagare" || UpdatedEmployee.Role.ToLower() == "kassör" || UpdatedEmployee.Role == null)
                        {
                            return UpdatedEmployee;
                        }
                        else
                        {
                            await MessageIfChoiceIsNotRight("Den angivna rollen finns inte.");
                            goto changeRole;
                        }
                    }
                }

                else
                {
                    await MessageIfChoiceIsNotRight("Felaktig inmatning.");
                    goto start;
                }
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
        public static async Task<Pizza> AddPizzaMenu(IDatabase database)
        {

            Pizza newPizza = new Pizza();

            Console.Clear();

        chooseName:
            Console.WriteLine("~~ LÄGG TILL PIZZA ~~");
            Console.WriteLine("Klicka på ESC för att gå tillbaka.");

            Console.WriteLine();
            Console.Write("Pizzans namn: ");
            newPizza.Type = await Menus.ReadLineWithOptionToGoBack();
            if(newPizza.Type == null) { return newPizza; }
            else if (newPizza.Type.Length < 1)
            {
                    await MessageIfChoiceIsNotRight("Pizzans namn måste innehålla tecken.");
                    await AddPizzaMenu(database);
            }
            else // om allt är ok med namnet går vi vidare till val av pizzabas
            {
                foreach(var pizza in await database.GetAllPizzas())
                {
                    if(newPizza.Type == pizza.Type)
                    {
                        await MessageIfChoiceIsNotRight("Namnet finns redan. Välj ett annat.");
                        goto chooseName;
                    }
                }

                Console.Write("Välj vilken bas pizzan skall ha: [1] Italiensk eller [2] Amerikansk: ");
                string choiceOfBase = await ReadLineWithOptionToGoBack();
                if(choiceOfBase == null) { return newPizza; }
                else
                {
                    newPizza.PizzabaseID = int.Parse(choiceOfBase);
                    if (newPizza.PizzabaseID == 1 || newPizza.PizzabaseID == 2)
                    {
                        Console.Write("Pizzans pris: ");
                        string pizzaPrice = await ReadLineWithOptionToGoBack();
                        if (pizzaPrice == null) { return newPizza; }
                        else
                        {
                            bool correctInput = int.TryParse(pizzaPrice, out int price);
                            if (correctInput == false || pizzaPrice.Length >= 4 || price < 0)
                            {
                                await MessageIfChoiceIsNotRight("Priset kan endast anges i siffror, kan inte kosta mer än 999 kr och måste vara över 0.");
                                await AddPizzaMenu(database);
                            }
                            else // om allt går igenom så returneras objektet
                            {
                                newPizza.Price = price;
                                return newPizza;
                            }

                        }
                    }
                    else
                    {
                        await MessageIfChoiceIsNotRight("Du måste välja [1] eller [2]");
                        await AddPizzaMenu(database);
                    }
                }
            }
            return newPizza;
        }
        public static async Task<List<Condiment>> AddCondimentToPizzaMenu(IDatabase database, Pizza pizzaWithoutCondiments)
        {
            List<Condiment> condimentsToNewPizza = new List<Condiment>();

        start:
            Console.Clear();
            Console.WriteLine("Välj en ingrediens till pizzan genom att ange ingrediensens nummer:");
            Console.WriteLine("Ange 0 för att bekräfta pizzans ingredienser.");
            Console.WriteLine("Klicka på ESC för att gå tillbaka.");
            Console.WriteLine();
            Console.Write(pizzaWithoutCondiments.Type + ":\n");
            for(int index = 0; index < condimentsToNewPizza.Count; index++)
            {
                if(index == 0) { Console.Write($"{condimentsToNewPizza[index].Type}"); }
                else { Console.Write($", {condimentsToNewPizza[index].Type}"); }

            }
            Console.WriteLine("\n");

            foreach (var condiment in await database.GetAllCondiments())
            {
                Console.WriteLine($"{condiment.CondimentID}. {condiment.Type}");
            }

            string chosenCondiment = await Menus.ReadLineWithOptionToGoBack();
            if(chosenCondiment == null) { return condimentsToNewPizza; }
            else
            {
                bool correctInput = int.TryParse(chosenCondiment, out int confirmedChosenCondiment);
                if(correctInput == true && confirmedChosenCondiment == 0 && condimentsToNewPizza.Count > 0)
                {
                    return condimentsToNewPizza;
                }
                else
                {
                    Condiment cond = await database.GetSingleCondiment(confirmedChosenCondiment);
                    cond.Price = 0;
                    condimentsToNewPizza.Add(cond);

                    await Menus.ConfirmationScreen("Ingrediens tillagd.");
                }
                goto start;
            }
        }
        public static async Task<Pizza> UpdatePizzaMenu(IDatabase database, Pizza pizza)
        {
            Pizza pizzaToUpdate = pizza;

            Console.Clear();
            Console.WriteLine("Klicka ESC för att gå tillbaka.");
            Console.WriteLine("Ange vad du vill ändra i pizzan:");
            Console.WriteLine("1. Botten");
            Console.WriteLine("2. Pris");
            Console.WriteLine("3. Ingredienser");

            string whatToUpdate = await ReadLineWithOptionToGoBack();
            if(whatToUpdate == null) { return pizzaToUpdate; }
            else
            {
                bool correctInput = int.TryParse(whatToUpdate, out int choice);
                if(correctInput == true && (choice == 1 || choice == 2 || choice == 3))
                {
                    if(choice == 1) { goto changeBase; }
                    else if(choice == 2) { goto changePrice; }
                    else if(choice == 3) { goto changeCondiments; }
                }
                else { await MessageIfChoiceIsNotRight("Det angivna valet finns inte."); }
            }

        changeBase:
            {
                if (pizzaToUpdate.PizzabaseID == 1)
                {
                    Console.Write("Skriv y om du vill ändra pizzans botten till amerikansk: ");
                    string userInput = await ReadLineWithOptionToGoBack();
                    if (userInput == null) { pizzaToUpdate.PizzabaseID = 0; return pizzaToUpdate; }
                    if (userInput == "y")
                    {
                        pizzaToUpdate.PizzabaseID = 2;
                        await ConfirmationScreen("Pizzans botten uppdaterad.");
                        return pizzaToUpdate;
                    }
                    else { await MessageIfChoiceIsNotRight("Felaktig inmatning. Klicka på ESC för att gå tillbaka."); }
                }

                else if (pizzaToUpdate.PizzabaseID == 2)
                {
                    Console.Write("Skriv y om du vill ändra pizzans botten till italiensk: ");
                    string userInput = await ReadLineWithOptionToGoBack();
                    if (userInput == null) { pizzaToUpdate.PizzabaseID = 0; return pizzaToUpdate; }
                    if (userInput == "y")
                    {
                        pizzaToUpdate.PizzabaseID = 1;
                        await ConfirmationScreen("Pizzans botten uppdaterad.");
                        return pizzaToUpdate;
                    }
                    else
                    {
                        await MessageIfChoiceIsNotRight("Felaktig inmatning. Klicka på ESC för att gå tillbaka.");
                    }
                }
            }

        changePrice:
            {
                Console.Write("Ange pizzans nya pris: ");
                string newPriceText = await ReadLineWithOptionToGoBack();
                if (newPriceText == null) { pizzaToUpdate.Price = 0; return pizzaToUpdate; }
                else
                {
                    bool correctInput = int.TryParse(newPriceText, out int newPrice);
                    if (correctInput == true && newPriceText.Length > 0 && newPriceText.Length < 4)
                    {
                        pizzaToUpdate.Price = newPrice;
                        await ConfirmationScreen("Pizzans pris uppdaterat.");
                        return pizzaToUpdate;
                    }
                    else { await MessageIfChoiceIsNotRight("Felaktig inmatning"); }
                }
            }

        changeCondiments:
            {
                Console.WriteLine("Ange vilken typ av ändring du vill göra:");
                Console.WriteLine("1. Lägga till ingredienser");
                Console.WriteLine("2. Ta bort ingredienser");
                Console.WriteLine();
                Console.Write("Ditt val: ");
                string choiceOfChange = await ReadLineWithOptionToGoBack();
                if (choiceOfChange == null) { pizzaToUpdate.PizzaIngredients = null; return pizzaToUpdate; }
                else
                {
                    bool correctInput = int.TryParse(choiceOfChange, out int choice);
                    if (correctInput == true && choice == 1) // lägga till
                    {
                        pizzaToUpdate.PizzaIngredients = await AddCondimentToPizzaMenu(database, pizzaToUpdate);

                        if(pizzaToUpdate.PizzaIngredients != null)
                        {
                            await database.AddCondimentToPizza(pizzaToUpdate);
                        }

                        return pizzaToUpdate;
                    }
                    else if (correctInput == true && choice == 2) // ta bort
                    {
                        Console.WriteLine("Ange numret på den ingrediens som du önskar att ta bort");

                        foreach (var condiment in pizzaToUpdate.PizzaIngredients)
                        {
                            Console.WriteLine($"{condiment.CondimentID}. {condiment.Type}");
                        }

                        Console.Write("Ditt val: "); string userChoice = await ReadLineWithOptionToGoBack();

                        bool correctRemovalInput = int.TryParse(userChoice, out int removalChoice);
                        if (correctRemovalInput == true)
                        {
                            foreach (var condiment in pizzaToUpdate.PizzaIngredients)
                            {
                                if (removalChoice == condiment.CondimentID)
                                {
                                    await database.DeleteCondimentFromPizza(pizzaToUpdate, condiment);
                                    pizzaToUpdate.PizzaIngredients = (await database.GetIngredientsFromSpecificPizza(pizzaToUpdate.PizzaID)).ToList();
                                    return pizzaToUpdate;
                                }
                            }
                        }
                        else
                        {
                            await MessageIfChoiceIsNotRight("Felaktig inmatning.");
                        }

                    }
                    else { await MessageIfChoiceIsNotRight("Felaktig inamtning."); }
                }

            }
            return pizzaToUpdate;
        }
        public static async Task<Pizza> DeletePizzaMenu(IDatabase database)
        {
            start:
            Pizza pizzaToDelete = new Pizza();

            Console.Clear();
            Console.WriteLine("~~ TA BORT PIZZA ~~");
            Console.WriteLine("Klicka på ESC för att gå tillbaka.");
            Console.WriteLine();

            foreach (var pizza in await database.GetAllPizzas())
            {
                Console.WriteLine($"ID - {pizza.PizzaID}, {pizza.Type}\n");
            }

            Console.Write("Ange ID för den pizza som du vill ta bort: ");
            string IDOfPizzaToRemove = await Menus.ReadLineWithOptionToGoBack();
            if(IDOfPizzaToRemove == null) { return pizzaToDelete; }
            else
            {
                bool correctInput = int.TryParse(IDOfPizzaToRemove, out int IDOfPizza);
                if(correctInput == true)
                {
                    bool checkIfPizzaExists = await database.CheckIfPizzaIDExists(IDOfPizza);
                    if(checkIfPizzaExists == true)
                    {
                        pizzaToDelete = await database.GetSinglePizza(IDOfPizza);
                        Console.Clear();
                        Console.WriteLine($"\nAnge y för att bekräfta borttagning av {pizzaToDelete.PizzaID}. {pizzaToDelete.Type}");
                        Console.Write("\nDitt val: "); string choiceToDelete = await ReadLineWithOptionToGoBack();

                        if (choiceToDelete == null) { pizzaToDelete = null; return pizzaToDelete; }
                        else if (choiceToDelete.ToLower() == "y") { return pizzaToDelete; }
                        else
                        {
                            await MessageIfChoiceIsNotRight("Ange y för att bekräfta eller ESC för att gå tillbaka.");
                            goto start;
                        }
                    }
                    else { await MessageIfChoiceIsNotRight("Det angivna ID:t finns inte."); goto start; }
                }
                else { await MessageIfChoiceIsNotRight("Felaktig inmatning."); goto start; }
            }
        }
        public static async Task<Condiment> AddCondimentMenu()
        {
            start:
            Condiment newCondiment = new Condiment();
            Console.Clear();
            Console.WriteLine("~~ LÄGG TILL INGREDIENS ~~");
            Console.WriteLine("Klicka på ESC för att gå tillbaka.");

            Console.WriteLine();
            Console.Write("Ange namnet på ingrediensen: ");
            string newCondimentName = await Menus.ReadLineWithOptionToGoBack();
            if(newCondimentName == null) { return newCondiment; }
            else if(newCondimentName.Length < 1 || !Regex.IsMatch(newCondimentName, @"^[a-öA-Ö]+$"))
            {
                await Menus.MessageIfChoiceIsNotRight("Felaktig inmatning.");
                goto start;
            }
            else
            {
                newCondiment.Type = newCondimentName;
                goto addPrice;
            }

            addPrice:
            {
                Console.Write("\nAnge pris för ingrediensen: ");
                string newCondimentPrice = await ReadLineWithOptionToGoBack();
                if(newCondimentPrice == null) { newCondiment = null; return newCondiment; }
                else
                {
                    bool correctInput = int.TryParse(newCondimentPrice, out int price);
                    if(correctInput == true && price > 0 && newCondimentPrice.Length < 3)
                    {
                        newCondiment.Price = price;
                        goto confirmation;
                    }
                    else { await MessageIfChoiceIsNotRight("Felaktig inmatning."); goto addPrice; }
                }
            }

            confirmation:
            {
                Console.Clear();
                Console.WriteLine($"Ange y om du vill lägga till {newCondiment.Type} som ingrediens.");
                Console.WriteLine("Klicka ESC om du vill gå tillbaka.");
                Console.Write("\nDitt val: "); string confirmChoice = await ReadLineWithOptionToGoBack();
                if(confirmChoice == null) { newCondiment = null; return newCondiment; }
                else if(confirmChoice.ToLower() == "y") { return newCondiment; }
                else
                {
                    await MessageIfChoiceIsNotRight("Vänligen ange y för att bekräfta eller ESC för att gå tillbaka.");
                    goto confirmation;
                }

            }

        }
        public static async Task<Condiment> UpdateCondimentMenu(Condiment condimentToUpdate)
        {
            start:
            Console.WriteLine($"\nVad vill du ändra på i ingrediensen {condimentToUpdate.Type}?");
            Console.WriteLine("1. Namn");
            Console.WriteLine("2. Pris");
            Console.Write("\nDitt val: "); string whatToUpdate = await ReadLineWithOptionToGoBack();
            if(whatToUpdate == null) { condimentToUpdate = null; return condimentToUpdate; }
            else
            {
                bool correctInput = int.TryParse(whatToUpdate, out int choice);
                if(correctInput == true && choice == 1)
                {
                    updateName:
                    {
                        Console.Clear();
                        Console.WriteLine("Klicka ESC för att gå tillbaka.");
                        Console.Write("\nAnge ingrediensens nya namn: ");
                        condimentToUpdate.Type = await ReadLineWithOptionToGoBack();
                        if(condimentToUpdate.Type == null) { condimentToUpdate = null; return condimentToUpdate; }
                        else if(condimentToUpdate.Type.Length < 1) { await MessageIfChoiceIsNotRight("Namnet måste innehålla tecken"); goto updateName; }
                        else { return condimentToUpdate; }
                    }
                }
                else if(correctInput == true && choice == 2)
                {
                    updatePrice:
                    {
                        Console.Clear();
                        Console.WriteLine("Klicka ESC för att gå tillbaka.");
                        Console.Write("\nAnge ingrediensens nya pris: ");
                        string newCondimentPrice = await ReadLineWithOptionToGoBack();
                        if(newCondimentPrice == null) { condimentToUpdate = null; return condimentToUpdate; }
                        else
                        {
                            correctInput = int.TryParse(newCondimentPrice, out int price);
                            if(correctInput == true && price > 0 && newCondimentPrice.Length < 3)
                            {
                                return condimentToUpdate;
                            }
                            else { await MessageIfChoiceIsNotRight("Felaktig inmatning."); goto updatePrice; }
                        }
                    }
                }
                else { await MessageIfChoiceIsNotRight("Felaktig inmatning"); goto start; }
            }

        }
        public static async Task<Condiment> DeleteCondimentMenu(IDatabase database)
        {
            start:
            Console.Clear();
            Console.WriteLine("~~ TA BORT INGREDIENS ~~");
            Console.WriteLine("Klicka på ESC för att gå tillbaka.");
            Console.WriteLine();

            Condiment condimentToDelete = new Condiment();

            foreach (var condiment in await database.GetAllCondiments())
            {
                Console.WriteLine($"{condiment.CondimentID}. {condiment.Type}");
            }

            Console.Write("Ange ID för den ingrediens som du vill ta bort: ");
            string IDOfCondimentToDelete = await ReadLineWithOptionToGoBack();
            if(IDOfCondimentToDelete == null) { return condimentToDelete; }
            else
            {
                bool correctInput = int.TryParse(IDOfCondimentToDelete, out int IDOfCondiment);
                if(correctInput == true)
                {
                    bool checkIfIDExists = await database.CheckIfCondimentIDExists(IDOfCondiment);
                    if(checkIfIDExists == true)
                    {
                    confirmationToDelete:
                        {
                            condimentToDelete = await database.GetSingleCondiment(IDOfCondiment);
                            Console.Clear();
                            Console.WriteLine($"Ange y om du vill ta bort ingrediensen {condimentToDelete.Type}");
                            string choiceToDelete = await ReadLineWithOptionToGoBack();
                            if (choiceToDelete == null) { condimentToDelete = null; return condimentToDelete; }
                            else if(choiceToDelete.ToLower() == "y") { return condimentToDelete; }
                            else { await MessageIfChoiceIsNotRight("Ange y för att bekräfta eller ESC för att gå tillbaka."); goto confirmationToDelete; }
                         }
                    }
                    else { await MessageIfChoiceIsNotRight("Det angivna ID:t finns inte."); goto start; }
                }
                else { await MessageIfChoiceIsNotRight("Felaktig inmatning."); goto start; }
            }

        }
        public static async Task<Extra> AddExtraMenu()
        {
        start:
            Extra newExtra = new Extra();

            Console.Clear();
            Console.WriteLine("~~ LÄGG TILL TILLBEHÖR ~~");
            Console.WriteLine("Klicka på ESC för att gå tillbaka.");

            Console.WriteLine();
            Console.Write("Ange namnet på tillbehöret du vill lägga till: ");
            newExtra.Type = await Menus.ReadLineWithOptionToGoBack();
            if(newExtra.Type == null) { newExtra = null; return newExtra; }
            else if(newExtra.Type.Length < 1 || !Regex.IsMatch(newExtra.Type, @"^[a-öA-Ö\s]+$"))
            {
                await MessageIfChoiceIsNotRight("Namnet kan endast innehålla bokstäver och det måste vara minst ett tecken.");
                goto start;
            }
            else { goto setPrice; }

            setPrice:
            {
                Console.Write("\nAnge tillbehörets pris: ");
                string priceOfNewExtra = await ReadLineWithOptionToGoBack();
                if(priceOfNewExtra == null) { newExtra = null; return newExtra; }
                else
                {
                    bool correctInput = int.TryParse(priceOfNewExtra, out int price);
                    if(correctInput == true && price > 0 && priceOfNewExtra.Length < 3)
                    {
                        return newExtra;
                    }
                    else { await MessageIfChoiceIsNotRight("Felaktig inmatning."); goto setPrice; }
                }
            }
        }
        public static async Task<Extra> UpdateExtraMenu(Extra extraToUpdate)
        {
            start:
            Console.WriteLine($"Ange vad du vill uppdatera med tillbehöret {extraToUpdate.Type}");
            Console.WriteLine("1. Namn");
            Console.WriteLine("2. Pris");
            string whatToUpdate = await ReadLineWithOptionToGoBack();
            if(whatToUpdate == null) { extraToUpdate = null; return extraToUpdate; }
            else
            {
                bool correctInput = int.TryParse(whatToUpdate, out int choice);
                if(correctInput == true && choice == 1)
                {
                    changeType:
                    {
                        Console.Clear();
                        Console.Write("Ange tillbehörets nya namn: ");
                        extraToUpdate.Type = await ReadLineWithOptionToGoBack();
                        if(extraToUpdate.Type == null) { extraToUpdate = null; return extraToUpdate; }
                        else if(extraToUpdate.Type.Length < 1 || !Regex.IsMatch(extraToUpdate.Type, @"^[a-öA-Ö]+$"))
                        {
                            await MessageIfChoiceIsNotRight("Namnet kan endast innehålla bokstäver och måste vara minst ett tecken.");
                            goto changeType;
                        }
                        else { return extraToUpdate; }
                    }
                }
                else if(correctInput == true && choice == 2)
                {
                    changePrice:
                    {
                        Console.Clear();
                        Console.Write("Ange tillbehörets nya pris: ");
                        string newPriceOfExtra = await ReadLineWithOptionToGoBack();
                        if(newPriceOfExtra == null) { extraToUpdate = null; return extraToUpdate; }
                        else
                        {
                            correctInput = int.TryParse(newPriceOfExtra, out int price);
                            if(correctInput == true && price > 0 && newPriceOfExtra.Length < 3)
                            {
                                return extraToUpdate;
                            }
                            else { await MessageIfChoiceIsNotRight("Felaktig inmatning."); goto changePrice; }
                        }
                    }
                }
                else { await MessageIfChoiceIsNotRight("Felaktig inmatning."); goto start; }
            }
        }
        public static async Task<Extra> DeleteExtraMenu(IDatabase database)
        {
        start:
            Extra extraToDelete = new Extra();
            Console.Clear();
            Console.WriteLine("~~ TA BORT TILLBEHÖR ~~");
            Console.WriteLine("Klicka på ESC för att gå tillbaka.");

            Console.Write("\n\nAnge ID för det tillbehör som ska tas bort: ");
            string IDOfExtraToDelete = await ReadLineWithOptionToGoBack();
            if(IDOfExtraToDelete == null) { extraToDelete = null; return extraToDelete; }
            else
            {
                bool correctInput = int.TryParse(IDOfExtraToDelete, out int IDOfExtra);
                if(correctInput == true)
                {
                    bool checkIfIDExists = await database.CheckIfProductIDExists(IDOfExtra);
                    if(checkIfIDExists == true)
                    {
                        extraToDelete = await database.GetSingleExtra(IDOfExtra);

                    confirmationToDelete:
                        {
                            Console.Clear();
                            Console.WriteLine($"Ange y för att bekräfta borttagningen av tillbehöret {extraToDelete.Type}");
                            Console.WriteLine("Klicka på ESC för att gå tillbaka.");
                            Console.Write("\nDitt val: ");
                            string choiceToDelete = await ReadLineWithOptionToGoBack();
                            if (choiceToDelete == null) { extraToDelete = null; return extraToDelete; }
                            else if (choiceToDelete.ToLower() == "y") { return extraToDelete; }
                            else { await MessageIfChoiceIsNotRight("Ange y för att bekräfta eller ESC för att gå tillbaka."); goto confirmationToDelete; }
                        }
                    }
                    else { await MessageIfChoiceIsNotRight("Det angivna ID:t finns inte."); goto start; }
                }
                else { await MessageIfChoiceIsNotRight("Felaktig inmatning."); goto start; }
            }

        }
        
        #endregion


        //Ny metod för att kolla igenom string input och se om det är siffror i stringen.
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
