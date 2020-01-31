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
                            bool correctPass = false, correctRole = false;
                            Employee emp = new Employee();

                            Console.Clear();
                            // MENU CHOICES
                            {
                                do
                                {
                                    Console.WriteLine("~~ LÄGG TILL ANSTÄLLD ~~");
                                    Console.WriteLine("Klicka på ESC för att gå tillbaka.");

                                    Console.Write("Ange den anställdes lösenord: ");
                                    string tempPassword = await Menus.ReadLineWithOptionToGoBack();

                                    if (tempPassword == null)
                                    {
                                        ProgramState.CURRENT_MENU = ProgramState.PROGRAM_MENUES.EMPLOYEES;
                                        break;
                                    }

                                    else if (tempPassword.Length < 1)
                                    {
                                        await Menus.MessageIfChoiceIsNotRight("Ditt lösenord måste innehålla tecken.");
                                    }

                                    else
                                    {
                                        emp.Password = tempPassword;
                                        correctPass = true;
                                    }
                                } while (correctPass == false);


                                while (correctRole == false && emp.Password != null)
                                {
                                    Console.Write("Ange den anställdes roll: ");
                                    string tempRole = await Menus.ReadLineWithOptionToGoBack();

                                    if(tempRole == null)
                                    {
                                        Console.Clear();
                                        ProgramState.CURRENT_MENU = ProgramState.PROGRAM_MENUES.EMPLOYEES;
                                        break;
                                    }

                                    else if (tempRole.ToLower() == "admin" || tempRole.ToLower() == "bagare" || tempRole.ToLower() == "kassör")
                                    {
                                        emp.Role = tempRole;
                                        ProgramState.CURRENT_MENU = ProgramState.PROGRAM_MENUES.EMPLOYEES;
                                        correctRole = true;
                                    }

                                    else
                                    {
                                        await Menus.MessageIfChoiceIsNotRight("Rollen finns inte.");
                                        Console.Clear();
                                    }

                                }

                                if (correctPass == true && correctRole == true)
                                {
                                    await rep.AddEmployee(emp);
                                    await Menus.ConfirmationScreen("Den anställde är tillagd.");
                                }
                            }
                        }
                        break;
                    case ProgramState.PROGRAM_MENUES.UPDATE_EMPLOYEE:
                        {

                            Console.Clear();

                            Console.WriteLine("~~ UPPDATERA ANSTÄLLD ~~");
                            Console.WriteLine("Klicka på ESC för att gå tillbaka.");

                            Console.Write("Ange ID på den anställde som du vill uppdatera: ");
                            string IDToChange = await Menus.ReadLineWithOptionToGoBack();

                            int.TryParse(IDToChange, out int ID);
                            //Fixat SP så man kan kolla om användaren man vill ändra finns i databasen
                            bool doesUserExist = (await rep.CheckIfUserIDExists(ID));

                            if(IDToChange == null)
                            {
                                ProgramState.CURRENT_MENU = ProgramState.PROGRAM_MENUES.EMPLOYEES;
                            }

                            else if (doesUserExist == false && IDToChange != null)
                            {
                                await Menus.MessageIfChoiceIsNotRight("Den anställde med angivet ID finns inte.");
                            }

                            else
                            {
                                Employee emp = await rep.GetSingleEmployee(ID);

                                Console.Clear();
                                Console.WriteLine("Klicka ESC för att gå tillbaka.");
                                Console.WriteLine("Ange vad du vill ändra hos användaren:");
                                Console.WriteLine("1. Lösenord");
                                Console.WriteLine("2. Roll");

                                string whatToChange = await Menus.ReadLineWithOptionToGoBack();
                                bool userInput = int.TryParse(whatToChange, out int changeChoice);

                                if(whatToChange == null) // || userInput == false?
                                {
                                    ProgramState.CURRENT_MENU = ProgramState.PROGRAM_MENUES.UPDATE_EMPLOYEE;
                                }

                                else if (userInput == true)
                                {
                                    bool correctPass = false, correctRole = false;
                                    // ändra lösenord
                                    if (changeChoice == 1)
                                    {
                                        while (correctPass == false)
                                        {
                                            Console.Clear();
                                            Console.WriteLine("Klicka ESC för att gå tillbaka.");
                                            Console.Write("Ange den anställdes nya lösenord: ");
                                            string tempPass = await Menus.ReadLineWithOptionToGoBack();
                                            if(tempPass == null)
                                            {
                                                Console.Clear();
                                                ProgramState.CURRENT_MENU = ProgramState.PROGRAM_MENUES.UPDATE_EMPLOYEE;
                                                break;
                                            }

                                            else if (tempPass.Length < 1)
                                            {
                                                await Menus.MessageIfChoiceIsNotRight("Lösenordet måste innehålla tecken.");
                                            }

                                            else
                                            {
                                                emp.Password = tempPass;
                                                await Menus.ConfirmationScreen("Lösenordet ändrat.");
                                                correctPass = true;
                                            }

                                        }

                                        if (correctPass == true)
                                        {
                                            await rep.UpdateEmployee(emp);
                                            await Menus.ConfirmationScreen("Den anställde är uppdaterad.");
                                        }

                                    }

                                    // ändra roll
                                    else if (changeChoice == 2)
                                    {
                                        while (correctRole == false)
                                        {
                                            Console.Clear();
                                            Console.WriteLine("Klicka ESC för att gå tillbaka.");
                                            Console.Write("Ange användarens nya roll: ");

                                            string tempRole = await Menus.ReadLineWithOptionToGoBack();

                                            if(tempRole == null)
                                            {
                                                Console.Clear();
                                                ProgramState.CURRENT_MENU = ProgramState.PROGRAM_MENUES.EMPLOYEES;
                                                break;
                                            }

                                            else if (tempRole.ToLower() == "admin" || tempRole.ToLower() == "bagare" || tempRole.ToLower() == "kassör")
                                            {
                                                emp.Role = tempRole;
                                                await Menus.ConfirmationScreen("Rollen ändrad.");
                                                correctRole = true;
                                            }

                                            else
                                            {
                                                await Menus.MessageIfChoiceIsNotRight("Rollen finns inte.");
                                            }
                                        }

                                        if(correctRole == true)
                                        {
                                            await rep.UpdateEmployee(emp);
                                            await Menus.ConfirmationScreen("Den anställde är uppdaterad.");
                                        }
                                    }
                                }
                                else
                                {
                                    await Menus.MessageIfChoiceIsNotRight("Valet finns inte.");
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
                            Console.Clear();
                            Console.WriteLine("~~ TA BORT ANSTÄLLD ~~");
                            Console.WriteLine("Klicka på ESC för att gå tillbaka.");
                            Console.WriteLine();

                            Console.Write("Ange ID för den anställda som du vill ta bort: ");

                            string IDOfUserToRemove = await Menus.ReadLineWithOptionToGoBack();
                            bool userInput = int.TryParse(IDOfUserToRemove, out int ID);
                            bool doesUserExist = (await rep.CheckIfUserIDExists(ID));

                            if (IDOfUserToRemove == null)
                            {
                                ProgramState.CURRENT_MENU = ProgramState.PROGRAM_MENUES.EMPLOYEES;
                            }

                            else if(doesUserExist == false)
                            {
                                await Menus.MessageIfChoiceIsNotRight("Användare med angivet ID finns inte.");
                            }

                            else if (userInput == true)
                            {
                                Console.WriteLine($"Är du säker på att du vill ta bort anställd {ID} (j/n)? Valet kan inte ändras.");

                                string choice = await Menus.ReadLineWithOptionToGoBack();

                                if(choice == null) { ProgramState.CURRENT_MENU = ProgramState.PROGRAM_MENUES.EMPLOYEES; }
                                else if (choice == "j")
                                {
                                    await rep.DeleteEmployee(ID);
                                    await Menus.ConfirmationScreen("Anställd borttagen");
                                    ProgramState.CURRENT_MENU = ProgramState.PROGRAM_MENUES.EMPLOYEES;

                                }
                                else if (choice == "n"){ ProgramState.CURRENT_MENU = ProgramState.PROGRAM_MENUES.DELETE_EMPLOYEE; }
                                else { await Menus.MessageIfChoiceIsNotRight("Vänligen svara med j eller n."); }
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
                            bool correctPizzaName = false, correctPizzaBase = false, correctPizzaPrice = false;

                            Pizza newPizza = new Pizza();                         

                            // kolla pizzans namn
                            {
                                while (correctPizzaName == false)
                                {
                                    Console.Clear();
                                    Console.WriteLine("~~ LÄGG TILL PIZZA ~~");
                                    Console.WriteLine("Klicka på ESC för att gå tillbaka.");

                                    Console.WriteLine();
                                    Console.Write("Pizzans namn: ");
                                    newPizza.Type = await Menus.ReadLineWithOptionToGoBack();

                                    if(newPizza.Type == null)
                                    {
                                        Console.Clear();
                                        ProgramState.CURRENT_MENU = ProgramState.PROGRAM_MENUES.PIZZAS;
                                        break;
                                    }

                                    else if (newPizza.Type.Length > 1) { correctPizzaName = true; }
                                    else { await Menus.MessageIfChoiceIsNotRight("Namnet måste innehålla minst ett tecken."); }
                                }
                            }
                            // kolla pizzans botten
                            {
                                while (correctPizzaBase == false && newPizza.Type != null)
                                {
                                    Console.Write("Pizzans botten ([1] Italiensk eller [2] Amerikansk): ");
                                    int.TryParse(Console.ReadLine(), out int input);

                                    
                                    newPizza.PizzabaseID = input;
                                    correctPizzaBase = true;
                                    //if(newPizza.Base == null)
                                    //{
                                    //    Console.Clear();
                                    //    ProgramState.CURREN_MENU = ProgramState.PROGRAM_MENUES.PIZZAS;
                                    //    break;
                                    //}

                                    //else if (newPizza.Base.ToLower() == "italiensk" || newPizza.Base.ToLower() == "amerikansk")
                                    //{
                                    //    correctPizzaBase = true;
                                    //}

                                    //else { Menus.MessageIfChoiceIsNotRight("Pizzabasen finns inte."); }
                                }
                            }

                            // kolla pizzans pris
                            {
                                while (correctPizzaPrice == false && newPizza.Type != null && newPizza.PizzabaseID != 0)
                                {
                                   
                                    Console.Write("Pizzans pris: ");
                                    string inputPrice = await Menus.ReadLineWithOptionToGoBack();
                                    bool correctInput = float.TryParse(inputPrice, out float price);

                                    if(inputPrice == null)
                                    {
                                        Console.Clear();
                                        ProgramState.CURRENT_MENU = ProgramState.PROGRAM_MENUES.PIZZAS;
                                        break;
                                    }

                                    else if (correctInput == true && price > 0)
                                    {
                                        newPizza.Price = price;
                                        correctPizzaPrice = true;
                                    }

                                    else { await Menus.MessageIfChoiceIsNotRight("Priset är angivet felaktigt."); }
                                }
                            }

                            newPizza = await rep.AddPizza(newPizza);


                            //kolla pizzans ingredienser
                            {
                                if (newPizza.Type != null && newPizza.PizzabaseID != 0 && newPizza.Price != 0)
                                {
                                    bool confirmedPizza = false;
                                    List<Condiment> condimentsOfNewPizza = new List<Condiment>();

                                    while (confirmedPizza == false)
                                    {
                                        Console.WriteLine("Välj en ingrediens till pizzan genom att ange ingrediensens nummer:");
                                        int counter = 1;

                                        foreach (var condiment in await rep.GetAllCondiments())
                                        {
                                            Console.WriteLine($"{counter}. {condiment.Type}");
                                            counter++;
                                        }

                                        string chosenCondiment = await Menus.ReadLineWithOptionToGoBack();
                                        bool correctInput = int.TryParse(chosenCondiment, out int confirmedChosenCondiment);

                                        if (chosenCondiment == null)
                                        {
                                            Console.Clear();
                                            ProgramState.CURRENT_MENU = ProgramState.PROGRAM_MENUES.PIZZAS;
                                            break;
                                        }

                                        else if (correctInput == true && confirmedChosenCondiment != 0)
                                        {
                                            Condiment cond = await rep.GetSingleCondiment(confirmedChosenCondiment);
                                            cond.Price = 0;
                                            condimentsOfNewPizza.Add(cond);

                                            await Menus.ConfirmationScreen("Ingrediens tillagd.");
                                            Console.Clear();

                                            Console.WriteLine("Ange ytterligare en ingrediens, ange 0 för att bekräfta pizzan.");
                                            Console.WriteLine("Klicka på ESC för att avbryta");
                                            Console.WriteLine();
                                            Console.Write(newPizza.Type + ":\n");
                                            foreach (var condName in condimentsOfNewPizza) { Console.Write($" {condName.Type},"); }
                                            Console.WriteLine();
                                        }

                                        else if (confirmedChosenCondiment == 0 && condimentsOfNewPizza.Count > 0)
                                        {
                                            newPizza.PizzaIngredients = condimentsOfNewPizza;
                                            
                                            await rep.AddCondimentToPizza(newPizza);
                                            await Menus.ConfirmationScreen("Pizzan tillagd");
                                            ProgramState.CURRENT_MENU = ProgramState.PROGRAM_MENUES.PIZZAS;
                                            break;
                                        }

                                        else { await Menus.MessageIfChoiceIsNotRight("Ditt val är felaktigt."); }
                                    }
                                }

                            }


                        }
                        break;
                    case ProgramState.PROGRAM_MENUES.UPDATE_PIZZA:
                        break;
                    case ProgramState.PROGRAM_MENUES.SHOW_PIZZA:
                        {
                            bool wantToGoBack = false;

                            while (wantToGoBack == false)
                            {
                                Console.Clear();
                                Console.WriteLine("~~ SAM" +
                                    "TLIGA PIZZOR ~~");
                                Console.WriteLine();
                                foreach (Pizza pizza in Helpers.LoadPizzasAsList(rep))
                                {
                                    Console.WriteLine(pizza.Type + ", " + pizza.Price + " kr");
                                    for (int index = 0; index < pizza.PizzaIngredients.Count; index++)
                                        Console.WriteLine(pizza.PizzaIngredients[index].Type);
                                    Console.WriteLine();
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
                        }
                        break;

                    case ProgramState.PROGRAM_MENUES.DELETE_PIZZA:
                        {
                            Console.Clear();
                            Console.WriteLine("~~ TA BORT PIZZA ~~");
                            Console.WriteLine("Klicka på ESC för att gå tillbaka.");
                            Console.WriteLine();

                            foreach (var pizza in await rep.GetAllPizzas())
                            {
                                Console.WriteLine($"ID - {pizza.PizzaID}, {pizza.Type}\n");
                            }

                            Console.Write("Ange ID för den pizza som du vill ta bort: ");

                            string IDOfPizzaToRemove = await Menus.ReadLineWithOptionToGoBack();
                            bool userInput = int.TryParse(IDOfPizzaToRemove, out int ID);
                            bool doesPizzaExist = (await rep.CheckIfPizzaIDExists(ID));

                            if (IDOfPizzaToRemove == null)
                            {
                                ProgramState.CURRENT_MENU = ProgramState.PROGRAM_MENUES.PIZZAS;
                            }

                            else if (doesPizzaExist == false)
                            {
                                await Menus.MessageIfChoiceIsNotRight("Pizza med angivet ID finns inte.");
                            }

                            else if (userInput == true)
                            {
                                Console.Clear();
                                Console.WriteLine($"Är du säker på att du vill ta bort pizza {ID} (j/n)? Valet kan inte ändras."); //Visa namn istället för ID?

                                string choice = await Menus.ReadLineWithOptionToGoBack();

                                if (choice == null) { ProgramState.CURRENT_MENU = ProgramState.PROGRAM_MENUES.PIZZAS; }
                                else if (choice == "j")
                                {
                                    await rep.DeletePizza(ID);
                                    await Menus.ConfirmationScreen("Pizza borttagen");
                                    ProgramState.CURRENT_MENU = ProgramState.PROGRAM_MENUES.PIZZAS;

                                }
                                else if (choice == "n") { ProgramState.CURRENT_MENU = ProgramState.PROGRAM_MENUES.DELETE_PIZZA; }
                                else { await Menus.MessageIfChoiceIsNotRight("Vänligen svara med j eller n."); }
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
                                    await rep.DeleteCondiment(ID);
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
                                    await rep.DeleteExtra(ID);
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
