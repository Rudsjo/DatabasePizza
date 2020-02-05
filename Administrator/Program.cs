using System;
using System.Threading;
using BackendHandler;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using System.Text.RegularExpressions;
using System.IO;
using MenuFunctions;
using System.Data;

namespace Administrator
{
    public static class ProgramState
    {
        public static bool Running { get; set; } = true;
        public enum PROGRAM_MENUES
        {
            LOGIN_SCREEN,

            MAIN_MENU,

            EMPLOYEES,
            ADD_EMPLOYEE,
            UPDATE_EMPLOYEE,
            SHOW_EMPLOYEE,
            DELETE_EMPLOYEE,
            PIZZAS,
            ADD_PIZZA,
            UPDATE_PIZZA,
            SHOW_PIZZA,
            DELETE_PIZZA,
            INGREDIENTS,
            ADD_INGREDIENT,
            UPDATE_INGREDIENT,
            SHOW_INGREDIENT,
            DELETE_INGREDIENT,
            EXTRAS,
            ADD_EXTRAS,
            UPDATE_EXTRAS,
            SHOW_EXTRAS,
            DELETE_EXTRAS,
            OLD_ORDERS,
            SHOW_OLD_ORDERS,
            DELETE_OLD_ORDERS
        }
        public static PROGRAM_MENUES CURRENT_MENU { get; set; } =
          PROGRAM_MENUES.LOGIN_SCREEN;
    }

    public class Program
    {
        public static IDatabase rep { get;set; }

