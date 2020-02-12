using System;
using System.Collections.Generic;
using BackendHandler;
using System.Linq;
using Newtonsoft;
using Newtonsoft.Json;
using System.Data;
using System.Data.SqlClient;
using Dapper;
using System.Threading.Tasks;
using System.Threading;
using System.IO;
using System.Reflection;
using Npgsql;

namespace CustomerTerminal
{
    public static class ProgramState
    {
        public static bool Running { get; set; } = true;
        public enum PROGRAM_MENUES
        {
            WELLCOME_SCREEN,
            OPTIONS_MENU,
            CURRENT_ORDER,
            PIZZA_MENU,
            CUSTOMIZE_PIZZA_MENU,
            EXTRA_MENU,
            PAYMENT_MENU
        }
        public static PROGRAM_MENUES CURRENT_MENU { get; set; } =
          PROGRAM_MENUES.WELLCOME_SCREEN;
    }

    class Program
    {
        // Internal variables >>
        private static Order                CurrentOrder  { get; set; }
        private static List<Pizza>          Pizzas        { get; set; }
        private static List<Extra>          Products      { get; set; }
        private static List<Condiment>      Ingredients   { get; set; }
        private static List<(int, string)>  PizzaBases    { get; set; }
        private static Pizza                CurrentPizza  { get; set; }

        private static IDatabase Rep = BackendHandler.Helpers.GetSelectedBackend();

        /// <summary>
        /// Load all items from database
        /// </summary>
        public async static Task PreloadAllLists()
        {
            Products    = (await Rep.GetAllExtras()).ToList();
            Ingredients = (await Rep.GetAllCondiments()).ToList();
            Pizzas      =  BackendHandler.Helpers.LoadPizzasAsList(Rep);
            PizzaBases  = (await Rep.GetAllPizzabases()).ToList();
        }

