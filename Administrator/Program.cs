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
            //Current directory och Backend.cfg ligger på olika ställen. För att byta backend så får flytta # i bin-foldern i admin.
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
                            ProgramState.CURRENT_MENU = ProgramState.PROGRAM_MENUES.MAIN_MENU;
                            Console.Clear();
                            Console.WriteLine("~ VÄLKOMMEN TILL ADMINPANELEN ~");
                            bool CorrectLogin = (await Menus.PrintAndReturnStateOfLogin(rep, "admin")).Item1;
                            
                            if (CorrectLogin == true)
                            {
                                ProgramState.CURRENT_MENU = ProgramState.PROGRAM_MENUES.MAIN_MENU;
                            }
                            else await Menus.PrintAndReturnStateOfLogin(rep, "admin");
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
                                    ProgramState.CURRENT_MENU = ProgramState.PROGRAM_MENUES.EMPLOYEES;
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
                            Console.Clear();
                            Console.WriteLine("~~ UPPDATERA Pizza ~~");
                            Console.WriteLine("Klicka ESC för att gå tillbaka.");

                            Console.Write("\nAnge ID för den pizza du vill ändra: ");
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
                                        Console.Clear();
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
                            Condiment newCondiment = await Menus.AddCondimentMenu(rep);
                            if(newCondiment == null) { ProgramState.CURRENT_MENU = ProgramState.PROGRAM_MENUES.INGREDIENTS; }
                            else
                            {
                                await rep.AddCondiment(newCondiment);
                                await Menus.ConfirmationScreen("Ingrediens tillagd.");
                                ProgramState.CURRENT_MENU = ProgramState.PROGRAM_MENUES.INGREDIENTS;
                            }
                        }
                        break;

                    case ProgramState.PROGRAM_MENUES.UPDATE_INGREDIENT:
                        {
                            start:
                            Console.Clear();
                            Console.WriteLine("~~ UPPDATERA INGREDIENS ~~");
                            Console.WriteLine("Klicka ESC för att gå tillbaka.");

                            Console.Write("\nAnge ID för den ingrediens du vill ändra: ");
                            string IDOfCondimentToUpdate = await Menus.ReadLineWithOptionToGoBack();
                            if(IDOfCondimentToUpdate == null) { ProgramState.CURRENT_MENU = ProgramState.PROGRAM_MENUES.INGREDIENTS; }
                            else
                            {
                                bool correctInput = int.TryParse(IDOfCondimentToUpdate, out int IDToCheck);
                                if(correctInput == true)
                                {
                                    bool checkIfIDExists = await rep.CheckIfCondimentIDExists(IDToCheck);
                                    if(checkIfIDExists == true)
                                    {
                                        Condiment condimentToUpdate = await rep.GetSingleCondiment(IDToCheck);
                                        condimentToUpdate = await Menus.UpdateCondimentMenu(rep, condimentToUpdate);
                                        if(condimentToUpdate == null) { ProgramState.CURRENT_MENU = ProgramState.PROGRAM_MENUES.INGREDIENTS; }
                                        else
                                        {
                                            await rep.UpdateCondiment(condimentToUpdate);
                                            await Menus.ConfirmationScreen("Ingrediens uppdaterad.");
                                            ProgramState.CURRENT_MENU = ProgramState.PROGRAM_MENUES.INGREDIENTS;
                                        }
                                    }
                                    else
                                    {
                                        await Menus.MessageIfChoiceIsNotRight("Angivet ID finns inte.");
                                        goto start;
                                    }
                                }
                                else { await Menus.MessageIfChoiceIsNotRight("Felaktig inmatning."); goto start; }

                            }

                        }
                        break;        

                    case ProgramState.PROGRAM_MENUES.SHOW_INGREDIENT:
                        {
                            Console.Clear();
                            Console.WriteLine($"~~ SAMTLIGA INGREDIENSER ~~\n");

                            foreach (var ingredient in await rep.GetAllCondiments())
                            {
                                Console.WriteLine($"{ingredient.CondimentID}. {ingredient.Type}, {ingredient.Price} kr");
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
                            Condiment condimentToDelete = await Menus.DeleteCondimentMenu(rep);
                            if(condimentToDelete == null) { ProgramState.CURRENT_MENU = ProgramState.PROGRAM_MENUES.INGREDIENTS; }
                            else
                            {
                                await rep.DeleteCondiment(condimentToDelete);
                                await Menus.ConfirmationScreen("Ingrediensen borttagen.");
                                ProgramState.CURRENT_MENU = ProgramState.PROGRAM_MENUES.INGREDIENTS;
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
                            Extra newExtra = await Menus.AddExtraMenu(rep);
                            if(newExtra == null) { ProgramState.CURRENT_MENU = ProgramState.PROGRAM_MENUES.EXTRAS; }
                            else
                            {
                                await rep.AddExtra(newExtra);
                                await Menus.ConfirmationScreen("Tillbehör tillagt.");
                                ProgramState.CURRENT_MENU = ProgramState.PROGRAM_MENUES.EXTRAS;
                            }
                        }
                        break;                    
                    case ProgramState.PROGRAM_MENUES.UPDATE_EXTRAS:
                        {
                        start:
                            
                            Console.Clear();
                            Console.WriteLine("~~ UPPDATERA TILLBEHÖR ~~\n");
                            Console.WriteLine("Klicka på ESC för att gå tillbaka.");

                            Console.Write("\n\nAnge ID på det tillbehör som ska ändras: ");
                            string IDOfExtraToUpdate = await Menus.ReadLineWithOptionToGoBack();
                            if(IDOfExtraToUpdate == null) { ProgramState.CURRENT_MENU = ProgramState.PROGRAM_MENUES.EXTRAS; }
                            else
                            {
                                bool correctInput = int.TryParse(IDOfExtraToUpdate, out int IDOfExtra);
                                if(correctInput == true)
                                {           
                                    bool checkIfIDExists = await rep.CheckIfProductIDExists(IDOfExtra);
                                    if(checkIfIDExists == true)
                                    {
                                        //Tillagt för att ha ett objekt att skicka in.
                                        Extra extraToUpdate = await rep.GetSingleExtra(IDOfExtra);
                                        extraToUpdate = await Menus.UpdateExtraMenu(rep, extraToUpdate);
                                        if(extraToUpdate == null) { ProgramState.CURRENT_MENU = ProgramState.PROGRAM_MENUES.EXTRAS; }
                                        else
                                        {
                                            await rep.UpdateExtra(extraToUpdate);
                                            await Menus.ConfirmationScreen("Tillbehöret uppdaterat.");
                                            ProgramState.CURRENT_MENU = ProgramState.PROGRAM_MENUES.EXTRAS;
                                        }
                                    }
                                    else { await Menus.MessageIfChoiceIsNotRight("Det angivna ID:t finns inte."); goto start; }
                                }
                                else { await Menus.MessageIfChoiceIsNotRight("Felaktig inmatning."); goto start; }
                            }
                        }
                        break;
                    case ProgramState.PROGRAM_MENUES.SHOW_EXTRAS:
                        {
                            Console.Clear();
                            Console.WriteLine($"~~ SAMTLIGA TILLBEHÖR ~~\n");

                            foreach (var extra in await rep.GetAllExtras())
                            {
                                Console.WriteLine($"{extra.ProductID}. {extra.Type}, {extra.Price} kr");
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
                    case ProgramState.PROGRAM_MENUES.DELETE_EXTRAS:
                        {
                            Extra extraToDelete = await Menus.DeleteExtraMenu(rep);
                            if(extraToDelete == null) { ProgramState.CURRENT_MENU = ProgramState.PROGRAM_MENUES.EXTRAS; }
                            else
                            {
                                await rep.DeleteExtra(extraToDelete);
                                await Menus.ConfirmationScreen("Tillbehöret borttaget.");
                                ProgramState.CURRENT_MENU = ProgramState.PROGRAM_MENUES.EXTRAS;
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
                    case ProgramState.PROGRAM_MENUES.SHOW_OLD_ORDERS: // Fungerar. Visar bara ordrar som gått igenom utlämning och är därmed gamla
                        {
                            Console.Clear();
                            Console.WriteLine("~~ VISA GAMLA ORDRAR ~~");
                            Console.WriteLine();

                            var result = await rep.GetOrderByStatus(3);
       
                            foreach (var order in result)
                            {
                                Console.WriteLine($"ID: {order.OrderID} Pris: {order.Price}");
                                Console.Write($"Pizzor: ");

                                foreach (var item in order.PizzaList)
                                {
                                    Console.Write($"{item.Type}, ");
                                }

                                Console.WriteLine();
                                Console.Write("Tillbehör: ");

                                foreach (var item in order.ExtraList)
                                {
                                    Console.Write($"{item.Type}, ");
                                }
                            }

                            Console.WriteLine();
                            Console.WriteLine("\nKlicka på ESC för att gå tillbaka");
                            if (Console.ReadKey(true).Key == ConsoleKey.Escape)
                            {
                                ProgramState.CURRENT_MENU = ProgramState.PROGRAM_MENUES.OLD_ORDERS;
                                break;
                            }
                        }
                        break;
                    case ProgramState.PROGRAM_MENUES.DELETE_OLD_ORDERS: // Fungerar. Gör koll mot databasen för att försäkra om att man inte skriver in en aktiv order.
                        {
                            {
                                Order orderToDelete = await Menus.DeleteOldOrderMenu(rep);
                                if (orderToDelete == null) { ProgramState.CURRENT_MENU = ProgramState.PROGRAM_MENUES.OLD_ORDERS; }
                                else
                                {
                                    await rep.DeleteOldOrder(orderToDelete);
                                    await Menus.ConfirmationScreen("Ordern borttagen.");
                                    ProgramState.CURRENT_MENU = ProgramState.PROGRAM_MENUES.OLD_ORDERS;
                                }
                            }
                        }
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
