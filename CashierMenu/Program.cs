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

namespace CashierMenu
{
    public static class ProgramState
    {
        public static bool Running { get; set; } = true;
        public enum PROGRAM_MENUES
        {
            LOGIN_SCREEN,
            ORDERS_SCREEN,
            CONFIRMATION_SCREEN
        }

        public static PROGRAM_MENUES CURRENT_MENU { get; set; } =
        PROGRAM_MENUES.LOGIN_SCREEN;
    }
    public class Program
    {
        public static IDatabase rep { get; set; }

        public static int IDOfOrderToServe { get; set; }

        public static Employee servingEmployee { get; set; }
        static async Task Main(string[] args)
        {
            #region Read configfile and populate lists
            rep = Helpers.GetSelectedBackend(File.ReadAllLines(Environment.CurrentDirectory + "\\Backend.cfg").First(s => !s.StartsWith("#")));
            #endregion


            while (ProgramState.Running)
            {
                switch (ProgramState.CURRENT_MENU)
                {
                    case ProgramState.PROGRAM_MENUES.LOGIN_SCREEN:
                        {
                        start:
                            Console.Clear();
                            Console.WriteLine("~ VÄLKOMMEN ATT LOGGA IN FÖR DELA UT FÄRDIGA ORDRAR ~");
                            var loginInItems = await Menus.PrintAndReturnStateOfLogin(rep, "kassör");
                            bool correctLogin = loginInItems.Item1;
                            servingEmployee = loginInItems.Item2;
                            if (correctLogin == true) { ProgramState.CURRENT_MENU = ProgramState.PROGRAM_MENUES.ORDERS_SCREEN; }

                        }
                        break;
                    case ProgramState.PROGRAM_MENUES.ORDERS_SCREEN:
                        {
                            Console.Clear();
                            Console.WriteLine("Följande ordrar är redo för servering:");

                            var allOrders = await rep.GetAllOrders();    // hämtar alla order i databasen


                            // går igenom alla ordrar och kontrollerar dess status. Om status är 2 (redo att servera) så läggs den in i en IEnumerable kallad ordersReadyToServe
                            var ordersReadyToServe = allOrders.Select(x => new { ID = x.OrderID, Status = x.Status }).Where(x => x.Status == 2);

                            //sedan skrivs dessa ordrar ut
                            foreach (var readyOrders in ordersReadyToServe)
                            {
                                Console.WriteLine(readyOrders.ID);
                            }

                            Console.WriteLine("\nKlicka på ESC för att logga ut.");
                            Console.Write("\n\nAnge numret på den order som är redo att serveras: ");
                            string OrderToServe = await Menus.ReadLineWithOptionToGoBack();

                            if (OrderToServe == null) { ProgramState.CURRENT_MENU = ProgramState.PROGRAM_MENUES.LOGIN_SCREEN; }
                            else
                            {
                                bool correctInput = int.TryParse(OrderToServe, out int ID);
                                if (correctInput == true)
                                {

                                    foreach (var order in ordersReadyToServe)
                                    {
                                        if (ID == order.ID)
                                        {
                                            IDOfOrderToServe = ID;
                                            ProgramState.CURRENT_MENU = ProgramState.PROGRAM_MENUES.CONFIRMATION_SCREEN;
                                        }
                                        else
                                        {
                                            await Menus.MessageIfChoiceIsNotRight("Den angivna ordern finns inte redo att serveras.");
                                        }
                                    }
                                }
                                else
                                {
                                    await Menus.MessageIfChoiceIsNotRight("Felaktig inmatning.");
                                    ProgramState.CURRENT_MENU = ProgramState.PROGRAM_MENUES.ORDERS_SCREEN;
                                }
                            }
                        }
                        break;

                    case ProgramState.PROGRAM_MENUES.CONFIRMATION_SCREEN:
                        {
                            Console.Clear();
                            Console.WriteLine($"Skriv y om du servera order {IDOfOrderToServe}?");
                            Console.WriteLine("Klicka ESC för att gå tillbaka.");

                            Console.Write("\nDitt svar: ");
                            string confirmationOfServing = await Menus.ReadLineWithOptionToGoBack();

                            if (confirmationOfServing == null) { ProgramState.CURRENT_MENU = ProgramState.PROGRAM_MENUES.ORDERS_SCREEN; }
                            else if (confirmationOfServing.ToLower() == "y")
                            {
                                var allOrders = await rep.GetAllOrders();
                                Order servedOrder = allOrders.Where(x => x.OrderID == IDOfOrderToServe).First();
                                await rep.UpdateOrderStatusWhenOrderIsServed(servingEmployee, servedOrder);
                                await Menus.ConfirmationScreen("Order serverad.", "Du skickas nu tillbaka till orderlistan.");
                                ProgramState.CURRENT_MENU = ProgramState.PROGRAM_MENUES.ORDERS_SCREEN;
                            }
                            else { await Menus.MessageIfChoiceIsNotRight("Vänligen bekräfta om ordern ska serveras."); }
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