        static async Task Main(string[] args)
        {
            Console.WriteLine("Hämtar produkter. . .");
            await PreloadAllLists();

            while (ProgramState.Running)
            {
                switch (ProgramState.CURRENT_MENU)
                {
                    case ProgramState.PROGRAM_MENUES.WELLCOME_SCREEN:
                        {
                            Menus.DrawWellcomeScreen();                                           // Draw wellcome screen
                            Console.ReadKey();                                                    // Wait for user input
                            CurrentOrder = new Order();                                           // Create new order 
                            CurrentOrder.PizzaList = new List<Pizza>();
                            CurrentOrder.ExtraList = new List<Extra>();
                            CurrentOrder.Price = 0;
                            ProgramState.CURRENT_MENU = ProgramState.PROGRAM_MENUES.OPTIONS_MENU; // Change program state
                        }
                        break;
                    case ProgramState.PROGRAM_MENUES.OPTIONS_MENU:
                        {
                            Menus.DrawOptionsScreen();                                    // Draw options
                            Console.Write(" ~ "); string Opt = Console.ReadLine();         // Wait for user input
                            if (int.TryParse(Opt, out int vOpt) && vOpt > 0 && vOpt <= 3)  // Let user choose
                            {
                                if (vOpt == 1) ProgramState.CURRENT_MENU = ProgramState.PROGRAM_MENUES.PIZZA_MENU;        // Goto pizza menu
                                else if (vOpt == 2) ProgramState.CURRENT_MENU = ProgramState.PROGRAM_MENUES.EXTRA_MENU;   // Goto Product menu
                                else if (vOpt == 3) ProgramState.CURRENT_MENU = ProgramState.PROGRAM_MENUES.CURRENT_ORDER;// Goto current order
                            }
                        }
                        break;
                    case ProgramState.PROGRAM_MENUES.PIZZA_MENU:
                        {
                            Menus.DrawPizzaMenu(Pizzas);                           // Draw options
                            Console.Write(" ~ "); string Opt = Console.ReadLine();  // Wait for user input
                            if (int.TryParse(Opt, out int Choice) && Choice > 0 && Choice <= Pizzas.Count + 1)
                            {
                                if (Choice == Pizzas.Count + 1) ProgramState.CURRENT_MENU = ProgramState.PROGRAM_MENUES.OPTIONS_MENU; // Go back to main menu
                                else
                                {
                                    // DONT SET 'CurrentPizza' = SELECTED PIZZA CUZ IT IS A !!REFERENCE!!
                                    CurrentPizza = new Pizza()
                                    {
                                        PizzabaseID = Pizzas[Choice - 1].PizzabaseID,
                                        Pizzabase = Pizzas[Choice - 1].Pizzabase,
                                        PizzaIngredients = new List<Condiment>(Pizzas[Choice - 1].PizzaIngredients),
                                        StandardIngredientsDeffinition = new List<int>(Pizzas[Choice - 1].StandardIngredientsDeffinition),
                                        Type = Pizzas[Choice - 1].Type,
                                        Price = Pizzas[Choice - 1].Price
                                    };
                                    /* Create new IngredientsList and copy over standard ingredients
                                     * Cuz of reference type
                                     */
                                    CurrentPizza.PizzaIngredients = new List<Condiment>(Pizzas[Choice - 1].PizzaIngredients);
                                    ProgramState.CURRENT_MENU = ProgramState.PROGRAM_MENUES.CUSTOMIZE_PIZZA_MENU;
                                }
                            }
                        }
                        break;
                    case ProgramState.PROGRAM_MENUES.CUSTOMIZE_PIZZA_MENU:
                        {
                            Console.Clear();
                            Menus.ShowPizza(CurrentPizza);
                            Console.Write(" ~ ");
                            string PizzaOption = Console.ReadLine();

                            if (int.TryParse(PizzaOption, out int PizzaOptionI))
                            {
                                if (PizzaOptionI == 1)
                                {
                                    CurrentOrder.PizzaList.Add(CurrentPizza);
                                    Console.ForegroundColor = ConsoleColor.Green; Console.WriteLine("Du har lagt till pizzan i din beställning, tryck på valfri knapp för att forstätta"); Console.ForegroundColor = ConsoleColor.White;
                                    Console.ReadKey();
                                    ProgramState.CURRENT_MENU = ProgramState.PROGRAM_MENUES.OPTIONS_MENU;
                                }
                                if (PizzaOptionI == 2) Menus.Edit_Pizza_Change_Base(CurrentPizza, PizzaBases);
                                if (PizzaOptionI == 3) Menus.Edit_Pizza_Add_Items(CurrentPizza, Ingredients); // Pass through the reference
                                if (PizzaOptionI == 4) Menus.Edit_Pizza_Remove_Items(CurrentPizza);           // Pass through the reference
                                if (PizzaOptionI == 5)
                                {
                                    if (Helpers.YesOrNo("\nÄr du säker på att du vill kasta din nuvarande pizza? (y/n) ", ConsoleColor.DarkRed))
                                        ProgramState.CURRENT_MENU = ProgramState.PROGRAM_MENUES.PIZZA_MENU;
                                    else break;
                                }
                            }
                        }
                        break;
                    case ProgramState.PROGRAM_MENUES.EXTRA_MENU:
                        {
                            Menus.DrawExtrasMenu(Products);                        // Draw options
                            Console.Write(" ~ "); string Opt = Console.ReadLine();  // Wait for user input
                            if (int.TryParse(Opt, out int Choice) && Choice > 0 && Choice <= Products.Count + 1)
                            {
                                if (Choice == Products.Count + 1) ProgramState.CURRENT_MENU = ProgramState.PROGRAM_MENUES.OPTIONS_MENU; // Go back to main menu
                                else
                                {
                                    CurrentOrder.ExtraList.Add(new Extra
                                    {
                                        Type = Products[Choice - 1].Type,
                                        Price = Products[Choice - 1].Price
                                    });
                                    Console.ForegroundColor = ConsoleColor.Green;
                                    Console.WriteLine($"{Products[Choice - 1].Type} har lagt till i din beställning, tryck på valfri knapp för att fortsätta");
                                    Console.ForegroundColor = ConsoleColor.White;
                                    Console.ReadKey();
                                }
                            }
                        }
                        break;
                    case ProgramState.PROGRAM_MENUES.CURRENT_ORDER:
                        {
                            if (CurrentOrder.PizzaList.Count.Equals(0) && CurrentOrder.ExtraList.Count.Equals(0))
                            {
                                Console.WriteLine("\nDu har för närvarande inga produkter i din beställning");
                                Console.ReadKey();
                                ProgramState.CURRENT_MENU = ProgramState.PROGRAM_MENUES.OPTIONS_MENU;
                            }
                            else
                            {
                                Menus.ShowOrder(CurrentOrder);
                                Console.Write(" ~ "); string Opt = Console.ReadLine();
                                if (int.TryParse(Opt, out int Choice) && Choice > 0 && Choice <= 3)
                                {
                                    if (Choice.Equals(3)) ProgramState.CURRENT_MENU = ProgramState.PROGRAM_MENUES.OPTIONS_MENU;
                                    else if (Choice.Equals(2)) ProgramState.CURRENT_MENU = ProgramState.PROGRAM_MENUES.PAYMENT_MENU;
                                    else if (Choice.Equals(1)) Menus.EditOrder(CurrentOrder, PizzaBases, Ingredients);
                                }
                            }

                        }
                        break;
                    case ProgramState.PROGRAM_MENUES.PAYMENT_MENU:
                        {
                            Console.Clear();
                            int ID = await Rep.AddOrder(CurrentOrder);
                            Random r = new Random();
                            string Bar = "[                              ]\nBetalningen kontrolleras. . .";
                            Console.WriteLine(Bar); Console.ForegroundColor = ConsoleColor.Yellow;
                            for (int i = 1; i <= 30; i++)
                            {
                                Console.SetCursorPosition(i, 0);
                                Console.Write("#");
                                Thread.Sleep(r.Next(25, 250));
                                Console.SetCursorPosition(33, 0);
                                Console.Write(((((float)i + 1) / 31) * 100F).ToString("0.0") + "%");
                            }
                            Console.SetCursorPosition(0, 1);                   Console.ForegroundColor = ConsoleColor.DarkGreen;
                            Console.Write("Betalningen godkänd             "); Console.ForegroundColor = ConsoleColor.White;
                            Console.Write("\n\nTryck på valfri knapp för att se ditt kvitto");
                            Console.ReadKey();

                            CurrentOrder.OrderID = ID;
                            Menus.DrawBeautifulReceipt(CurrentOrder);
                            Console.ReadKey();
                            ProgramState.CURRENT_MENU = ProgramState.PROGRAM_MENUES.WELLCOME_SCREEN;
                        }
                        break;
                    default: ProgramState.Running = false; break;
                }
            }
        }
    }