        static async Task Main(string[] args)
        {
            #region Read configfile and populate lists
            rep = Helpers.GetSelectedBackend(File.ReadAllLines(Environment.CurrentDirectory + "\\Backend.cfg").First(s => ! s.StartsWith("#")));
            #endregion


            while (ProgramState.Running)
            {
                switch (ProgramState.CURRENT_MENU)
                {
                    #region // Case: Login
                    case ProgramState.PROGRAM_MENUES.LOGIN_SCREEN:
                        {
                            bool CorrectLogin = await Menus.PrintAndReturnStateOfLogin(rep);
                            if (CorrectLogin == true)
                            {
                                ProgramState.CURRENT_MENU = ProgramState.PROGRAM_MENUES.MAIN_MENU;
                            }
                            else await Menus.PrintAndReturnStateOfLogin(rep);
                        }
                        break;
                    #endregion
                    
                    #region // Case: Main Menu
                    case ProgramState.PROGRAM_MENUES.MAIN_MENU:
                        {
                            var UserChoice = await Menus.PrintMenuAndCheckChoice(Menus.MenuChoices[0]);
                            {
                                if (UserChoice.Item1 == true)
                                {                                 
                                    if (UserChoice.Item2 == 1) { ProgramState.CURRENT_MENU = ProgramState.PROGRAM_MENUES.EMPLOYEES; }
                                    else if (UserChoice.Item2 == 2) { ProgramState.CURRENT_MENU = ProgramState.PROGRAM_MENUES.PIZZAS; }
                                    else if (UserChoice.Item2 == 3) { ProgramState.CURRENT_MENU = ProgramState.PROGRAM_MENUES.INGREDIENTS; }
                                    else if (UserChoice.Item2 == 4) { ProgramState.CURRENT_MENU = ProgramState.PROGRAM_MENUES.EXTRAS; }
                                    else if (UserChoice.Item2 == 5) { ProgramState.CURRENT_MENU = ProgramState.PROGRAM_MENUES.OLD_ORDERS; }
                                    else if (UserChoice.Item2 == 6) { ProgramState.CURRENT_MENU = ProgramState.PROGRAM_MENUES.LOGIN_SCREEN; }
                                    else await Menus.MessageIfChoiceIsNotRight("Ditt val är felaktigt.", "Valet finns inte.");                             
                                }
                                else await Menus.MessageIfChoiceIsNotRight("Ditt val är felaktigt.");                                
                            }
                        }
                        break;
                    #endregion

                    #region // Employees
                    case ProgramState.PROGRAM_MENUES.EMPLOYEES:
                        {
                            var UserChoice = await Menus.PrintMenuAndCheckChoice(Menus.MenuChoices[1]);
                            {
                                if (UserChoice.Item1 == true)
                                {
                                    if (UserChoice.Item2 == 1) { ProgramState.CURRENT_MENU = ProgramState.PROGRAM_MENUES.ADD_EMPLOYEE; }
                                    else if (UserChoice.Item2 == 2) { ProgramState.CURRENT_MENU = ProgramState.PROGRAM_MENUES.UPDATE_EMPLOYEE; }
                                    else if (UserChoice.Item2 == 3) { ProgramState.CURRENT_MENU = ProgramState.PROGRAM_MENUES.SHOW_EMPLOYEE; }
                                    else if (UserChoice.Item2 == 4) { ProgramState.CURRENT_MENU = ProgramState.PROGRAM_MENUES.DELETE_EMPLOYEE; }
                                    else if (UserChoice.Item2 == 5) { ProgramState.CURRENT_MENU = ProgramState.PROGRAM_MENUES.MAIN_MENU; }
                                    else await Menus.MessageIfChoiceIsNotRight("Ditt val är felaktigt.", "Valet finns inte.");
                                }
                                else await Menus.MessageIfChoiceIsNotRight("Ditt val är felaktigt.");
                            }
                        }
                        break;

                    case ProgramState.PROGRAM_MENUES.ADD_EMPLOYEE:
                        {
                            Employee newEmployee = await Menus.AddEmployeeMenu();

                            if(newEmployee.Password == null || newEmployee.Role == null)    // Om något av värdena returnerats som null så innebär det att ESC klickats och då skickas användaren tillbaka
                            {
                                ProgramState.CURRENT_MENU = ProgramState.PROGRAM_MENUES.EMPLOYEES;
                            }
                            else
                            {                                               // Annars läggs den nya användaren till i databasen
                                await rep.AddEmployee(newEmployee);
                                await Menus.ConfirmationScreen("Den anställde är tillagd.");
                            }
                        }
                        break;
                    case ProgramState.PROGRAM_MENUES.UPDATE_EMPLOYEE:
                        {
                            Employee employeeToUpdate = await Menus.CheckEmployeeIDMenu(rep);
                            if(employeeToUpdate.Password == null)                               // kontrollerar om ESC klickats
                            {
                                ProgramState.CURRENT_MENU = ProgramState.PROGRAM_MENUES.EMPLOYEES;
                            }
                            else
                            {
                                employeeToUpdate = await Menus.UpdateEmployeeMenu(employeeToUpdate);
                                if(employeeToUpdate.Password == null || employeeToUpdate.Role == null)
                                {
                                    ProgramState.CURRENT_MENU = ProgramState.PROGRAM_MENUES.EMPLOYEES;
                                }
                                else
                                {
                                    await rep.UpdateEmployee(employeeToUpdate);
                                    await Menus.ConfirmationScreen("Anställd uppdaterad.");
                                }
                            }

                        }
                        break;
                    case ProgramState.PROGRAM_MENUES.SHOW_EMPLOYEE:
                        {
                            bool wantToGoBack = false;

                            while (wantToGoBack == false)
                            {
                                Console.Clear();
                                Console.WriteLine("~~ SAMTLIGA ANSTÄLLDA ~~");
                                Console.WriteLine();
                                foreach (var employees in await rep.GetAllEmployees())
                                {
                                    Console.WriteLine(employees);
                                }

                                Console.WriteLine();
                                Console.WriteLine("Klicka på ESC för att gå tillbaka");
                                if (Console.ReadKey(true).Key == ConsoleKey.Escape)
                                {
                                    ProgramState.CURRENT_MENU = ProgramState.PROGRAM_MENUES.EMPLOYEES;
                                    break;
                                }
                            }
                        }
                        break;
                    case ProgramState.PROGRAM_MENUES.DELETE_EMPLOYEE:
                        {
                            Employee employeeToDelete = await Menus.DeleteEmployeeMenu(rep);
                            if(employeeToDelete.Password == null) { ProgramState.CURRENT_MENU = ProgramState.PROGRAM_MENUES.EMPLOYEES; }
                            else
                            {
                                await rep.DeleteEmployee(employeeToDelete);
                                await Menus.ConfirmationScreen("Anställd borttagen.");
                            }
                        }
                        break;
                    #endregion

                    #region// Pizzas
                    case ProgramState.PROGRAM_MENUES.PIZZAS:
                        {
                            var UserChoice = await Menus.PrintMenuAndCheckChoice(Menus.MenuChoices[2]);
                            {
                                if (UserChoice.Item1 == true)
                                {
                                    if (UserChoice.Item2 == 1) { ProgramState.CURRENT_MENU = ProgramState.PROGRAM_MENUES.ADD_PIZZA; }
                                    else if (UserChoice.Item2 == 2) { ProgramState.CURRENT_MENU = ProgramState.PROGRAM_MENUES.UPDATE_PIZZA; }
                                    else if (UserChoice.Item2 == 3) { ProgramState.CURRENT_MENU = ProgramState.PROGRAM_MENUES.SHOW_PIZZA; }
                                    else if (UserChoice.Item2 == 4) { ProgramState.CURRENT_MENU = ProgramState.PROGRAM_MENUES.DELETE_PIZZA; }
                                    else if (UserChoice.Item2 == 5) { ProgramState.CURRENT_MENU = ProgramState.PROGRAM_MENUES.MAIN_MENU; }
                                    else await Menus.MessageIfChoiceIsNotRight("Ditt val är felaktigt.", "Valet finns inte.");
                                }
                                else await Menus.MessageIfChoiceIsNotRight("Ditt val är felaktigt.");
                            }
                        }
                        break;
                    case ProgramState.PROGRAM_MENUES.ADD_PIZZA:
                        {
                            Pizza newPizza = new Pizza();
                            newPizza = await Menus.AddPizzaMenu(rep);
                            if(newPizza.Type == null || newPizza.PizzabaseID == 0 || newPizza.Price <= 0)
                            {
                                ProgramState.CURRENT_MENU = ProgramState.PROGRAM_MENUES.PIZZAS;
                            }
                            else
                            {
                                //using (IDbTransaction transaction = await rep.Transaction())
                                //{
                                    newPizza = await rep.AddPizza(newPizza);
                                    newPizza.PizzaIngredients = await Menus.AddCondimentToPizzaMenu(rep, newPizza);
                                    if(newPizza.PizzaIngredients.Count < 1) 
                                    {
                                        await rep.DeletePizza(newPizza);
                                        ProgramState.CURRENT_MENU = ProgramState.PROGRAM_MENUES.PIZZAS; 
                                    }
                                    else
                                    {
                                        await rep.AddCondimentToPizza(newPizza);
                                        await Menus.ConfirmationScreen("Pizzan tillagd.");
                                        ProgramState.CURRENT_MENU = ProgramState.PROGRAM_MENUES.PIZZAS;
                                    }
                                //}

                            }

                        }
                        break;
                    case ProgramState.PROGRAM_MENUES.UPDATE_PIZZA:
                        {
                            Console.WriteLine("Klicka på ESC för att gå tillbaka.");
                            Console.Write("Ange ID på den pizza du vill ändra: ");
                            string pizzaIDToBeUpdated = await Menus.ReadLineWithOptionToGoBack();
                            if(pizzaIDToBeUpdated == null) { ProgramState.CURRENT_MENU = ProgramState.PROGRAM_MENUES.PIZZAS; }
                            else
                            {
                                bool correctInput = int.TryParse(pizzaIDToBeUpdated, out int IDOfPizzaToBeUpdated);
                                if(correctInput == true)
                                {
                                    bool checkIfPizzaExists = await rep.CheckIfPizzaIDExists(IDOfPizzaToBeUpdated);

                                    if(checkIfPizzaExists == true)
                                    {
                                        Pizza pizzaToBeUpdated = new Pizza();
                                        pizzaToBeUpdated = await rep.GetSinglePizza(IDOfPizzaToBeUpdated);
                                        pizzaToBeUpdated = await Menus.UpdatePizzaMenu(rep, pizzaToBeUpdated);

                                        if(pizzaToBeUpdated.PizzabaseID == 0 || pizzaToBeUpdated.PizzaIngredients == null || pizzaToBeUpdated.Type == null || pizzaToBeUpdated.Price == 0)
                                        {
                                            ProgramState.CURRENT_MENU = ProgramState.PROGRAM_MENUES.PIZZAS;
                                        }
                                        else
                                        {
                                            await rep.UpdatePizza(pizzaToBeUpdated);
                                            await Menus.ConfirmationScreen("Pizzan uppdaterad.");
                                            ProgramState.CURRENT_MENU = ProgramState.PROGRAM_MENUES.PIZZAS;
                                        }

                                    }
                                    else { await Menus.MessageIfChoiceIsNotRight("Angiven pizza finns inte."); }
                                }
                                else { await Menus.MessageIfChoiceIsNotRight("Felaktig inmatning."); }

                            }
                        }
                        break;
                    case ProgramState.PROGRAM_MENUES.SHOW_PIZZA:
                        {
                                Console.Clear();
                                Console.WriteLine("~~ SAMTLIGA PIZZOR ~~");
                                Console.WriteLine();
                                foreach (Pizza pizza in Helpers.LoadPizzasAsList(rep))
                                {
                                    Console.WriteLine($"{pizza.PizzaID}. {pizza.Type}  {pizza.Price} kr");
                                    for (int index = 0; index < pizza.PizzaIngredients.Count; index++)
                                    {
                                        if(index == 0) { Console.Write($"{pizza.PizzaIngredients[index].Type}"); }
                                        else { Console.Write($", {pizza.PizzaIngredients[index].Type}"); }
                                    }
                                        
                                    Console.WriteLine("\n");
                                }

                                Console.WriteLine();
                                Console.WriteLine("Klicka på ESC för att gå tillbaka");
                                if (Console.ReadKey(true).Key == ConsoleKey.Escape)
                                {
                                    ProgramState.CURRENT_MENU = ProgramState.PROGRAM_MENUES.PIZZAS;
                                    break;
                                }
                                else { await Menus.MessageIfChoiceIsNotRight("Vänligen klicka på ESC för att återgå."); }
                        }
                        break;

                    case ProgramState.PROGRAM_MENUES.DELETE_PIZZA:
                        {
                            Pizza pizzaToDelete = await Menus.DeletePizzaMenu(rep);

                            if(pizzaToDelete == null) { ProgramState.CURRENT_MENU = ProgramState.PROGRAM_MENUES.PIZZAS; }
                            else
                            {
                                await rep.DeletePizza(pizzaToDelete);
                                await Menus.ConfirmationScreen("Pizzan raderad.");
                                ProgramState.CURRENT_MENU = ProgramState.PROGRAM_MENUES.PIZZAS;
                            }
                        }
                        break;
                    #endregion

                    #region// Ingredients
                    case ProgramState.PROGRAM_MENUES.INGREDIENTS:
                        {
                            var UserChoice = await Menus.PrintMenuAndCheckChoice(Menus.MenuChoices[3]);
                            {
                                if (UserChoice.Item1 == true)
                                {
                                    if (UserChoice.Item2 == 1) { ProgramState.CURRENT_MENU = ProgramState.PROGRAM_MENUES.ADD_INGREDIENT; }
                                    else if (UserChoice.Item2 == 2) { ProgramState.CURRENT_MENU = ProgramState.PROGRAM_MENUES.UPDATE_INGREDIENT; }
                                    else if (UserChoice.Item2 == 3) { ProgramState.CURRENT_MENU = ProgramState.PROGRAM_MENUES.SHOW_INGREDIENT; }
                                    else if (UserChoice.Item2 == 4) { ProgramState.CURRENT_MENU = ProgramState.PROGRAM_MENUES.DELETE_INGREDIENT; }
                                    else if (UserChoice.Item2 == 5) { ProgramState.CURRENT_MENU = ProgramState.PROGRAM_MENUES.MAIN_MENU; }
                                    else await Menus.MessageIfChoiceIsNotRight("Ditt val är felaktigt.", "Valet finns inte.");
                                }
                                else await Menus.MessageIfChoiceIsNotRight("Ditt val är felaktigt.");
                            }
                        }
                        break;

                    case ProgramState.PROGRAM_MENUES.ADD_INGREDIENT:
                        {
                            Condiment cond = new Condiment();
                            cond.Type = null; cond.Price = 0;                          

                            Console.Clear();
                            Console.WriteLine("~~ LÄGG TILL INGREDIENS ~~");
                            Console.WriteLine("Klicka på ESC för att gå tillbaka.");

                            Console.WriteLine();
                            Console.Write("Vilken ingrediens vill du lägga till: ");
                            string condimentName = await Menus.ReadLineWithOptionToGoBack();

                            if(condimentName == null)
                            {
                                ProgramState.CURRENT_MENU = ProgramState.PROGRAM_MENUES.INGREDIENTS;
                                break;
                            }
                            else if(condimentName.Length < 1 || !Regex.IsMatch(condimentName, @"^[a-öA-Ö]+$")) { await Menus.MessageIfChoiceIsNotRight("Felaktig inmatning."); }
                            else { cond.Type = condimentName; }

                            Console.Write("Ange pris för ingrediensen: ");
                            string inputPrice = await Menus.ReadLineWithOptionToGoBack();
                            bool correctInput = float.TryParse(inputPrice, out float price);

                            if (inputPrice == null || correctInput == false || price == 0)
                            {
                                Thread.Sleep(500);
                                ProgramState.CURRENT_MENU = ProgramState.PROGRAM_MENUES.INGREDIENTS;
                                break;
                            }
                            else { cond.Price = price; }

                            Console.Clear();
                            Console.WriteLine("~~ LÄGG TILL INGREDIENS ~~\n");
                            Console.Write($"Vill du lägga till artikeln i ingredienser?\n\n{cond.Type} {cond.Price}kr\n\nj/n: ");
                            string confirm = Console.ReadLine();

                            if (confirm.ToLower() == "j") {await rep.AddCondiment(cond); }
                            else { ProgramState.CURRENT_MENU = ProgramState.PROGRAM_MENUES.INGREDIENTS; break; }
                        }
                        break;

                    case ProgramState.PROGRAM_MENUES.UPDATE_INGREDIENT:
                        {
                            Console.Clear();
                            Console.WriteLine("~~ UPPDATERA INGREDIENS ~~\n");
                            foreach (var ingredient in await rep.GetAllCondiments())
                            {
                                Console.WriteLine($"{ingredient.CondimentID} - {ingredient.Type}, {ingredient.Price} kr\n");
                            }
                            Console.WriteLine("Klicka på ESC för att gå tillbaka.");

                            Console.Write("Ange ID på den ingrediens som du vill uppdatera: ");
                            string IDToChange = await Menus.ReadLineWithOptionToGoBack();

                            int.TryParse(IDToChange, out int ID);
                            //Fixat SP så man kan kolla om användaren man vill ändra finns i databasen
                            bool doesIngredientExist = (await rep.CheckIfCondimentIDExists(ID));

                            if (IDToChange == null)
                            {
                                ProgramState.CURRENT_MENU = ProgramState.PROGRAM_MENUES.INGREDIENTS;
                            }

                            else if (doesIngredientExist == false && IDToChange != null)
                            {
                                await Menus.MessageIfChoiceIsNotRight("Ingrediensen med angivet ID finns inte.");
                            }

                            else
                            {
                                Condiment cond = await rep.GetSingleCondiment(ID);

                                Console.Clear();
                                Console.WriteLine("Klicka ESC för att gå tillbaka.\n");
                                Console.Write("Ange ingrediensens nya pris: ");

                                string newPrice = await Menus.ReadLineWithOptionToGoBack();
                                float.TryParse(newPrice, out float userInput);

                                if (newPrice == null || userInput <= 0) // || userInput == false?
                                {
                                    ProgramState.CURRENT_MENU = ProgramState.PROGRAM_MENUES.UPDATE_INGREDIENT;
                                }
                                // ändra roll
                                else if (userInput > 0)
                                {              
                                    cond.Price = userInput;
                                    await rep.UpdateCondiment(cond);
                                    Thread.Sleep(500);
                                    await Menus.ConfirmationScreen("Priset ändrat.");
                                    
                                }
                            }
                        }
                        break;        

                    case ProgramState.PROGRAM_MENUES.SHOW_INGREDIENT:
                        {
                            Console.Clear();
                            Console.WriteLine($"~~ SAMTLIGA INGREDIENSER ~~\n");

                            foreach (var ingredient in await rep.GetAllCondiments())
                            {
                                Console.WriteLine($"{ingredient.Type}, {ingredient.Price} kr\n");
                            }
                            
                            Console.WriteLine($"\nKlicka på ESC för att gå tillbaka");
                            if (Console.ReadKey(true).Key == ConsoleKey.Escape)
                            {
                                ProgramState.CURRENT_MENU = ProgramState.PROGRAM_MENUES.INGREDIENTS;
                                break;
                            }
                            else { await Menus.MessageIfChoiceIsNotRight("Vänligen klicka på ESC för att återgå."); }
                        }
                        break;

                    case ProgramState.PROGRAM_MENUES.DELETE_INGREDIENT:
                        {
                            Console.Clear();
                            Console.WriteLine("~~ TA BORT INGREDIENS ~~");
                            Console.WriteLine("Klicka på ESC för att gå tillbaka.");
                            Console.WriteLine();

                            Condiment c = new Condiment();

                            foreach (var ingredient in await rep.GetAllCondiments())
                            {
                                Console.WriteLine($"ID - {ingredient.CondimentID}, {ingredient.Type}\n");
                            }

                            Console.Write("Ange ID för den ingrediens som du vill ta bort: ");

                            string IDOfIngredientToRemove = await Menus.ReadLineWithOptionToGoBack();
                            bool userInput = int.TryParse(IDOfIngredientToRemove, out int ID);
                            bool doesIngredientExist = (await rep.CheckIfCondimentIDExists(ID));

                            if (IDOfIngredientToRemove == null)
                            {
                                ProgramState.CURRENT_MENU = ProgramState.PROGRAM_MENUES.INGREDIENTS;
                            }

                            else if (doesIngredientExist == false)
                            {
                                await Menus.MessageIfChoiceIsNotRight("Ingrediens med angivet ID finns inte.");
                            }

                            else if (userInput == true)
                            {
                                Console.Clear();
                                Console.WriteLine($"Är du säker på att du vill ta bort ingrediens {ID} (j/n)? Valet kan inte ändras.");

                                string choice = await Menus.ReadLineWithOptionToGoBack();

                                if (choice == null) { ProgramState.CURRENT_MENU = ProgramState.PROGRAM_MENUES.INGREDIENTS; }
                                else if (choice == "j")
                                {
                                    await rep.DeleteCondiment(c);
                                    await Menus.ConfirmationScreen("Ingrediens borttagen");
                                    ProgramState.CURRENT_MENU = ProgramState.PROGRAM_MENUES.INGREDIENTS;

                                }
                                else if (choice == "n") { ProgramState.CURRENT_MENU = ProgramState.PROGRAM_MENUES.DELETE_INGREDIENT; }
                                else { await Menus.MessageIfChoiceIsNotRight("Vänligen svara med j eller n."); }
                            }
                        }
                        break;
                    #endregion

                    #region// Extras
                    case ProgramState.PROGRAM_MENUES.EXTRAS:
                        {
                            var UserChoice = await Menus.PrintMenuAndCheckChoice(Menus.MenuChoices[4]);

                            {
                                if (UserChoice.Item1 == true)
                                {
                                    if (UserChoice.Item2 == 1) { ProgramState.CURRENT_MENU = ProgramState.PROGRAM_MENUES.ADD_EXTRAS; }
                                    else if (UserChoice.Item2 == 2) { ProgramState.CURRENT_MENU = ProgramState.PROGRAM_MENUES.UPDATE_EXTRAS; }
                                    else if (UserChoice.Item2 == 3) { ProgramState.CURRENT_MENU = ProgramState.PROGRAM_MENUES.SHOW_EXTRAS; }
                                    else if (UserChoice.Item2 == 4) { ProgramState.CURRENT_MENU = ProgramState.PROGRAM_MENUES.DELETE_EXTRAS; }
                                    else if (UserChoice.Item2 == 5) { ProgramState.CURRENT_MENU = ProgramState.PROGRAM_MENUES.MAIN_MENU; }
                                    else await Menus.MessageIfChoiceIsNotRight("Ditt val är felaktigt.", "Valet finns inte.");
                                }
                                else await Menus.MessageIfChoiceIsNotRight("Ditt val är felaktigt.");
                            }
                        }
                        break;

                    case ProgramState.PROGRAM_MENUES.ADD_EXTRAS:
                        {
                            Extra extra = new Extra();
                            extra.Type = null; extra.Price = 0;

                            Console.Clear();
                            Console.WriteLine("~~ LÄGG TILL TILLBEHÖR ~~");
                            Console.WriteLine("Klicka på ESC för att gå tillbaka.");

                            Console.WriteLine();
                            Console.Write("Vilket tillbehör vill du lägga till: ");
                            string extraName = await Menus.ReadLineWithOptionToGoBack();
                            
                            if (extraName == null || extraName.Length < 1 || !Regex.IsMatch(extraName, @"^[a-öA-Ö\s]+$")) // \s är för att inkludera whitespace
                            {
                                Thread.Sleep(500);
                                ProgramState.CURRENT_MENU = ProgramState.PROGRAM_MENUES.EXTRAS;
                                break;
                            }
                            else { extra.Type = extraName; }

                            Console.Write("Ange pris för tillbehöret: ");
                            string inputPrice = await Menus.ReadLineWithOptionToGoBack();
                            bool correctInput = float.TryParse(inputPrice, out float price);

                            if (inputPrice == null)
                            {
                                ProgramState.CURRENT_MENU = ProgramState.PROGRAM_MENUES.EXTRAS;
                                break;
                            }
                            else if(correctInput == false || price == 0) { await Menus.MessageIfChoiceIsNotRight("Felaktig inmatning."); }
                            else { extra.Price = price; }

                            Console.Clear();
                            Console.WriteLine("~~ LÄGG TILL TILLBEHÖR ~~\n");
                            Console.Write($"Vill du lägga till artikeln i tillbehör?\n\n{extra.Type} {extra.Price}kr\n\nj/n: ");
                            string confirm = Console.ReadLine();

                            if (confirm.ToLower() == "j") { await rep.AddExtra(extra); } //Break här?
                            else { ProgramState.CURRENT_MENU = ProgramState.PROGRAM_MENUES.EXTRAS; break; }
                        }
                        break;                    
                    case ProgramState.PROGRAM_MENUES.UPDATE_EXTRAS:
                        {
                            Console.Clear();
                            Console.WriteLine("~~ UPPDATERA TILLBEHÖR ~~\n");
                            foreach (var extra in await rep.GetAllExtras())
                            {
                                Console.WriteLine($"{extra.ProductID} - {extra.Type}, {extra.Price} kr\n");
                            }
                            Console.WriteLine("Klicka på ESC för att gå tillbaka.");

                            Console.Write("Ange ID på det tillbehör som du vill uppdatera: ");
                            string IDToChange = await Menus.ReadLineWithOptionToGoBack();

                            int.TryParse(IDToChange, out int ID);
                            //Fixat SP så man kan kolla om användaren man vill ändra finns i databasen
                            bool doesExtraExist = (await rep.CheckIfProductIDExists(ID));

                            if (IDToChange == null)
                            {
                                ProgramState.CURRENT_MENU = ProgramState.PROGRAM_MENUES.EXTRAS;
                            }

                            else if (doesExtraExist == false && IDToChange != null)
                            {
                                await Menus.MessageIfChoiceIsNotRight("Tillbehöret med angivet ID finns inte.");
                            }

                            else
                            {
                                Extra extra = await rep.GetSingleExtra(ID);

                                Console.Clear();
                                Console.WriteLine("Klicka ESC för att gå tillbaka.\n");
                                Console.Write("Ange tillbehörets nya pris: ");

                                string newPrice = await Menus.ReadLineWithOptionToGoBack();
                                float.TryParse(newPrice, out float userInput);

                                if (newPrice == null || userInput <= 0) // || userInput == false?
                                {
                                    ProgramState.CURRENT_MENU = ProgramState.PROGRAM_MENUES.UPDATE_EXTRAS;
                                }
                                // ändra roll
                                else if (userInput > 0)
                                {
                                    extra.Price = userInput;
                                    await rep.UpdateExtra(extra);
                                    Thread.Sleep(500);
                                    await Menus.ConfirmationScreen("Priset ändrat.");

                                }
                            }
                        }
                        break;
                    /*
                    Denna är tillagd utan att pusha till GIT
                    */
                    case ProgramState.PROGRAM_MENUES.SHOW_EXTRAS:
                        {
                            Console.Clear();
                            Console.WriteLine($"~~ SAMTLIGA TILLBEHÖR ~~\n");

                            foreach (var extra in await rep.GetAllExtras())
                            {
                                Console.WriteLine($"{extra.Type}, {extra.Price} kr\n");
                            }

                            Console.WriteLine($"\nKlicka på ESC för att gå tillbaka");
                            if (Console.ReadKey(true).Key == ConsoleKey.Escape)
                            {
                                ProgramState.CURRENT_MENU = ProgramState.PROGRAM_MENUES.EXTRAS;
                                break;
                            }
                            else { await Menus.MessageIfChoiceIsNotRight("Vänligen klicka på ESC för att återgå."); }
                        }
                        break;
                    ///
                    ///Denna är tillagd utan att pusha till GIT
                    ///Lagt till så att man får upp en lista på allt som är möjligt att radera, visas med ID och typ
                    ///
                    case ProgramState.PROGRAM_MENUES.DELETE_EXTRAS:
                        {
                            Console.Clear();
                            Console.WriteLine("~~ TA BORT TILLBEHÖR ~~");
                            Console.WriteLine("Klicka på ESC för att gå tillbaka.");
                            Console.WriteLine();

                            Extra e = new Extra();

                            foreach (var extra in await rep.GetAllExtras())
                            {
                                Console.WriteLine($"ID - {extra.ProductID}, {extra.Type}\n");
                            }

                            Console.Write("Ange ID för det tillbehör som du vill ta bort: ");

                            string IDOfExtraToRemove = await Menus.ReadLineWithOptionToGoBack();
                            bool userInput = int.TryParse(IDOfExtraToRemove, out int ID);
                            bool doesExtraExist = (await rep.CheckIfProductIDExists(ID));

                            if (IDOfExtraToRemove == null)
                            {
                                ProgramState.CURRENT_MENU = ProgramState.PROGRAM_MENUES.EXTRAS;
                            }

                            else if (doesExtraExist == false)
                            {
                                await Menus.MessageIfChoiceIsNotRight("Tillbehör med angivet ID finns inte.");
                            }

                            else if (userInput == true)
                            {
                                Console.Clear();
                                Console.WriteLine($"Är du säker på att du vill ta bort tillbehör {ID} (j/n)? Valet kan inte ändras.");

                                string choice = await Menus.ReadLineWithOptionToGoBack();

                                if (choice == null) { ProgramState.CURRENT_MENU = ProgramState.PROGRAM_MENUES.EXTRAS; }
                                else if (choice == "j")
                                {
                                    await rep.DeleteExtra(e);
                                    await Menus.ConfirmationScreen("Tillbehör borttaget");
                                    ProgramState.CURRENT_MENU = ProgramState.PROGRAM_MENUES.EXTRAS;

                                }
                                else if (choice == "n") { ProgramState.CURRENT_MENU = ProgramState.PROGRAM_MENUES.DELETE_EXTRAS; }
                                else { await Menus.MessageIfChoiceIsNotRight("Vänligen svara med j eller n."); }
                            }
                        }
                        break;
                    #endregion

                    #region // Old orders
                    case ProgramState.PROGRAM_MENUES.OLD_ORDERS:
                        {
                            var UserChoice = await Menus.PrintMenuAndCheckChoice(Menus.MenuChoices[5]);

                            {
                                if (UserChoice.Item1 == true)
                                {
                                    if (UserChoice.Item2 == 1) { ProgramState.CURRENT_MENU = ProgramState.PROGRAM_MENUES.SHOW_OLD_ORDERS; }
                                    else if (UserChoice.Item2 == 2) { ProgramState.CURRENT_MENU = ProgramState.PROGRAM_MENUES.DELETE_OLD_ORDERS; }
                                    else if (UserChoice.Item2 == 3) { ProgramState.CURRENT_MENU = ProgramState.PROGRAM_MENUES.MAIN_MENU; }
                                    else await Menus.MessageIfChoiceIsNotRight("Ditt val är felaktigt.", "Valet finns inte.");
                                }
                                else await Menus.MessageIfChoiceIsNotRight("Ditt val är felaktigt.");
                            }
                        }
                        break;
                    case ProgramState.PROGRAM_MENUES.SHOW_OLD_ORDERS:
                        {
                            Console.Clear();
                            Console.WriteLine("~~ SAMTLIGA GAMLA ORDRAR ~~");
                            Console.WriteLine();
                            var result = await rep.GetAllOrders();
                            var orders = result.Where(x => x.Status == 4);

                            foreach (var order in orders)
                            {
                                Console.WriteLine(order);
                            }

                            Console.WriteLine();
                            Console.WriteLine("Klicka på ESC för att gå tillbaka");
                            if (Console.ReadKey(true).Key == ConsoleKey.Escape)
                            {
                                ProgramState.CURRENT_MENU = ProgramState.PROGRAM_MENUES.EMPLOYEES;
                                break;
                            }
                        }
                        break;
                    case ProgramState.PROGRAM_MENUES.DELETE_OLD_ORDERS:
                        break;
                    #endregion

                    default:
                        ProgramState.Running = false;
                        break;
                }
            }
        }
        /// <summary>
        /// Load all pizzas
        /// </summary>
        /// <returns></returns>

    }
}
