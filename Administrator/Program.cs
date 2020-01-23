using System;
using System.Threading;
using IDatabasePizza;
using MenuFunctions;
using MSSQLRepository;
using PostgreSQLRepository;
using System.Threading.Tasks;

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


            // Denna kan tas bort när vi skapat funktioner för Postgre SQL
            MSSQL rep = new MSSQL();
            Menus menus = new Menus();

            while (ProgramState.Running)
            {
                switch (ProgramState.CURRENT_MENU)
                {
                    case ProgramState.PROGRAM_MENUES.LOGIN_SCREEN:
                        {
                            int userID;
                            string password = "";
                            bool correctTypeOfInput = false;

                            Console.Clear();
                            // LOG IN TEXT
                            {
                                Console.Write("AnvändarID: ");
                                bool userInput = int.TryParse(Console.ReadLine(), out userID);

                                if (userInput == true)
                                {
                                    Console.Write("Lösenord: ");
                                    password = Console.ReadLine();
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

                            Console.Clear();
                            // MENU CHOICES
                            {
                                Console.Clear();
                                ProgramState.PrintMenu(menus.menuChoices[0]);
                            }

                            bool userChoice = int.TryParse(Console.ReadLine(), out int choice);

                            // USER CHOICE
                            {
                                if (userChoice == true)
                                {
                                    if (choice == 1) { ProgramState.SetFormerPosition(); ProgramState.CURRENT_MENU = ProgramState.PROGRAM_MENUES.EMPLOYEES; }
                                    else if (choice == 2) { ProgramState.SetFormerPosition(); ProgramState.CURRENT_MENU = ProgramState.PROGRAM_MENUES.PIZZAS; }
                                    else if (choice == 3) { ProgramState.SetFormerPosition(); ProgramState.CURRENT_MENU = ProgramState.PROGRAM_MENUES.INGREDIENTS; }
                                    else if (choice == 4) { ProgramState.SetFormerPosition(); ProgramState.CURRENT_MENU = ProgramState.PROGRAM_MENUES.EXTRAS; }
                                    else if (choice == 5) { ProgramState.SetFormerPosition(); ProgramState.CURRENT_MENU = ProgramState.PROGRAM_MENUES.OLD_ORDERS; }
                                    else if (choice == 6) { ProgramState.SetFormerPosition(); ProgramState.CURRENT_MENU = ProgramState.PROGRAM_MENUES.LOGIN_SCREEN; }
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
                                    if (choice == 1) { ProgramState.SetFormerPosition(); ProgramState.CURRENT_MENU = ProgramState.PROGRAM_MENUES.ADD_EMPLOYEE; }
                                    else if (choice == 2) { ProgramState.SetFormerPosition(); ProgramState.CURRENT_MENU = ProgramState.PROGRAM_MENUES.UPDATE_EMPLOYEE; }
                                    else if (choice == 3) { ProgramState.SetFormerPosition(); ProgramState.CURRENT_MENU = ProgramState.PROGRAM_MENUES.SHOW_EMPLOYEE; }
                                    else if (choice == 4) { ProgramState.SetFormerPosition(); ProgramState.CURRENT_MENU = ProgramState.PROGRAM_MENUES.DELETE_EMPLOYEE; }
                                    else if (choice == 5) { ProgramState.SetFormerPosition(); ProgramState.CURRENT_MENU = ProgramState.PROGRAM_MENUES.MAIN_MENU; }
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
                            string password = "";
                            string role = "";
                            bool correctPass = false, correctRole = false, wantToGoBack = false;
                            char charChoice;

                            Console.Clear();
                            // MENU CHOICES
                            {
                                do
                                {
                                    Console.WriteLine("~~ LÄGG TILL ANSTÄLLD ~~");
                                    Console.WriteLine("Klicka på backspace för att gå tillbaka.");


                                    /**
                                     * LÄGGA TILL GÅ TILLBAKA FUNKTION MEN I ANNAN THREAD SÅ ATT MAN KAN SKRIVA OCH GÅ TILLBAKA SAMTIDIGT?
                                     **/


                                    Console.Write("Ange den anställdes lösenord: ");
                                    string tempPassword = Console.ReadLine();
                                    if (tempPassword.Length < 1)
                                    {
                                        ProgramState.MessageIfChoiceIsNotRight("Ditt lösenord måste innehålla tecken.");
                                    }

                                    else
                                    {
                                        password = tempPassword;
                                        correctPass = true;
                                    }
                                } while (correctPass == false);

                                do
                                {
                                    Console.Write("Ange den anställdes roll: ");
                                    string tempRole = Console.ReadLine();
                                    if (tempRole.ToLower() == "admin" || tempRole.ToLower() == "bagare" || tempRole.ToLower() == "kassör")
                                    {
                                        role = tempRole;
                                        ProgramState.SetFormerPosition();
                                        ProgramState.CURRENT_MENU = ProgramState.PROGRAM_MENUES.EMPLOYEES;
                                        correctRole = true;
                                    }
                                    else
                                    {
                                        ProgramState.MessageIfChoiceIsNotRight("Rollen finns inte.");
                                        Console.Clear();
                                    }
                                } while (correctRole == false);

                                if(correctPass == true && correctRole == true)
                                {
                                    await rep.AddEmployee(password, role);
                                    ProgramState.ConfirmationScreen("Den anställde är tillagd.");
                                }                              
                            }
                        }
                        break;
                    case ProgramState.PROGRAM_MENUES.UPDATE_EMPLOYEE:
                        {
                            string password = "";
                            string role = "";

                            Console.Clear();

                            Console.WriteLine("~~ UPPDATERA ANSTÄLLD ~~");
                            Console.WriteLine("Klicka på backspace för att gå tillbaka.");

                            /**
                             * LÄGGA TILL GÅ TILLBAKA FUNKTION MEN I ANNAN THREAD SÅ ATT MAN KAN SKRIVA OCH GÅ TILLBAKA SAMTIDIGT?
                             **/

                            Console.Write("Ange ID på den anställde som du vill uppdatera: ");

                            int.TryParse(Console.ReadLine(), out int choice);

                            if (choice == 1)
                            {
                                Console.Clear();
                                Console.WriteLine("Ange vad du vill ändra hos användaren:");
                                Console.WriteLine("1. Lösenord");
                                Console.WriteLine("2. Roll");

                                bool userInput = int.TryParse(Console.ReadLine(), out int changeChoice);

                                if (userInput == true)
                                {
                                    bool correctPass = false, correctRole = false;
                                    // ändra lösenord
                                    if (changeChoice == 1)
                                    {
                                        while (correctPass == false)
                                        {
                                            Console.Clear();
                                            Console.Write("Ange den anställdes nya lösenord: ");
                                            string tempPass = Console.ReadLine();
                                            if (tempPass.Length < 1)
                                            {
                                                ProgramState.MessageIfChoiceIsNotRight("Lösenordet måste innehålla tecken.");
                                            }
                                            else
                                            {
                                                password = tempPass;
                                                ProgramState.ConfirmationScreen("Lösenordet ändrat.");
                                                correctPass = true;
                                            }
                                        }
                                    }

                                    // ändra roll
                                    else if (changeChoice == 2)
                                    {
                                        while (correctRole == false)
                                        {
                                            Console.Clear();
                                            Console.Write("Ange användarens nya roll: ");
                                            string tempRole = Console.ReadLine();
                                            if (tempRole.ToLower() == "admin" || tempRole.ToLower() == "bagare" || tempRole.ToLower() == "kassör")
                                            {
                                                role = tempRole;
                                                ProgramState.ConfirmationScreen("Rollen ändrad.");
                                                correctRole = true;
                                            }

                                            else
                                            {
                                                ProgramState.MessageIfChoiceIsNotRight("Rollen finns inte.");
                                            }
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
                            char charChoice;
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
                                Console.WriteLine("Klicka på backspace för att gå tillbaka");
                                charChoice = Console.ReadKey().KeyChar;
                                wantToGoBack = ProgramState.GoBackOption(charChoice);
                            }
                        }
                        break;
                    case ProgramState.PROGRAM_MENUES.DELETE_EMPLOYEE:
                        {
                            Console.Clear();
                            Console.WriteLine("~~ TA BORT ANSTÄLLD ~~");
                            Console.WriteLine("Klicka på backspace för att gå tillbaka.");
                            Console.WriteLine();

                            /**
                             * PLATS FÖR FUNKTION ATT KUNNA GÅ TILLBAKA
                             **/

                            Console.Write("Ange ID för den anställda som du vill ta bort: ");

                            bool userInput = int.TryParse(Console.ReadLine(), out int id);

                            if (userInput == true)
                            {
                                Console.WriteLine($"Är du säker på att du vill ta bort anställd {id}? j/n");
                                Console.WriteLine("Valet kan inte ändras.");
                                string choice = Console.ReadLine();
                                if (choice == "j")
                                {
                                    // kör funktion för delete
                                    ProgramState.ConfirmationScreen("Anställd borttagen");
                                    ProgramState.CURRENT_MENU = ProgramState.FORMER_MENU;

                                }
                                else { ProgramState.CURRENT_MENU = ProgramState.FORMER_MENU; }
                            }

                        }
                        break;

                    // Pizzas
                    case ProgramState.PROGRAM_MENUES.PIZZAS:
                        {
                        }
                        break;

                    // Ingredients
                    case ProgramState.PROGRAM_MENUES.INGREDIENTS:
                        {
                        }
                        break;

                    // Extras
                    case ProgramState.PROGRAM_MENUES.EXTRAS:
                        {
                        }
                        break;

                    // Old orders
                    case ProgramState.PROGRAM_MENUES.OLD_ORDERS:
                        {
                        }
                        break;

                    default:
                        ProgramState.Running = false;
                        break;
                }
            }
        }
    }
}