    /// <summary>
    /// Static menu drawer class
    /// </summary>
    public static class Menus
    {
        /// <summary>
        /// Draw the wellcome screen
        /// </summary>
        public static void DrawWellcomeScreen()
        {
            Console.Clear();
            Console.WriteLine("Välkommen till pizza palatset!");
            Console.WriteLine("Tryck på valfri knapp för att påbörja din beställning");
        }

        /// <summary>
        /// Draw the main menu screen
        /// </summary>
        public static void DrawOptionsScreen()
        {
            Console.Clear();
            Console.WriteLine("1: Pizzor");
            Console.WriteLine("2: Annat");
            Console.Write("3: ");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write("Visa beställning\n");
            Console.ForegroundColor = ConsoleColor.White;
        }

        /// <summary>
        /// Show all pizzas on the screen
        /// </summary>
        public static void DrawPizzaMenu(List<Pizza> PizzaList)
        {
            Console.Clear();
            Pizza[] Pizzas = PizzaList.ToArray();
            string[] Display = new string[Pizzas.Length];
            for (int i = 0; i < Pizzas.Length; i++)
                Display[i] = $"{i + 1}: {Pizzas[i].Type};{Pizzas[i].Price} SEK";
            foreach (string DisplayProp in Helpers.Format(Display, ';'))
                Console.WriteLine(DisplayProp);
            Console.Write($"{Pizzas.Length + 1}: ");

            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.Write("Gå tillbaka\n");
            Console.ForegroundColor = ConsoleColor.White;
        }

