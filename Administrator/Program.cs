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
                    case ProgramState.PROGRAM_MENUES.LOGIN_SCREEN:
                        {

                            #region Variables
                            int userID;
                            string password = "";
                            bool correctTypeOfInput = false;
                            #endregion

                            Console.Clear();
                            // LOG IN TEXT
                            {
                                Console.Write("AnvändarID: ");
                                bool userInput = int.TryParse(Menus.ReadLineWithOptionToGoBack(), out userID);

                                if (userInput == true)
                                {
                                    Console.Write("Lösenord: ");
                                    password = Menus.ShowPasswordAsStarsWithOptionToGoBack();
                                    correctTypeOfInput = true;
                                }
                                else
                                {
                                    Menus.MessageIfChoiceIsNotRight("ID är skrivet i fel format.");
                                }
                            }

                            // CHECK LOG IN
                            {
                                if (correctTypeOfInput == true)
                                {
                                    bool correctLogInCredentials = (await rep.CheckUserIdAndPassword(userID, password, "CheckPassword")).Item1;
                                    string checkRole = (await rep.CheckUserIdAndPassword(userID, password, "CheckPassword")).Item2;

                                    if (correctLogInCredentials == true)
                                    {
                                        if (checkRole.ToLower() == "admin")
                                        {
                                            ProgramState.CURRENT_MENU = ProgramState.PROGRAM_MENUES.MAIN_MENU;
                                        }
                                        else
                                        {
                                            Menus.MessageIfChoiceIsNotRight("Behörighet saknas.");
                                        }
                                    }
                                    else
                                    {
                                        Menus.MessageIfChoiceIsNotRight("AnvändarID eller lösenord är felaktigt. Försök igen.");
                                    }
                                }
                            }
                        }
                        break;
                    case ProgramState.PROGRAM_MENUES.MAIN_MENU:
                        {
                            // MENU CHOICES
                            Console.Clear();
                            Menus.PrintMenu(Menus.menuChoices[0]);

                            bool userChoice = int.TryParse(Console.ReadLine(), out int choice);

                            // USER CHOICE
                            {
                                if (userChoice == true)
                                {                                 
                                    if (choice == 1) { ProgramState.CURRENT_MENU = ProgramState.PROGRAM_MENUES.EMPLOYEES; }
                                    else if (choice == 2) { ProgramState.CURRENT_MENU = ProgramState.PROGRAM_MENUES.PIZZAS; }
                                    else if (choice == 3) { ProgramState.CURRENT_MENU = ProgramState.PROGRAM_MENUES.INGREDIENTS; }
                                    else if (choice == 4) { ProgramState.CURRENT_MENU = ProgramState.PROGRAM_MENUES.EXTRAS; }
                                    else if (choice == 5) { ProgramState.CURRENT_MENU = ProgramState.PROGRAM_MENUES.OLD_ORDERS; }
                                    else if (choice == 6) { ProgramState.CURRENT_MENU = ProgramState.PROGRAM_MENUES.LOGIN_SCREEN; }
                                    else Menus.MessageIfChoiceIsNotRight("Ditt val är felaktigt.", "Valet finns inte.");                             
                                }
                                else Menus.MessageIfChoiceIsNotRight("Ditt val är felaktigt.");
                                
                            }
                        }
                        break;

                    #region // Employees
                    case ProgramState.PROGRAM_MENUES.EMPLOYEES:
                        {
                            Console.Clear();
                            // MENU CHOICES
                            {
                                Console.WriteLine("~~ ANSTÄLLDA ~~");
                                Menus.PrintMenu(Menus.menuChoices[1]);
                            }

                            bool userChoice = int.TryParse(Console.ReadLine(), out int choice);

                            //USER CHOICE
                            {
                                if (userChoice == true)
                                {
                                    if (choice == 1) { ProgramState.CURRENT_MENU = ProgramState.PROGRAM_MENUES.ADD_EMPLOYEE; }
                                    else if (choice == 2) { ProgramState.CURRENT_MENU = ProgramState.PROGRAM_MENUES.UPDATE_EMPLOYEE; }
                                    else if (choice == 3) { ProgramState.CURRENT_MENU = ProgramState.PROGRAM_MENUES.SHOW_EMPLOYEE; }
                                    else if (choice == 4) { ProgramState.CURRENT_MENU = ProgramState.PROGRAM_MENUES.DELETE_EMPLOYEE; }
                                    else if (choice == 5) { ProgramState.CURRENT_MENU = ProgramState.PROGRAM_MENUES.MAIN_MENU; }
                                    else Menus.MessageIfChoiceIsNotRight("Ditt val är felaktigt.", "Valet finns inte.");
                              
                                }

                                else
                                {
                                    Menus.MessageIfChoiceIsNotRight("Ditt val är felaktigt.");
                                }
                            }
                        }
                        break;

                    case ProgramState.PROGRAM_MENUES.ADD_EMPLOYEE:
                        {
                            string password = null;
                            string role = null;
                            bool correctPass = false, correctRole = false;

                            Console.Clear();
                            // MENU CHOICES
                            {
                                do
                                {
                                    Console.WriteLine("~~ LÄGG TILL ANSTÄLLD ~~");
                                    Console.WriteLine("Klicka på ESC för att gå tillbaka.");

                                    Console.Write("Ange den anställdes lösenord: ");
                                    string tempPassword = Menus.ReadLineWithOptionToGoBack();

                                    if (tempPassword == null)
                                    {
                                        Console.Clear();
                                        ProgramState.CURRENT_MENU = ProgramState.PROGRAM_MENUES.EMPLOYEES;
                                        break;
                                    }

                                    else if (tempPassword.Length < 1)
                                    {
                                        Menus.MessageIfChoiceIsNotRight("Ditt lösenord måste innehålla tecken.");
                                    }

                                    else
                                    {
                                        password = tempPassword;
                                        correctPass = true;
                                    }
                                } while (correctPass == false);


                                while (correctRole == false && password != null)
                                {
                                    Console.Write("Ange den anställdes roll: ");
                                    string tempRole = Menus.ReadLineWithOptionToGoBack();

                                    if(tempRole == null)
                                    {
                                        Console.Clear();
                                        ProgramState.CURRENT_MENU = ProgramState.PROGRAM_MENUES.EMPLOYEES;
                                        break;
                                    }

                                    else if (tempRole.ToLower() == "admin" || tempRole.ToLower() == "bagare" || tempRole.ToLower() == "kassör")
                                    {
                                        role = tempRole;
                                        ProgramState.CURRENT_MENU = ProgramState.PROGRAM_MENUES.EMPLOYEES;
                                        correctRole = true;
                                    }

                                    else
                                    {
                                        Menus.MessageIfChoiceIsNotRight("Rollen finns inte.");
                                        Console.Clear();
                                    }

                                }

                                if (correctPass == true && correctRole == true)
                                {
                                    await rep.AddEmployee(password, role);
                                    Menus.ConfirmationScreen("Den anställde är tillagd.");
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
                            string IDToChange = Menus.ReadLineWithOptionToGoBack();

                            int.TryParse(IDToChange, out int ID);
                            //Fixat SP så man kan kolla om användaren man vill ändra finns i databasen
                            bool doesUserExist = (await rep.CheckIfUserIDExists(ID));

                            if(IDToChange == null)
                            {
                                ProgramState.CURRENT_MENU = ProgramState.PROGRAM_MENUES.EMPLOYEES;
                            }

                            else if (doesUserExist == false && IDToChange != null)
                            {
                                Menus.MessageIfChoiceIsNotRight("Den anställde med angivet ID finns inte.");
                            }

                            else
                            {
                                Employee emp = await rep.GetSingleEmployee(ID);

                                Console.Clear();
                                Console.WriteLine("Klicka ESC för att gå tillbaka.");
                                Console.WriteLine("Ange vad du vill ändra hos användaren:");
                                Console.WriteLine("1. Lösenord");
                                Console.WriteLine("2. Roll");

                                string whatToChange = Menus.ReadLineWithOptionToGoBack();
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
                                            string tempPass = Menus.ReadLineWithOptionToGoBack();
                                            if(tempPass == null)
                                            {
                                                Console.Clear();
                                                ProgramState.CURRENT_MENU = ProgramState.PROGRAM_MENUES.UPDATE_EMPLOYEE;
                                                break;
                                            }

                                            else if (tempPass.Length < 1)
                                            {
                                                Menus.MessageIfChoiceIsNotRight("Lösenordet måste innehålla tecken.");
                                            }

                                            else
                                            {
                                                emp.Password = tempPass;
                                                Menus.ConfirmationScreen("Lösenordet ändrat.");
                                                correctPass = true;
                                            }

                                        }

                                        if (correctPass == true)
                                        {
                                            await rep.UpdateEmployee(emp);
                                            Menus.ConfirmationScreen("Den anställde är uppdaterad.");
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

                                            string tempRole = Menus.ReadLineWithOptionToGoBack();

                                            if(tempRole == null)
                                            {
                                                Console.Clear();
                                                ProgramState.CURRENT_MENU = ProgramState.PROGRAM_MENUES.EMPLOYEES;
                                                break;
                                            }

                                            else if (tempRole.ToLower() == "admin" || tempRole.ToLower() == "bagare" || tempRole.ToLower() == "kassör")
                                            {
                                                emp.Role = tempRole;
                                                Menus.ConfirmationScreen("Rollen ändrad.");
                                                correctRole = true;
                                            }

                                            else
                                            {
                                                Menus.MessageIfChoiceIsNotRight("Rollen finns inte.");
                                            }
                                        }

                                        if(correctRole == true)
                                        {
                                            await rep.UpdateEmployee(emp);
                                            Menus.ConfirmationScreen("Den anställde är uppdaterad.");
                                        }
                                    }
                                }
                                else
                                {
                                    Menus.MessageIfChoiceIsNotRight("Valet finns inte.");
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

                            string IDOfUserToRemove = Menus.ReadLineWithOptionToGoBack();
                            bool userInput = int.TryParse(IDOfUserToRemove, out int ID);
                            bool doesUserExist = (await rep.CheckIfUserIDExists(ID));

                            if (IDOfUserToRemove == null)
                            {
                                ProgramState.CURRENT_MENU = ProgramState.PROGRAM_MENUES.EMPLOYEES;
                            }

                            else if(doesUserExist == false)
                            {
                                Menus.MessageIfChoiceIsNotRight("Användare med angivet ID finns inte.");
                            }

                            else if (userInput == true)
                            {
                                Console.WriteLine($"Är du säker på att du vill ta bort anställd {ID} (j/n)? Valet kan inte ändras.");

                                string choice = Menus.ReadLineWithOptionToGoBack();

                                if(choice == null) { ProgramState.CURRENT_MENU = ProgramState.PROGRAM_MENUES.EMPLOYEES; }
                                else if (choice == "j")
                                {
                                    await rep.DeleteEmployee(ID);
                                    Menus.ConfirmationScreen("Anställd borttagen");
                                    ProgramState.CURRENT_MENU = ProgramState.PROGRAM_MENUES.EMPLOYEES;

                                }
                                else if (choice == "n"){ ProgramState.CURRENT_MENU = ProgramState.PROGRAM_MENUES.DELETE_EMPLOYEE; }
                                else { Menus.MessageIfChoiceIsNotRight("Vänligen svara med j eller n."); }
                            }

                        }
                        break;
                    #endregion

                    #region// Pizzas
                    case ProgramState.PROGRAM_MENUES.PIZZAS:
                        {
                            Console.Clear();
                            // MENU CHOICES
                            {
                                Console.WriteLine("~~ PIZZOR ~~");
                                Menus.PrintMenu(Menus.menuChoices[2]);
                            }

                            bool userChoice = int.TryParse(Console.ReadLine(), out int choice);

                            //USER CHOICE
                            {
                                if (userChoice == true)
                                {
                                    if (choice == 1) { ProgramState.CURRENT_MENU = ProgramState.PROGRAM_MENUES.ADD_PIZZA; }
                                    else if (choice == 2) { ProgramState.CURRENT_MENU = ProgramState.PROGRAM_MENUES.UPDATE_PIZZA; }
                                    else if (choice == 3) { ProgramState.CURRENT_MENU = ProgramState.PROGRAM_MENUES.SHOW_PIZZA; }
                                    else if (choice == 4) { ProgramState.CURRENT_MENU = ProgramState.PROGRAM_MENUES.DELETE_PIZZA; }
                                    else if (choice == 5) { ProgramState.CURRENT_MENU = ProgramState.PROGRAM_MENUES.MAIN_MENU; }
                                    else
                                    {
                                        Menus.MessageIfChoiceIsNotRight("Ditt val är felaktigt.", "Valet finns inte.");
                                    }
                                }

                                else
                                {
                                    Menus.MessageIfChoiceIsNotRight("Ditt val är felaktigt.");
                                }
                            }
                        }
                        break;
                    case ProgramState.PROGRAM_MENUES.ADD_PIZZA:
                        {
                            bool correctPizzaName = false, correctPizzaBase = false, correctPizzaPrice = false;

                            Pizza newPizza = new Pizza();
                            newPizza.Type = null; newPizza.Base = null; newPizza.Price = 0;

                            // kolla pizzans namn
                            {
                                while (correctPizzaName == false)
                                {
                                    Console.Clear();
                                    Console.WriteLine("~~ LÄGG TILL PIZZA ~~");
                                    Console.WriteLine("Klicka på ESC för att gå tillbaka.");

                                    Console.WriteLine();
                                    Console.Write("Pizzans namn: ");
                                    newPizza.Type = Menus.ReadLineWithOptionToGoBack();

                                    if(newPizza.Type == null)
                                    {
                                        Console.Clear();
                                        ProgramState.CURRENT_MENU = ProgramState.PROGRAM_MENUES.PIZZAS;
                                        break;
                                    }

                                    else if (newPizza.Type.Length > 1) { correctPizzaName = true; }
                                    else { Menus.MessageIfChoiceIsNotRight("Namnet måste innehålla minst ett tecken."); }
                                }
                            }
                            // kolla pizzans botten
                            {
                                while (correctPizzaBase == false && newPizza.Type != null)
                                {
                                    Console.Write("Pizzans botten (Italiensk eller amerikansk): ");
                                    newPizza.Base = Menus.ReadLineWithOptionToGoBack();

                                    if(newPizza.Base == null)
                                    {
                                        Console.Clear();
                                        ProgramState.CURRENT_MENU = ProgramState.PROGRAM_MENUES.PIZZAS;
                                        break;
                                    }

                                    else if (newPizza.Base.ToLower() == "italiensk" || newPizza.Base.ToLower() == "amerikansk")
                                    {
                                        correctPizzaBase = true;
                                    }

                                    else { Menus.MessageIfChoiceIsNotRight("Pizzabasen finns inte."); }
                                }
                            }

                            // kolla pizzans pris
                            {
                                while (correctPizzaPrice == false && newPizza.Type != null && newPizza.Base != null)
                                {
                                    Console.Write("Pizzans pris: ");
                                    string inputPrice = Menus.ReadLineWithOptionToGoBack();
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

                                    else { Menus.MessageIfChoiceIsNotRight("Priset är angivet felaktigt."); }
                                }
                            }

                            //kolla pizzans ingredienser
                            {
                                if (newPizza.Type != null && newPizza.Base != null && newPizza.Price != 0)
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

                                        string chosenCondiment = Menus.ReadLineWithOptionToGoBack();
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

                                            Menus.ConfirmationScreen("Ingrediens tillagd.");
                                            Console.Clear();

                                            Console.WriteLine("Ange ytterligare en ingrediens, ange 0 för att bekräfta pizzan.");
                                            Console.WriteLine("Klicka på ESC för att avbryta");
                                            Console.WriteLine();
                                        }

                                        else if (confirmedChosenCondiment == 0 && condimentsOfNewPizza.Count > 0)
                                        {
                                            newPizza.PizzaIngredients = condimentsOfNewPizza;
                                            await rep.AddPizza(newPizza.Type, newPizza.Price, newPizza.Base, newPizza.PizzaIngredients);
                                            Menus.ConfirmationScreen("Pizzan tillagd");
                                            ProgramState.CURRENT_MENU = ProgramState.PROGRAM_MENUES.PIZZAS;
                                            break;
                                        }

                                        else { Menus.MessageIfChoiceIsNotRight("Ditt val är felaktigt."); }
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
                                else { Menus.MessageIfChoiceIsNotRight("Vänligen klicka på ESC för att återgå."); }
                            }
                        }
                        break;
                    ///
                    ///Denna är tillagd utan att pusha till GIT
                    ///Lagt till så att man får upp en lista på allt som är möjligt att radera, visas med ID och typ
                    ///
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

                            string IDOfPizzaToRemove = Menus.ReadLineWithOptionToGoBack();
                            bool userInput = int.TryParse(IDOfPizzaToRemove, out int ID);
                            bool doesPizzaExist = (await rep.CheckIfPizzaIDExists(ID));

                            if (IDOfPizzaToRemove == null)
                            {
                                ProgramState.CURRENT_MENU = ProgramState.PROGRAM_MENUES.PIZZAS;
                            }

                            else if (doesPizzaExist == false)
                            {
                                Menus.MessageIfChoiceIsNotRight("Pizza med angivet ID finns inte.");
                            }

                            else if (userInput == true)
                            {
                                Console.Clear();
                                Console.WriteLine($"Är du säker på att du vill ta bort pizza {ID} (j/n)? Valet kan inte ändras."); //Visa namn istället för ID?

                                string choice = Menus.ReadLineWithOptionToGoBack();

                                if (choice == null) { ProgramState.CURRENT_MENU = ProgramState.PROGRAM_MENUES.PIZZAS; }
                                else if (choice == "j")
                                {
                                    await rep.DeletePizza(ID);
                                    Menus.ConfirmationScreen("Pizza borttagen");
                                    ProgramState.CURRENT_MENU = ProgramState.PROGRAM_MENUES.PIZZAS;

                                }
                                else if (choice == "n") { ProgramState.CURRENT_MENU = ProgramState.PROGRAM_MENUES.DELETE_PIZZA; }
                                else { Menus.MessageIfChoiceIsNotRight("Vänligen svara med j eller n."); }
                            }
                        }
                        break;
                    #endregion

                    #region// Ingredients
                    case ProgramState.PROGRAM_MENUES.INGREDIENTS:
                        {
                            Console.Clear();
                            // MENU CHOICES
                            {
                                Console.WriteLine("~~ INGREDIENSER ~~");
                                Menus.PrintMenu(Menus.menuChoices[3]);
                            }

                            bool userChoice = int.TryParse(Console.ReadLine(), out int choice);

                            //USER CHOICE
                            {
                                if (userChoice == true)
                                {
                                    if (choice == 1) { ProgramState.CURRENT_MENU = ProgramState.PROGRAM_MENUES.ADD_INGREDIENT; }
                                    else if (choice == 2) { ProgramState.CURRENT_MENU = ProgramState.PROGRAM_MENUES.UPDATE_INGREDIENT; }
                                    else if (choice == 3) { ProgramState.CURRENT_MENU = ProgramState.PROGRAM_MENUES.SHOW_INGREDIENT; }
                                    else if (choice == 4) { ProgramState.CURRENT_MENU = ProgramState.PROGRAM_MENUES.DELETE_INGREDIENT; }
                                    else if (choice == 5) { ProgramState.CURRENT_MENU = ProgramState.PROGRAM_MENUES.MAIN_MENU; }
                                    else
                                    {
                                        Menus.MessageIfChoiceIsNotRight("Ditt val är felaktigt.", "Valet finns inte.");
                                    }
                                }

                                else
                                {
                                    Menus.MessageIfChoiceIsNotRight("Ditt val är felaktigt.");
                                }
                            }
                        }
                        break;
                    ///
                    ///Regex är använt för att kolla så att man inte försöker lägga in siffror i namnet. kollar a - ö och A-Ö i unicode.
                    ///Denna är tillagd utan att pusha till GIT
                    ///
                    case ProgramState.PROGRAM_MENUES.ADD_INGREDIENT:
                        {
                            Condiment condiment = new Condiment();
                            condiment.Type = null; condiment.Price = 0;                          

                            Console.Clear();
                            Console.WriteLine("~~ LÄGG TILL INGREDIENS ~~");
                            Console.WriteLine("Klicka på ESC för att gå tillbaka.");

                            Console.WriteLine();
                            Console.Write("Vilken ingrediens vill du lägga till: ");
                            string condimentName = Menus.ReadLineWithOptionToGoBack();

                            if(condimentName == null || condimentName.Length < 1 || !Regex.IsMatch(condimentName, @"^[a-öA-Ö]+$"))
                            {
                                Thread.Sleep(500);
                                ProgramState.CURRENT_MENU = ProgramState.PROGRAM_MENUES.INGREDIENTS;
                                break;
                            }
                            else { condiment.Type = condimentName; }

                            Console.Write("Ange pris för ingrediensen: ");
                            string inputPrice = Menus.ReadLineWithOptionToGoBack();
                            bool correctInput = float.TryParse(inputPrice, out float price);

                            if (inputPrice == null || correctInput == false || price == 0)
                            {
                                Thread.Sleep(500);
                                ProgramState.CURRENT_MENU = ProgramState.PROGRAM_MENUES.INGREDIENTS;
                                break;
                            }
                            else { condiment.Price = price; }

                            Console.Clear();
                            Console.WriteLine("~~ LÄGG TILL INGREDIENS ~~\n");
                            Console.Write($"Vill du lägga till artikeln i ingredienser?\n\n{condiment.Type} {condiment.Price}kr\n\nj/n: ");
                            string confirm = Console.ReadLine();

                            if (confirm.ToLower() == "j") {await rep.AddCondiment(condimentName, price); }
                            else { ProgramState.CURRENT_MENU = ProgramState.PROGRAM_MENUES.INGREDIENTS; break; }
                        }
                        break;
                    ///
                    ///Denna är tillagd utan att pusha till GIT
                    ///Testad, funkar.
                    ///
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
                            string IDToChange = Menus.ReadLineWithOptionToGoBack();

                            int.TryParse(IDToChange, out int ID);
                            //Fixat SP så man kan kolla om användaren man vill ändra finns i databasen
                            bool doesIngredientExist = (await rep.CheckIfCondimentIDExists(ID));

                            if (IDToChange == null)
                            {
                                ProgramState.CURRENT_MENU = ProgramState.PROGRAM_MENUES.INGREDIENTS;
                            }

                            else if (doesIngredientExist == false && IDToChange != null)
                            {
                                Menus.MessageIfChoiceIsNotRight("Ingrediensen med angivet ID finns inte.");
                            }

                            else
                            {
                                Condiment cond = await rep.GetSingleCondiment(ID);

                                Console.Clear();
                                Console.WriteLine("Klicka ESC för att gå tillbaka.\n");
                                Console.Write("Ange ingrediensens nya pris: ");

                                string newPrice = Menus.ReadLineWithOptionToGoBack();
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
                                    Menus.ConfirmationScreen("Priset ändrat.");
                                    
                                }
                            }
                        }
                        break;        
                    /// 
                    ///Denna är tillagd utan att pusha till GIT
                    ///
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
                            else { Menus.MessageIfChoiceIsNotRight("Vänligen klicka på ESC för att återgå."); }
                        }
                        break;
                    ///
                    ///Denna är tillagd utan att pusha till GIT
                    ///Lagt till så att man får upp en lista på allt som är möjligt att radera, visas med ID och typ
                    ///
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

                            string IDOfIngredientToRemove = Menus.ReadLineWithOptionToGoBack();
                            bool userInput = int.TryParse(IDOfIngredientToRemove, out int ID);
                            bool doesIngredientExist = (await rep.CheckIfCondimentIDExists(ID));

                            if (IDOfIngredientToRemove == null)
                            {
                                ProgramState.CURRENT_MENU = ProgramState.PROGRAM_MENUES.INGREDIENTS;
                            }

                            else if (doesIngredientExist == false)
                            {
                                Menus.MessageIfChoiceIsNotRight("Ingrediens med angivet ID finns inte.");
                            }

                            else if (userInput == true)
                            {
                                Console.Clear();
                                Console.WriteLine($"Är du säker på att du vill ta bort ingrediens {ID} (j/n)? Valet kan inte ändras.");

                                string choice = Menus.ReadLineWithOptionToGoBack();

                                if (choice == null) { ProgramState.CURRENT_MENU = ProgramState.PROGRAM_MENUES.INGREDIENTS; }
                                else if (choice == "j")
                                {
                                    await rep.DeleteCondiment(ID);
                                    Menus.ConfirmationScreen("Ingrediens borttagen");
                                    ProgramState.CURRENT_MENU = ProgramState.PROGRAM_MENUES.INGREDIENTS;

                                }
                                else if (choice == "n") { ProgramState.CURRENT_MENU = ProgramState.PROGRAM_MENUES.DELETE_INGREDIENT; }
                                else { Menus.MessageIfChoiceIsNotRight("Vänligen svara med j eller n."); }
                            }
                        }
                        break;
                    #endregion

                    #region// Extras
                    case ProgramState.PROGRAM_MENUES.EXTRAS:
                        {
                            Console.Clear();
                            // MENU CHOICES
                            {
                                Console.WriteLine("~~ TILLBEHÖR ~~");
                                Menus.PrintMenu(Menus.menuChoices[4]);
                            }

                            bool userChoice = int.TryParse(Console.ReadLine(), out int choice);

                            //USER CHOICE
                            {
                                if (userChoice == true)
                                {
                                    if (choice == 1) { ProgramState.CURRENT_MENU = ProgramState.PROGRAM_MENUES.ADD_EXTRAS; }
                                    else if (choice == 2) { ProgramState.CURRENT_MENU = ProgramState.PROGRAM_MENUES.UPDATE_EXTRAS; }
                                    else if (choice == 3) { ProgramState.CURRENT_MENU = ProgramState.PROGRAM_MENUES.SHOW_EXTRAS; }
                                    else if (choice == 4) { ProgramState.CURRENT_MENU = ProgramState.PROGRAM_MENUES.DELETE_EXTRAS; }
                                    else if (choice == 5) { ProgramState.CURRENT_MENU = ProgramState.PROGRAM_MENUES.MAIN_MENU; }
                                    else
                                    {
                                        Menus.MessageIfChoiceIsNotRight("Ditt val är felaktigt.", "Valet finns inte.");
                                    }
                                }

                                else
                                {
                                    Menus.MessageIfChoiceIsNotRight("Ditt val är felaktigt.");
                                }
                            }
                        }
                        break;
                    ///
                    ///Regex är använt för att kolla så att man inte försöker lägga in siffror i namnet,
                    ///kollar a - ö och A-Ö i unicode, tillåter blanksteg i typnamn
                    ///Denna är tillagd utan att pusha till GIT
                    ///
                    case ProgramState.PROGRAM_MENUES.ADD_EXTRAS:
                        {
                            Extra extra = new Extra();
                            extra.Type = null; extra.Price = 0;

                            Console.Clear();
                            Console.WriteLine("~~ LÄGG TILL TILLBEHÖR ~~");
                            Console.WriteLine("Klicka på ESC för att gå tillbaka.");

                            Console.WriteLine();
                            Console.Write("Vilket tillbehör vill du lägga till: ");
                            string extraName = Menus.ReadLineWithOptionToGoBack();
                            
                            if (extraName == null || extraName.Length < 1 || !Regex.IsMatch(extraName, @"^[a-öA-Ö\s]+$")) // \s är för att inkludera whitespace
                            {
                                Thread.Sleep(500);
                                ProgramState.CURRENT_MENU = ProgramState.PROGRAM_MENUES.EXTRAS;
                                break;
                            }
                            else { extra.Type = extraName; }

                            Console.Write("Ange pris för tillbehöret: ");
                            string inputPrice = Menus.ReadLineWithOptionToGoBack();
                            bool correctInput = float.TryParse(inputPrice, out float price);

                            if (inputPrice == null || correctInput == false || price == 0)
                            {
                                Thread.Sleep(500);
                                ProgramState.CURRENT_MENU = ProgramState.PROGRAM_MENUES.EXTRAS;
                                break;
                            }
                            else { extra.Price = price; }

                            Console.Clear();
                            Console.WriteLine("~~ LÄGG TILL TILLBEHÖR ~~\n");
                            Console.Write($"Vill du lägga till artikeln i tillbehör?\n\n{extra.Type} {extra.Price}kr\n\nj/n: ");
                            string confirm = Console.ReadLine();

                            if (confirm.ToLower() == "j") { await rep.AddExtra(extraName, price); } //Break här?
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
                            string IDToChange = Menus.ReadLineWithOptionToGoBack();

                            int.TryParse(IDToChange, out int ID);
                            //Fixat SP så man kan kolla om användaren man vill ändra finns i databasen
                            bool doesExtraExist = (await rep.CheckIfProductIDExists(ID));

                            if (IDToChange == null)
                            {
                                ProgramState.CURRENT_MENU = ProgramState.PROGRAM_MENUES.EXTRAS;
                            }

                            else if (doesExtraExist == false && IDToChange != null)
                            {
                                Menus.MessageIfChoiceIsNotRight("Tillbehöret med angivet ID finns inte.");
                            }

                            else
                            {
                                Extra extra = await rep.GetSingleExtra(ID);

                                Console.Clear();
                                Console.WriteLine("Klicka ESC för att gå tillbaka.\n");
                                Console.Write("Ange tillbehörets nya pris: ");

                                string newPrice = Menus.ReadLineWithOptionToGoBack();
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
                                    Menus.ConfirmationScreen("Priset ändrat.");

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
                            else { Menus.MessageIfChoiceIsNotRight("Vänligen klicka på ESC för att återgå."); }
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

                            string IDOfExtraToRemove = Menus.ReadLineWithOptionToGoBack();
                            bool userInput = int.TryParse(IDOfExtraToRemove, out int ID);
                            bool doesExtraExist = (await rep.CheckIfProductIDExists(ID));

                            if (IDOfExtraToRemove == null)
                            {
                                ProgramState.CURRENT_MENU = ProgramState.PROGRAM_MENUES.EXTRAS;
                            }

                            else if (doesExtraExist == false)
                            {
                                Menus.MessageIfChoiceIsNotRight("Tillbehör med angivet ID finns inte.");
                            }

                            else if (userInput == true)
                            {
                                Console.Clear();
                                Console.WriteLine($"Är du säker på att du vill ta bort tillbehör {ID} (j/n)? Valet kan inte ändras.");

                                string choice = Menus.ReadLineWithOptionToGoBack();

                                if (choice == null) { ProgramState.CURRENT_MENU = ProgramState.PROGRAM_MENUES.EXTRAS; }
                                else if (choice == "j")
                                {
                                    await rep.DeleteExtra(ID);
                                    Menus.ConfirmationScreen("Tillbehör borttaget");
                                    ProgramState.CURRENT_MENU = ProgramState.PROGRAM_MENUES.EXTRAS;

                                }
                                else if (choice == "n") { ProgramState.CURRENT_MENU = ProgramState.PROGRAM_MENUES.DELETE_EXTRAS; }
                                else { Menus.MessageIfChoiceIsNotRight("Vänligen svara med j eller n."); }
                            }
                        }
                        break;
                    #endregion

                    #region // Old orders
                    case ProgramState.PROGRAM_MENUES.OLD_ORDERS:
                        {
                            Console.Clear();
                            // MENU CHOICES
                            {
                                Console.WriteLine("~~ GAMLA ORDRAR ~~");
                                Menus.PrintMenu(Menus.menuChoices[5]);
                            }

                            bool userChoice = int.TryParse(Console.ReadLine(), out int choice);

                            //USER CHOICE
                            {
                                if (userChoice == true)
                                {
                                    if (choice == 1) { ProgramState.CURRENT_MENU = ProgramState.PROGRAM_MENUES.SHOW_OLD_ORDERS; }
                                    else if (choice == 2) { ProgramState.CURRENT_MENU = ProgramState.PROGRAM_MENUES.DELETE_OLD_ORDERS; }
                                    else if (choice == 3) { ProgramState.CURRENT_MENU = ProgramState.PROGRAM_MENUES.MAIN_MENU; }
                                    else
                                    {
                                        Menus.MessageIfChoiceIsNotRight("Ditt val är felaktigt.", "Valet finns inte.");
                                    }
                                }

                                else
                                {
                                    Menus.MessageIfChoiceIsNotRight("Ditt val är felaktigt.");
                                }
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
