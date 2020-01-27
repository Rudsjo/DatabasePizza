using System;
using System.Threading;
using IDatabasePizza;
using MenuFunctions;
using MSSQLRepository;
using PostgreSQLRepository;
using PizzaClassLibrary;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace Administrator
{

    class Program
    {
        static async Task Main(string[] args)
        {

            // KONTROLL AV DATABAS
            {
                //bool databaseIsChosen = false;
                //IDatabase rep = null;

                //while (databaseIsChosen == false)
                //{
                //    Console.WriteLine("Ange vilken databas du vill använda: ");
                //    Console.WriteLine("1. MS SQL");
                //    Console.WriteLine("2. Postgre SQL");
                //    bool input = int.TryParse(Console.ReadLine(), out int choice);

                //    if (input == true)
                //    {
                //        if (choice == 1)
                //        {
                //            rep = new MSSQL();
                //            databaseIsChosen = true;
                //        }
                //        else if (choice == 2)
                //        {
                //            rep = new PostgreSQL();
                //            databaseIsChosen = true;
                //        }
                //        else
                //        {
                //            ProgramState.MessageIfChoiceIsNotRight("Ditt val är felaktigt.");

                //        }
                //    }
                //    else
                //    {
                //        ProgramState.MessageIfChoiceIsNotRight("Fel typ av inmatning");
                //    }
                //}
            }

            MSSQL rep = new MSSQL(); // Denna kan tas bort när vi skapat funktioner för Postgre SQL
            Menus menus = new Menus();

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
                                bool userInput = int.TryParse(ProgramState.ReadLineWithOptionToGoBack(), out userID);

                                if (userInput == true)
                                {
                                    Console.Write("Lösenord: ");
                                    password = ProgramState.ShowPasswordAsStarsWithOptionToGoBack();
                                    correctTypeOfInput = true;
                                }
                                else
                                {
                                    ProgramState.MessageIfChoiceIsNotRight("ID är skrivet i fel format.");
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
                                            ProgramState.MessageIfChoiceIsNotRight("Behörighet saknas.");
                                        }
                                    }
                                    else
                                    {
                                        ProgramState.MessageIfChoiceIsNotRight("AnvändarID eller lösenord är felaktigt. Försök igen.");
                                    }
                                }
                            }
                        }
                        break;
                    case ProgramState.PROGRAM_MENUES.MAIN_MENU:
                        {
                            // MENU CHOICES
                            Console.Clear();
                            ProgramState.PrintMenu(menus.menuChoices[0]);

                            bool userChoice = int.TryParse(Console.ReadLine(), out int choice);

                            // USER CHOICE
                            {
                                if (userChoice == true)
                                {
                                    #region Massa if skit
                                    if (choice == 1) { ProgramState.CURRENT_MENU = ProgramState.PROGRAM_MENUES.EMPLOYEES; }
                                    else if (choice == 2) { ProgramState.CURRENT_MENU = ProgramState.PROGRAM_MENUES.PIZZAS; }
                                    else if (choice == 3) { ProgramState.CURRENT_MENU = ProgramState.PROGRAM_MENUES.INGREDIENTS; }
                                    else if (choice == 4) { ProgramState.CURRENT_MENU = ProgramState.PROGRAM_MENUES.EXTRAS; }
                                    else if (choice == 5) { ProgramState.CURRENT_MENU = ProgramState.PROGRAM_MENUES.OLD_ORDERS; }
                                    else if (choice == 6) { ProgramState.CURRENT_MENU = ProgramState.PROGRAM_MENUES.LOGIN_SCREEN; }
                                    else ProgramState.MessageIfChoiceIsNotRight("Ditt val är felaktigt.", "Valet finns inte.");
                                    #endregion
                                }
                                else ProgramState.MessageIfChoiceIsNotRight("Ditt val är felaktigt.");
                                
                            }
                        }
                        break;

                    // Employees
                    case ProgramState.PROGRAM_MENUES.EMPLOYEES:
                        {
                            Console.Clear();
                            // MENU CHOICES
                            {
                                Console.WriteLine("~~ ANSTÄLLDA ~~");
                                ProgramState.PrintMenu(menus.menuChoices[1]);
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
                                    else
                                    {
                                        ProgramState.MessageIfChoiceIsNotRight("Ditt val är felaktigt.", "Valet finns inte.");
                                    }
                                }

                                else
                                {
                                    ProgramState.MessageIfChoiceIsNotRight("Ditt val är felaktigt.");
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
                                    string tempPassword = ProgramState.ReadLineWithOptionToGoBack();

                                    if (tempPassword == null)
                                    {
                                        Console.Clear();
                                        ProgramState.CURRENT_MENU = ProgramState.PROGRAM_MENUES.EMPLOYEES;
                                        break;
                                    }

                                    else if (tempPassword.Length < 1)
                                    {
                                        ProgramState.MessageIfChoiceIsNotRight("Ditt lösenord måste innehålla tecken.");
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
                                    string tempRole = ProgramState.ReadLineWithOptionToGoBack();

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
                                        ProgramState.MessageIfChoiceIsNotRight("Rollen finns inte.");
                                        Console.Clear();
                                    }

                                }

                                if (correctPass == true && correctRole == true)
                                {
                                    await rep.AddEmployee(password, role);
                                    ProgramState.ConfirmationScreen("Den anställde är tillagd.");
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
                            string IDToChange = ProgramState.ReadLineWithOptionToGoBack();

                            int.TryParse(IDToChange, out int ID);
                            //Fixat SP så man kan kolla om användaren man vill ändra finns i databasen
                            bool doesUserExist = (await rep.CheckIfUserIDExists(ID));

                            if(IDToChange == null)
                            {
                                ProgramState.CURRENT_MENU = ProgramState.PROGRAM_MENUES.EMPLOYEES;
                            }

                            else if (doesUserExist == false && IDToChange != null)
                            {
                                ProgramState.MessageIfChoiceIsNotRight("Den anställde med angivet ID finns inte.");
                            }

                            else
                            {
                                Employee emp = await rep.ShowSingleEmployee(ID);

                                Console.Clear();
                                Console.WriteLine("Klicka ESC för att gå tillbaka.");
                                Console.WriteLine("Ange vad du vill ändra hos användaren:");
                                Console.WriteLine("1. Lösenord");
                                Console.WriteLine("2. Roll");

                                string whatToChange = ProgramState.ReadLineWithOptionToGoBack();
                                bool userInput = int.TryParse(whatToChange, out int changeChoice);

                                if(whatToChange == null)
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
                                            string tempPass = ProgramState.ReadLineWithOptionToGoBack();
                                            if(tempPass == null)
                                            {
                                                Console.Clear();
                                                ProgramState.CURRENT_MENU = ProgramState.PROGRAM_MENUES.UPDATE_EMPLOYEE;
                                                break;
                                            }

                                            else if (tempPass.Length < 1)
                                            {
                                                ProgramState.MessageIfChoiceIsNotRight("Lösenordet måste innehålla tecken.");
                                            }

                                            else
                                            {
                                                emp.Password = tempPass;
                                                ProgramState.ConfirmationScreen("Lösenordet ändrat.");
                                                correctPass = true;
                                            }

                                        }

                                        if (correctPass == true)
                                        {
                                            await rep.UpdateEmployee(emp);
                                            ProgramState.ConfirmationScreen("Den anställde är uppdaterad.");
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

                                            string tempRole = ProgramState.ReadLineWithOptionToGoBack();

                                            if(tempRole == null)
                                            {
                                                Console.Clear();
                                                ProgramState.CURRENT_MENU = ProgramState.PROGRAM_MENUES.EMPLOYEES;
                                                break;
                                            }

                                            else if (tempRole.ToLower() == "admin" || tempRole.ToLower() == "bagare" || tempRole.ToLower() == "kassör")
                                            {
                                                emp.Role = tempRole;
                                                ProgramState.ConfirmationScreen("Rollen ändrad.");
                                                correctRole = true;
                                            }

                                            else
                                            {
                                                ProgramState.MessageIfChoiceIsNotRight("Rollen finns inte.");
                                            }
                                        }

                                        if(correctRole == true)
                                        {
                                            await rep.UpdateEmployee(emp);
                                            ProgramState.ConfirmationScreen("Den anställde är uppdaterad.");
                                        }
                                    }
                                }
                                else
                                {
                                    ProgramState.MessageIfChoiceIsNotRight("Valet finns inte.");
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
                                foreach (var employees in await rep.ShowEmployee())
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

                            string IDOfUserToRemove = ProgramState.ReadLineWithOptionToGoBack();
                            bool userInput = int.TryParse(IDOfUserToRemove, out int ID);
                            bool doesUserExist = (await rep.CheckIfUserIDExists(ID));

                            if (IDOfUserToRemove == null)
                            {
                                ProgramState.CURRENT_MENU = ProgramState.PROGRAM_MENUES.EMPLOYEES;
                            }

                            else if(doesUserExist == false)
                            {
                                ProgramState.MessageIfChoiceIsNotRight("Användare med angivet ID finns inte.");
                            }

                            else if (userInput == true)
                            {
                                Console.WriteLine($"Är du säker på att du vill ta bort anställd {ID} (j/n)? Valet kan inte ändras.");

                                string choice = ProgramState.ReadLineWithOptionToGoBack();

                                if(choice == null) { ProgramState.CURRENT_MENU = ProgramState.PROGRAM_MENUES.EMPLOYEES; }
                                else if (choice == "j")
                                {
                                    await rep.DeleteEmployee(ID);
                                    ProgramState.ConfirmationScreen("Anställd borttagen");
                                    ProgramState.CURRENT_MENU = ProgramState.PROGRAM_MENUES.EMPLOYEES;

                                }
                                else if (choice == "n"){ ProgramState.CURRENT_MENU = ProgramState.PROGRAM_MENUES.DELETE_EMPLOYEE; }
                                else { ProgramState.MessageIfChoiceIsNotRight("Vänligen svara med j eller n."); }
                            }

                        }
                        break;

                    // Pizzas
                    case ProgramState.PROGRAM_MENUES.PIZZAS:
                        {
                            Console.Clear();
                            // MENU CHOICES
                            {
                                Console.WriteLine("~~ PIZZOR ~~");
                                ProgramState.PrintMenu(menus.menuChoices[2]);
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
                                        ProgramState.MessageIfChoiceIsNotRight("Ditt val är felaktigt.", "Valet finns inte.");
                                    }
                                }

                                else
                                {
                                    ProgramState.MessageIfChoiceIsNotRight("Ditt val är felaktigt.");
                                }
                            }
                        }
                        break;
                    case ProgramState.PROGRAM_MENUES.ADD_PIZZA:
                        {
                            string pizzaName = null, pizzaBase = null, pizzaIngredients = null;
                            bool correctPizzaName = false, correctPizzaBase = false, correctPizzaPrice = false;
                            float pizzaPrice;

                            // kolla pizzans namn
                            {
                                while (correctPizzaName == false)
                                {
                                    Console.Clear();
                                    Console.WriteLine("~~ LÄGG TILL PIZZA ~~");
                                    Console.WriteLine("Klicka på ESC för att gå tillbaka.");

                                    Console.WriteLine();
                                    Console.Write("Pizzans namn: ");
                                    pizzaName = ProgramState.ReadLineWithOptionToGoBack();

                                    if(pizzaName == null)
                                    {
                                        Console.Clear();
                                        ProgramState.CURRENT_MENU = ProgramState.PROGRAM_MENUES.PIZZAS;
                                        break;
                                    }

                                    else if (pizzaName.Length > 1) { correctPizzaName = true; }
                                    else { ProgramState.MessageIfChoiceIsNotRight("Namnet måste innehålla minst ett tecken."); }
                                }
                            }
                            // kolla pizzans botten
                            {
                                while (correctPizzaBase == false && pizzaName != null)
                                {
                                    Console.Write("Pizzans botten (Italiensk eller amerikansk): ");
                                    pizzaBase = Console.ReadLine();
                                    if (pizzaBase.ToLower() == "italiensk" || pizzaBase.ToLower() == "amerikansk")
                                    {
                                        correctPizzaBase = true;
                                    }
                                }
                            }

                            // kolla pizzans pris
                            {
                                while (correctPizzaPrice == false)
                                {
                                    Console.Write("Pizzans pris: ");
                                    bool correctInput = float.TryParse(Console.ReadLine(), out float price);
                                    if (correctInput == true)
                                    {
                                        pizzaPrice = price;
                                        correctPizzaPrice = true;
                                    }
                                }
                            }

                            //kolla pizzans ingredienser
                            {
                                Console.WriteLine("Välj pizzans ingredienser (nummer, nummer, nummer etc.)");
                                int counter = 1;

                                foreach (var ingredient in await rep.ShowCondiments())
                                {
                                    Console.WriteLine($"{counter}. {ingredient.Type}");
                                    counter++;
                                }

                                string[] chosenPizzaIngredients = Console.ReadLine().Trim().Split(',');
                                int[] confirmedChosenPizzaIngredients = null;
                                counter = 0;

                                foreach (var ingredientID in chosenPizzaIngredients)
                                {
                                    bool checkCorrectValues = int.TryParse(ingredientID, out int id);

                                    if (checkCorrectValues == true)
                                    {
                                        confirmedChosenPizzaIngredients[counter] = id;
                                        counter++;
                                    }
                                    else
                                    {
                                        continue;
                                    }
                                }

                                // SKA DETTA ÖVERSÄTTAS TILL JSON OCH SKICKAS IN??

                            }


                        }
                        break;
                    case ProgramState.PROGRAM_MENUES.UPDATE_PIZZA:
                        break;
                    case ProgramState.PROGRAM_MENUES.SHOW_PIZZA:
                        {
                            IEnumerable<Pizza> plist = await rep.ShowPizza("ShowPizzas");
                            foreach(Pizza p in plist)
                            {
                                foreach (Condiment c in p.ObjectList) Console.WriteLine(p.Type + ": " + c.Type + " - " + c.Price);
                                Console.ReadKey();
                            }
                        }
                        break;
                    case ProgramState.PROGRAM_MENUES.DELETE_PIZZA:
                        break;

                    // Ingredients
                    case ProgramState.PROGRAM_MENUES.INGREDIENTS:
                        {
                            Console.Clear();
                            // MENU CHOICES
                            {
                                Console.WriteLine("~~ INGREDIENSER ~~");
                                ProgramState.PrintMenu(menus.menuChoices[3]);
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
                                        ProgramState.MessageIfChoiceIsNotRight("Ditt val är felaktigt.", "Valet finns inte.");
                                    }
                                }

                                else
                                {
                                    ProgramState.MessageIfChoiceIsNotRight("Ditt val är felaktigt.");
                                }
                            }
                        }
                        break;
                    case ProgramState.PROGRAM_MENUES.ADD_INGREDIENT:
                        break;
                    case ProgramState.PROGRAM_MENUES.UPDATE_INGREDIENT:
                        break;
                    case ProgramState.PROGRAM_MENUES.SHOW_INGREDIENT:
                        break;
                    case ProgramState.PROGRAM_MENUES.DELETE_INGREDIENT:
                        break;

                    // Extras
                    case ProgramState.PROGRAM_MENUES.EXTRAS:
                        {
                            Console.Clear();
                            // MENU CHOICES
                            {
                                Console.WriteLine("~~ TILLBEHÖR ~~");
                                ProgramState.PrintMenu(menus.menuChoices[4]);
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
                                        ProgramState.MessageIfChoiceIsNotRight("Ditt val är felaktigt.", "Valet finns inte.");
                                    }
                                }

                                else
                                {
                                    ProgramState.MessageIfChoiceIsNotRight("Ditt val är felaktigt.");
                                }
                            }
                        }
                        break;
                    case ProgramState.PROGRAM_MENUES.ADD_EXTRAS:
                        break;
                    case ProgramState.PROGRAM_MENUES.UPDATE_EXTRAS:
                        break;
                    case ProgramState.PROGRAM_MENUES.SHOW_EXTRAS:
                        break;
                    case ProgramState.PROGRAM_MENUES.DELETE_EXTRAS:
                        break;

                    // Old orders
                    case ProgramState.PROGRAM_MENUES.OLD_ORDERS:
                        {
                            Console.Clear();
                            // MENU CHOICES
                            {
                                Console.WriteLine("~~ GAMLA ORDRAR ~~");
                                ProgramState.PrintMenu(menus.menuChoices[5]);
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
                                        ProgramState.MessageIfChoiceIsNotRight("Ditt val är felaktigt.", "Valet finns inte.");
                                    }
                                }

                                else
                                {
                                    ProgramState.MessageIfChoiceIsNotRight("Ditt val är felaktigt.");
                                }
                            }
                        }
                        break;
                    case ProgramState.PROGRAM_MENUES.SHOW_OLD_ORDERS:
                        break;
                    case ProgramState.PROGRAM_MENUES.DELETE_OLD_ORDERS:
                        break;

                    default:
                        ProgramState.Running = false;
                        break;
                }
            }
        }
    }
}