        /// <summary>
        /// Show all extra products on the screen
        /// </summary>
        public static void DrawExtrasMenu(List<Extra> Products)
        {
            Console.Clear();
            Extra[] Extras = Products.ToArray();
            string[] Display = new string[Extras.Length];
            for (int i = 0; i < Extras.Length; i++)
                Display[i] = $"{i + 1}: {Extras[i].Type};{Extras[i].Price} SEK";
            foreach (string DisplayProp in Helpers.Format(Display, ';'))
                Console.WriteLine(DisplayProp);
            Console.Write($"{Extras.Length + 1}: ");

            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.Write("Gå tillbaka\n");
            Console.ForegroundColor = ConsoleColor.White;
        }

        /// <summary>
        /// Show a pizza in the console
        /// </summary>
        /// <param name="p"></param>
        /// <param name="ShowStandardOptions"></param>
        /// <param name="ShowNumbers"></param>
        public static void ShowPizza(Pizza p, bool ShowStandardOptions = true)
        {
            foreach (string str in Helpers.Format(new List<string> { $"Namn;{p.Type}", $"Botten;{p.Pizzabase}", $"Pris;{p.Price} SEK" }.ToArray(), ';', ':'))
                Console.WriteLine(str);
            Console.Write("Ingridienser\n");

            List<string> ToFormat = new List<string>();
            for (int i = 0; i < p.PizzaIngredients.Count; i++)
            {          
                if (!Helpers.ShouldCalculateAsAddon(p, p.PizzaIngredients[i], i + 1)) ToFormat.Add("    ~ " + p.PizzaIngredients[i].Type + ": 0 SEK (Standard)");
                else ToFormat.Add("    ~ " + p.PizzaIngredients[i].Type + $": {p.PizzaIngredients[i].Price} SEK");
            }
            foreach(string str in Helpers.Format(ToFormat.ToArray(), ':', '-'))
                Console.WriteLine(str);

            if (ShowStandardOptions) // Else show only the pizza
            {
                Console.WriteLine("\n1: Lägg till i beställning");
                Console.WriteLine("2: Ändra pizza botten");
                Console.WriteLine("3: Extra tillägg");
                Console.WriteLine("4: Ta bort tillägg");
                Console.Write("5: ");
                Console.ForegroundColor = ConsoleColor.DarkRed; Console.Write("Gå tillbaka\n"); Console.ForegroundColor = ConsoleColor.White;
            }
        }

        /// <summary>
        /// Show all objects in a order
        /// </summary>
        /// <param name="o"></param>
        public static void ShowOrder(Order o, bool ShowOptions = true)
        {
            Console.Clear();
            float TotalPrice = 0;
            foreach (Pizza p in o.PizzaList)
            {
                TotalPrice += p.Price;
                ShowPizza(p, false);
            }
            foreach (Extra ep in o.ExtraList)
            {
                TotalPrice += ep.Price;
                Console.WriteLine(" ~ " + ep.Type + " + " + ep.Price + " SEK");
            }
            o.Price = TotalPrice;
            Console.WriteLine("\n Summa ~ " + TotalPrice + "SEK");

            if (ShowOptions)
            {
                Console.WriteLine();
                Console.WriteLine(" 1: Ändra din order");
                Console.WriteLine(" 2: Gå till betalning");
                Console.ForegroundColor = ConsoleColor.DarkRed; Console.WriteLine(" 3: Gå tillbaka"); Console.ForegroundColor = ConsoleColor.White;
            }
        }

