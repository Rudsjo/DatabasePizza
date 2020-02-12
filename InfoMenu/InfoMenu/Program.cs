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

            await ShowMenuWithStatusCheck();
        }
        public static async Task ShowMenuWithStatusCheck()
        {
            GetStatus paid = GetStatus.Paid;
            GetStatus baking = GetStatus.Baking;
            GetStatus ready = GetStatus.Ready;
            var ordersPaid = await rep.GetOrderByStatus((int)paid);
            var ordersBaking = await rep.GetOrderByStatus((int)baking);
            var ordersReady = await rep.GetOrderByStatus((int)ready);
            int checkPaid = ordersPaid.Count();
            int checkBaking = ordersBaking.Count();
            int checkReady = ordersReady.Count();
            int newRowCount = 0;

            //Dessa loopar är för att skriva ut all som finns när programmet startar. 
            Console.WriteLine($"~~PÅGÅENDE ORDRAR~~\n");
            foreach (var order in ordersPaid)
            {
                if (newRowCount <= 4) { Console.Write($"{order.OrderID}, "); newRowCount++; }
                else { Console.Write($"{order.OrderID}\n"); newRowCount = 0; }
            }
            foreach (var order in ordersBaking)
            {
                if(newRowCount <= 4) {Console.Write($"{order.OrderID}, "); newRowCount++; }
                else { Console.Write($"{order.OrderID}\n"); newRowCount = 0; }
            }           

            Console.WriteLine("\n\n~~KLARA ATT HÄMTA~~\n");
            newRowCount = 0;
            foreach (var order in ordersReady)
            {
                if (newRowCount <= 4) { Console.Write($"{order.OrderID}, "); newRowCount++; }
                else { Console.Write($"{order.OrderID}\n"); newRowCount = 0; }
            }

            //Här startar delen där skärmen uppdateras. uppdatering sker var 5:e sekund mot databasen
            startUpdating:

            if (checkBaking == ordersBaking.Count() && checkReady == ordersReady.Count())
            {
                ordersPaid = await rep.GetOrderByStatus((int)paid);
                ordersBaking = await rep.GetOrderByStatus((int)baking);
                ordersReady = await rep.GetOrderByStatus((int)ready);
            }
            else
            {
                newRowCount = 0;
                Console.Clear();
                Console.WriteLine($"~~PÅGÅENDE ORDRAR~~\n");

                foreach (var order in ordersPaid)
                {
                    if (newRowCount <= 4) { Console.Write($"{order.OrderID}, "); newRowCount++; }
                    else { Console.Write($"{order.OrderID}\n"); newRowCount = 0; }
                }
                foreach (var order in ordersBaking)
                {
                    if (newRowCount <= 4) { Console.Write($"{order.OrderID}, "); newRowCount++; }
                    else { Console.Write($"{order.OrderID}\n"); newRowCount = 0; }
                }

                newRowCount = 0;
                Console.WriteLine("\n\n~~KLARA ATT HÄMTA~~\n");
                foreach (var order in ordersReady)
                {
                    if (newRowCount <= 4) { Console.Write($"{order.OrderID}, "); newRowCount++; }
                    else { Console.Write($"{order.OrderID}\n"); newRowCount = 0; }
                }
                checkPaid = ordersPaid.Count();
                checkBaking = ordersBaking.Count();
                checkReady = ordersReady.Count();
            }
            Thread.Sleep(5000);
            goto startUpdating;
        }
    }
}


