using System;
using System.IO;
using System.Linq;
using BackendHandler;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Threading;

namespace InfoMenu
{
    public enum GetStatus { Paid = 0, Baking = 1, Ready = 2, Finished = 3 }
    class Program
    {
        public static IDatabase rep { get; set; }

        static async Task Main(string[] args)
        {
            rep = Helpers.GetSelectedBackend(File.ReadAllLines(Environment.CurrentDirectory + "\\Backend.cfg").First(s => !s.StartsWith("#")));


            //await ShowMenu();
            await ShowMenuWithStatusCheck();
        }
        

        public static async Task ShowMenuWithStatusCheck()
        {
            GetStatus baking = GetStatus.Baking;
            GetStatus ready = GetStatus.Ready;
            var ordersBaking = await rep.GetOrderByStatus((int)baking);
            var ordersReady = await rep.GetOrderByStatus((int)ready);
            int checkBaking = ordersBaking.Count();
            int checkReady = ordersReady.Count();

            //Dessa loopar är för att skriva ut all som finns när programmet startar. 
            Console.WriteLine($"~~PÅGÅENDE ORDRAR~~\n");

            foreach (var order in ordersBaking)
            {
                Console.WriteLine($"        {order.OrderID}\n");
            }

            Console.WriteLine("~~KLARA ATT HÄMTA~~\n");
            foreach (var order in ordersReady)
            {
                Console.WriteLine($"        {order.OrderID}\n");
            }

            //Här startar delen där skärmen uppdateras. uppdatering sker var 5:e sekund mot databasen
            startUpdating:

            if (checkBaking == ordersBaking.Count() && checkReady == ordersReady.Count())
            {
                ordersBaking = await rep.GetOrderByStatus((int)baking);

                ordersReady = await rep.GetOrderByStatus((int)ready);
            }
            else
            {
                Console.Clear();
                Console.WriteLine($"~~PÅGÅENDE ORDRAR~~\n");

                foreach (var order in ordersBaking)
                {
                    Console.WriteLine($"        {order.OrderID}\n");
                }

                Console.WriteLine("~~KLARA ATT HÄMTA~~\n");
                foreach (var order in ordersReady)
                {
                    Console.WriteLine($"        {order.OrderID}\n");
                }
                checkBaking = ordersBaking.Count();
                checkReady = ordersReady.Count();
            }

            Thread.Sleep(5000);
            goto startUpdating;
        }

        /* public static async Task ShowMenu()
 {
     var orders = await rep.GetAllOrders();
     IEnumerable<Order> lookingForChangeList = orders;

     do
     {
         if (orders == lookingForChangeList) //Dessa anses inte vara lika. 
         {
             orders = await rep.GetAllOrders(); //Här händer det något när den hämtar. Trots att inget är ändrat i databasen så blir det skillnad på orders och lookingForChangeList
         }
         else
         {
             System.Threading.Thread.Sleep(2500);

             Console.Clear();
             Console.WriteLine($"~~PÅGÅENDE ORDRAR~~                 ~~KLARA ATT HÄMTA~~\n");
             var ongoingOrders = orders.Where(x => x.Status == 0).Take(5); //Take(5) för att bara lista max 5 åt gången.

             //Plockar ut alla ordrar i listan som har status 1 och är därmed hos kocken
             foreach (var order in ongoingOrders)
             {
                 Console.WriteLine($"        {order.OrderID}\n");
             }

             //Console.WriteLine($"~~KLARA ATT HÄMTA~~\n");
             var completedOrders = orders.Where(x => x.Status == 1).Take(5);

             //Plockar ut alla ordrar i listan som har status 2 och är därmed hos kassörskan
             foreach (var order in completedOrders)
             {
                 Console.SetCursorPosition(37, 2);
                 Console.WriteLine($"        {order.OrderID}\n");
             }
             lookingForChangeList = orders;
         }

     } while (true);
 }*/
    }
}