        /// <summary>
        /// Useless function that only takes up space
        /// </summary>
        /// <param name="o"></param>
        public static void DrawBeautifulReceipt(Order o)
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("\tTony's pizzeria");
            Console.WriteLine("     13454 Silkroad valley");
            Console.WriteLine("\t  034-6839985");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("-------------------------------");
            Console.WriteLine("\t  Ordernummer");
            Console.WriteLine("\t      #" + o.OrderID);
            Console.WriteLine("-------------------------------");
            foreach (Pizza p in o.PizzaList) ShowPizza(p, false);
            List<string> ToFormat = new List<string>();
            foreach (Extra e in o.ExtraList) ToFormat.Add(e.Type + ":" + e.Price + " SEK");
            Console.WriteLine();
            Console.WriteLine();
            foreach (string str in Helpers.Format(ToFormat.ToArray(), ':', ' ')) Console.WriteLine(str);
            Console.WriteLine();
            Console.WriteLine("Moms:  " + ((o.Price / 100F) * 12).ToString("0.00") + " SEK");
            Console.WriteLine("Total: " + o.Price + " SEK");
            Console.WriteLine();
            Console.WriteLine("-------------------------------");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("     Tack för att du valde");
            Console.WriteLine("        tony's pizzeria");
            Console.ForegroundColor = ConsoleColor.White;

            Console.ReadKey();
        }

