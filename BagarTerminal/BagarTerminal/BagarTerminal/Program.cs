using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using System.Text.RegularExpressions;
using System.IO;
using BackendHandler;
using MenuFunctions;
using System.Data;

namespace BagarTerminal
{
    public static class ProgramState
    {
        public static bool Running { get; set; } = true;
        public enum PROGRAM_MENUES
        {
            LOGIN_MENU,
            MAIN_MENU,
            CHOSEN_ORDER
            
        }

        public static PROGRAM_MENUES CURRENT_MENU { get; set; } =
          PROGRAM_MENUES.LOGIN_MENU;
    }


    class Program
    {
       
        public static IDatabase rep { get; set; }

        public static async Task Main(string[] args)
        {
            #region Read configfile and populate lists
            rep = Helpers.GetSelectedBackend(File.ReadAllLines(Environment.CurrentDirectory + "\\Backend.cfg").First(s => !s.StartsWith("#")));
            #endregion

            List<Order> listOfOrders = new List<Order>();
            Employee employee = new Employee();
            #region Variables
            int counter = 1;
            int calculateOrdersWaiting = 0;
            int i;
            int index = 0;
            int userInput = 0;
            int userInput2 = 0;
            #endregion

            while (ProgramState.Running)

                switch (ProgramState.CURRENT_MENU)
                {
                    #region Login
                    case ProgramState.PROGRAM_MENUES.LOGIN_MENU:
                        Console.WriteLine("~VÄLKOMMEN TILL BAGARTERMINALEN ~");
                        Console.WriteLine("--------------------------------");
                        (bool CorrectLogin, Employee emp) = (await Menus.PrintAndReturnStateOfLogin(rep, "bagare"));
                        if (CorrectLogin == true)
                        {
                            employee = emp;
                            ProgramState.CURRENT_MENU = ProgramState.PROGRAM_MENUES.MAIN_MENU;

                        }
                        else await Menus.PrintAndReturnStateOfLogin(rep, "bagare");
                        break;
                    #endregion

                    #region Main Menu
                    case ProgramState.PROGRAM_MENUES.MAIN_MENU:
                       
                        
                        listOfOrders = new List<Order>();
                        foreach (Order order in await rep.GetOrderByStatus(0)) //Lägger in alla betalda men ej tillagade ordrar i en lista.   
                        {
                            if (order.PizzaList.Count >= 1)
                            { 
                                listOfOrders.Add(order);
                            }
                            
                        }

                        //Skriver ut Huvudmenyn och alla ordrar
                        counter = 1;
                        calculateOrdersWaiting = 0;
                        DrawMainMenu(employee);
                        for (i = 0; i < 5; i++)
                        {

                            if (i < listOfOrders.Count )
                            {
                                Console.Write($"{counter}. \n");
                                for (index = 0; index < listOfOrders[i].PizzaList.Count; index++)
                                {
                                    DrawSinglePizza(listOfOrders, i, index);
                                    Console.WriteLine();
                                }
                                Console.WriteLine();
                                counter++;
                                calculateOrdersWaiting++;
                            }
                        }
                        for (int counter2 = counter; counter2 < 6; counter2++)
                        {
                            Console.WriteLine($"{counter}. ---------------");
                            counter++;
                        }
                        Console.WriteLine("\n6. Logga ut.");

                        if (calculateOrdersWaiting < 0)
                        {
                            calculateOrdersWaiting = 0;
                        }
                        int ordersLeft = listOfOrders.Count - calculateOrdersWaiting; // räknar ut hur många ordrar som står i kö efter de som är utskrivna i huvudmenyn.
                        Console.WriteLine($"\nDet finns ytterligare {ordersLeft} ordrar som väntar.");
                        
                      
                        bool correctInput = int.TryParse(await Menus.ReadLineWithOptionToGoBack(), out userInput);
                        if (correctInput == true && userInput > 0 && userInput < 7)
                        {                                                              
                            if (userInput == 6)
                            {
                                ProgramState.CURRENT_MENU = ProgramState.PROGRAM_MENUES.LOGIN_MENU;
                                Console.Clear();

                            }
                            else if ((userInput) > listOfOrders.Count)
                            {
                                ProgramState.CURRENT_MENU = ProgramState.PROGRAM_MENUES.MAIN_MENU;
                            }
                            else
                            {
                               
                                ProgramState.CURRENT_MENU = ProgramState.PROGRAM_MENUES.CHOSEN_ORDER;
                            }
                        }
                        break;
                    #endregion

                    #region Chosen Order
                    case ProgramState.PROGRAM_MENUES.CHOSEN_ORDER: 

                        Console.Clear();
                        
                        userInput -= 1;
                        Console.WriteLine($"Du har valt att tillaga Order: \n");
                        for (index = 0; index < listOfOrders[userInput].PizzaList.Count; index++)
                        {
                            DrawSinglePizza(listOfOrders, userInput, index);
                            Console.WriteLine();
                        }
                        Console.WriteLine();
                        Console.WriteLine("Vill du bekräfta order? \n1. Ja 2. Nej");


                        bool CorrectInput2 = int.TryParse(await Menus.ReadLineWithOptionToGoBack(), out userInput2);
                        if (CorrectInput2 == true && userInput2 == 1)
                        {

                            await BakeOrder(employee, listOfOrders[userInput]);
                            ProgramState.CURRENT_MENU = ProgramState.PROGRAM_MENUES.MAIN_MENU;

                        }
                        else if (userInput2 == 2)
                        {
                            ProgramState.CURRENT_MENU = ProgramState.PROGRAM_MENUES.MAIN_MENU;
                        }
                        else
                        {
                            ProgramState.CURRENT_MENU = ProgramState.PROGRAM_MENUES.CHOSEN_ORDER;
                        }
                        break;
                    #endregion

                    default:

                        ProgramState.Running = false;
                        break;
                }
        }

       
        static async Task BakeOrder(Employee employee, Order order)
        {

                await rep.BakerChoosingOrderToCook(employee, order);
                Console.Clear();
                Console.WriteLine("Tryck ESC för att avbryta tillagningen.");
                Console.WriteLine("\nJust nu tillagas: ");

                for (int index = 0; index < order.PizzaList.Count; index++)
                {
                    if ((Console.KeyAvailable && Console.ReadKey(true).Key == ConsoleKey.Escape))
                    {
                        await rep.BakerCancellingCooking(employee, order);
                        Console.WriteLine("\nAvbryter Order. Går tillbaka till huvudmenyn");
                        Thread.Sleep(4000);
                        return;
                    }
                List<Order> listToDrawOrder = new List<Order>(); // Listan används för att kunna skriva ut pizzan.
                listToDrawOrder.Add(order);
                int i = 0;
                DrawSinglePizza(listToDrawOrder, i, index);
               
                
                for (int dot = 0; dot < 4; dot++)
                    {
                        if ((Console.KeyAvailable && Console.ReadKey(true).Key == ConsoleKey.Escape))
                        {
                            await rep.BakerCancellingCooking(employee, order);
                            Console.WriteLine("\nAvbryter Order. Går tillbaka till huvudmenyn");
                            Thread.Sleep(4000);
                            return;
                        }
                        Thread.Sleep(2000);
                        Console.Write(".");
                    }
                    Console.WriteLine($" är klar.");

                }
                Console.WriteLine();

                await rep.UpdateOrderStatusWhenOrderIsCooked(order);
                Console.WriteLine("\nAlla rätter är tillagade. Går tillbaka till huvudmenyn.");
                Thread.Sleep(4000);

            
        }

        public static void DrawSinglePizza(List<Order>listOfOrders, int i, int index)
        {
            
            
                Console.Write($"-{listOfOrders[i].PizzaList[index].Type}");
                if (listOfOrders[i].PizzaList[index].PizzabaseID > 0)
                {
                Console.Write($", Botten: {listOfOrders[i].PizzaList[index].Pizzabase}, med (");  
                }
                int x = 0;
                foreach (var text in listOfOrders[i].PizzaList[index].PizzaIngredients)
                {
                    x++;

                    Console.Write($"{text.Type}");
                    if (listOfOrders[i].PizzaList[index].PizzaIngredients.Count > x)
                    {
                        Console.Write(", ");
                    }
                    else
                    {
                        Console.Write(")");
                    }
                }
                

            
            
        }

        public static void DrawMainMenu(Employee employee)
        {
            Console.Clear();
            Console.WriteLine("~VÄLKOMMEN TILL BAGARTERMINALEN ~");
            Console.WriteLine("--------------------------------");
            Console.WriteLine($"Inloggad som ID: {employee.UserID}.\nRoll: {employee.Role}");
            Console.WriteLine("Välj vad du vill tillaga.\n");
        }
    }
}