        /* Menus for editing specific pizzas, products and orders
         * ref pizza to be changed
         * provide ingredients-list from wich to choose ingredients from
         * Also handles input
         */
        public static void Edit_Pizza_Add_Items(Pizza CurrentPizza, List<Condiment> AvailableIngredients)
        {
            while (true)
            {
                Console.Clear();
                ShowPizza(CurrentPizza, false);
                Console.WriteLine();

                #region Just to write it out nice in the console
                Console.ForegroundColor = ConsoleColor.Green; Console.WriteLine(" Välj extra tillägg till pizzan"); Console.ForegroundColor = ConsoleColor.White;
                List<string> ToFormat = new List<string>();
                for (int i = 0; i < AvailableIngredients.Count; i++) 
                    ToFormat.Add($" {i + 1}: {AvailableIngredients[i].Type};" + ((Helpers.ShouldCalculateAsAddon(CurrentPizza, AvailableIngredients[i], CurrentPizza.PizzaIngredients.Count(), 1)) ? $"{AvailableIngredients[i].Price} SEK" : "0 SEK (Standard)"));
                ToFormat = Helpers.Format(ToFormat.ToArray(), ';', '~').ToList();
                foreach (string row in ToFormat) Console.WriteLine(row);
                Console.ForegroundColor = ConsoleColor.DarkRed; Console.WriteLine($" {ToFormat.Count + 1}: Gå tillbaka"); Console.ForegroundColor = ConsoleColor.White;
                Console.Write(" ~ ");
                #endregion

                string Opt = Console.ReadLine();
                if (int.TryParse(Opt, out int Choice) && Choice > 0 && Choice <= ToFormat.Count + 1)
                {
                    if (Choice.Equals(ToFormat.Count + 1)) break;
                    else
                    {
                        CurrentPizza.PizzaIngredients.Add(AvailableIngredients[Choice - 1]);
                        if (Helpers.ShouldCalculateAsAddon(CurrentPizza, AvailableIngredients[Choice - 1], CurrentPizza.PizzaIngredients.Count())) CurrentPizza.Price += AvailableIngredients[Choice - 1].Price;
                    }
                }

            }
        }
        public static void Edit_Pizza_Remove_Items(Pizza CurrentPizza)
        {
            while (true)
            {
                Console.Clear();
                ShowPizza(CurrentPizza, false);
                Console.WriteLine();

                #region Just to write it out nice in the console
                Console.ForegroundColor = ConsoleColor.Green; Console.WriteLine(" Välj tillägg att ta bort ifrån pizzan"); Console.ForegroundColor = ConsoleColor.White;
                List<string> ToFormat = new List<string>();
                for (int i = 0; i < CurrentPizza.PizzaIngredients.Count; i++) 
                    ToFormat.Add($" {i + 1}: {CurrentPizza.PizzaIngredients[i].Type};" + ((Helpers.ShouldCalculateAsAddon(CurrentPizza, CurrentPizza.PizzaIngredients[i], i + 1)) ? $" - {CurrentPizza.PizzaIngredients[i].Price} SEK": " - 0 SEK (Standard)"));
                ToFormat = Helpers.Format(ToFormat.ToArray(), ';', '~').ToList();
                foreach (string row in ToFormat) Console.WriteLine(row);
                Console.ForegroundColor = ConsoleColor.DarkRed; Console.WriteLine($" {ToFormat.Count + 1}: Gå tillbaka"); Console.ForegroundColor = ConsoleColor.White;
                Console.Write(" ~ ");
                #endregion

                string Opt = Console.ReadLine();
                if (int.TryParse(Opt, out int Choice) && Choice > 0 && Choice <= ToFormat.Count + 1)
                {
                    if (Choice.Equals(ToFormat.Count + 1)) break;
                    else
                    {
                        if (!CurrentPizza.StandardIngredientsDeffinition.Contains(CurrentPizza.PizzaIngredients[Choice - 1].CondimentID)||
                            CurrentPizza.PizzaIngredients.Select(P => P.CondimentID).Count() > CurrentPizza.StandardIngredientsDeffinition.Select(I => I).Count()
                            )
                            CurrentPizza.Price -= CurrentPizza.PizzaIngredients[Choice - 1].Price;
                        CurrentPizza.PizzaIngredients.RemoveAt(Choice - 1);
                    }
                }

            }
        }
        public static void Edit_Pizza_Change_Base(Pizza CurrentPizza, List<(int, string)> AvailablePizzaBases)
        {
            while (true)
            {
                Console.Clear();
                Console.ForegroundColor = ConsoleColor.Green; Console.WriteLine(" Välj vilken botten du viil ha på pizzan"); Console.ForegroundColor = ConsoleColor.White;
                for (int i = 0; i < AvailablePizzaBases.Count; i++) Console.WriteLine($" {i + 1}: {AvailablePizzaBases[i].Item2}");
                Console.ForegroundColor = ConsoleColor.DarkRed; Console.WriteLine($" {AvailablePizzaBases.Count + 1}: Gå tillbaka"); Console.ForegroundColor = ConsoleColor.White;
                Console.Write(" ~ "); string Opt = Console.ReadLine();
                if (int.TryParse(Opt, out int Choice) && Choice > 0 && Choice <= AvailablePizzaBases.Count + 1)
                {
                    if (Choice.Equals(AvailablePizzaBases.Count + 1)) break;
                    else
                    {
                        CurrentPizza.Pizzabase = AvailablePizzaBases[Choice - 1].Item2;
                        CurrentPizza.PizzabaseID = AvailablePizzaBases[Choice - 1].Item1;
                        break;
                    }
                }
            }
        }
        public static void EditOrder(Order o, List<(int, string)> AvailableBases, List<Condiment> AvailableIngredients)
        {
            while (true)
            {
                #region Write reciete
                Console.Clear();
                int EndCount = 1;
                for (int i = 0; i < o.PizzaList.Count; i++)
                {
                    Console.ForegroundColor = ConsoleColor.Green; Console.Write(EndCount); Console.Write(": Ändra\n"); Console.ForegroundColor = ConsoleColor.White;
                    ShowPizza(o.PizzaList[i], false);
                    EndCount++;
                    Console.WriteLine();
                }
                Console.WriteLine();
                for (int i = 0; i < o.ExtraList.Count; i++)
                {
                    Console.Write($" { EndCount}: ");
                    Console.ForegroundColor = ConsoleColor.DarkRed; Console.Write("Ta bort "); Console.ForegroundColor = ConsoleColor.White;
                    Console.Write(o.ExtraList[i].Type + "\n");
                    EndCount++;
                }
                #endregion

                Console.WriteLine(); Console.Write($" { EndCount}: ");
                Console.ForegroundColor = ConsoleColor.DarkRed; Console.WriteLine("Gå tillbaka"); Console.ForegroundColor = ConsoleColor.White;
                Console.Write(" ~ "); string Opt = Console.ReadLine();
                if (int.TryParse(Opt, out int Choice) && Choice > 0 && Choice <= EndCount)
                {
                    if (Choice.Equals(EndCount)) break;
                    else if (Choice <= o.PizzaList.Count)
                    {
                        while (true)
                        {
                            Console.Clear();
                            ShowPizza(o.PizzaList[Choice - 1], false);
                            Console.WriteLine("1: Ändra pizza botten");
                            Console.WriteLine("2: Extra tillägg");
                            Console.WriteLine("3: Ta bort tillägg");
                            Console.Write("4: ");
                            Console.ForegroundColor = ConsoleColor.Green; Console.Write("Klar\n"); Console.ForegroundColor = ConsoleColor.White;
                            Console.Write(" ~ ");
                            string PizzaOption = Console.ReadLine();

                            if (int.TryParse(PizzaOption, out int PizzaOptionI))
                            {
                                if (PizzaOptionI == 1) Edit_Pizza_Change_Base(o.PizzaList[Choice - 1], AvailableBases);     // Pass through the reference
                                if (PizzaOptionI == 2) Edit_Pizza_Add_Items(o.PizzaList[Choice - 1], AvailableIngredients); // Pass through the reference
                                if (PizzaOptionI == 3) Edit_Pizza_Remove_Items(o.PizzaList[Choice - 1]);                    // Pass through the reference
                                if (PizzaOptionI == 4) break;
                            }
                        }
                    }
                    else
                    {
                        Choice = (Choice - o.PizzaList.Count) - 1;
                        if (Helpers.YesOrNo($" Är du säker på att du vill ta bort {o.ExtraList[Choice].Type}? (y/n)", ConsoleColor.DarkRed)) o.ExtraList.RemoveAt(Choice);
                    }

                }
            }
        }
    }

    public static class Helpers
    {
        /// <summary>
        /// Formats an array of strings for display
        /// </summary>
        /// <param name="rows"></param>
        /// <param name="splittingSymbol"></param>
        /// <param name="devidingSymbol"></param>
        /// <returns></returns>
        public static string[] Format(string[] rows, char splittingSymbol, char devidingSymbol = '-')
        {
            string[] Result = new string[rows.Length];
            for (int count = 0; count < rows.Length; count++)
            {
                int FieldCount = rows[count].Split(splittingSymbol).Length;
                string CurrentField = "";
                for (int i = 0; i < FieldCount; i++)
                {
                    int CharCount = 0;
                    try
                    {
                        for (int i2 = 0; i2 < rows.Length; i2++)
                            if (rows[i2].Split(splittingSymbol)[i].Length > CharCount)
                                CharCount = rows[i2].Split(splittingSymbol)[i].Length;
                    }
                    catch { }

                    CurrentField += rows[count].Split(splittingSymbol)[i];

                    try
                    {
                        for (int i2 = rows[count].Split(splittingSymbol)[i].Length; i2 < CharCount; i2++)
                            CurrentField += " ";
                        if (i != FieldCount - 1)
                            CurrentField += $" {devidingSymbol} ";
                    }
                    catch { }
                }
                Result[count] = CurrentField;
            }
            return Result;
        }

        public static bool YesOrNo(string Message, ConsoleColor cc)
        {
            Console.ForegroundColor = cc; Console.Write(Message); Console.ForegroundColor = ConsoleColor.White;
            while (true)
            {
                char o = Console.ReadKey(true).KeyChar;

                if (o.Equals('y')) { return true; }
                else if (o.Equals('n')) return false;
            }
        }

        public static bool ShouldCalculateAsAddon(Pizza CurrentPizza, Condiment Ingredient, int StoppIndex, int X = 0)
        {
            if (CurrentPizza.StandardIngredientsDeffinition.Contains(Ingredient.CondimentID))
            {
                if (CurrentPizza.PizzaIngredients.GetRange(0, StoppIndex).Where(p => p.CondimentID.Equals(Ingredient.CondimentID)).Count() + X <=
                    CurrentPizza.StandardIngredientsDeffinition.Where(I => I.Equals(Ingredient.CondimentID)).Count())
                    return false;
                else return true;
            }
            else return true;
        }
    }
}
