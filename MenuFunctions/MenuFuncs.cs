using System;
using System.Threading;
using System.Text.RegularExpressions;
using BackendHandler;
using System.Collections.Generic;

namespace MenuFunctions
{
    public static class Menus
    {
        public static string[][] menuChoices { get; } =
        {
            // main menu (0)
            new string[] { "Anställda", "Pizzor", "Ingredienser", "Tillbehör", "Gamla ordrar", "Logga ut" },
            // anställda (1)
            new string[] { "Lägg till anställd", "Uppdatera anställd", "Visa alla anställda", "Ta bort anställd", "Gå tillbaka" },
            // pizzor (2)
            new string[] { "Lägg till pizza", "Uppdatera pizza", "Visa alla pizzor", "Ta bort pizza", "Gå tillbaka" },
            // ingredienser (3)
            new string[] { "Lägg till ingrediens", "Uppdatera ingrediens", "Visa alla ingredienser", "Ta bort ingrediens", "Gå tillbaka" },
            // tillbehör (4)
            new string[] { "Lägg till tillbehör", "Uppdatera tillbehör", "Visa alla tillbehör", "Ta bort tillbehör", "Gå tillbaka" },
            // gamla ordrar (5)
            new string[] { "Visa gamla ordrar", "Ta bort gamla ordrar", "Gå tillbaka" }

        };

        public static void MessageIfChoiceIsNotRight(params string[] text)
        {
            Console.Clear();
            foreach (var item in text)
            {
                Console.WriteLine(item);
            }
            Thread.Sleep(1000);
            Console.Clear();
        }
        public static void ConfirmationScreen(params string[] text)
        {
            Console.Clear();
            foreach (var item in text)
            {
                Console.WriteLine(item);
            }
            Thread.Sleep(1000);

        }
        public static void PrintMenu(string[] menuChoices)
        {
            int counter = 1;

            foreach(string menuChoice in menuChoices)
            {
                Console.WriteLine($"{counter}. {menuChoice}");
                counter++;
            }
        }
        //Ny metod för att kolla igenom string input och se om det är siffror i stringen.
        public static bool RestrictNumericalInStrings(string input)
        {
            if(Regex.IsMatch(input, @"[a-öA-Ö]$")) { return true; }
            else { return false; }               
        }
        public static bool RestrictLettersInNumerical(string input)
        {
            if(Regex.IsMatch(input, @"[0-9]$")) { return true; }
            else { return false; }
        }
        public static string ShowPasswordAsStarsWithOptionToGoBack()
        {
            {
                string input = "";
                int stringIndex = 0;

                do
                {
                    ConsoleKeyInfo readKeyResult = Console.ReadKey(true);

                    if (readKeyResult.Key == ConsoleKey.Escape)
                    {
                        return null;
                    }

                    // handle Enter
                    if (readKeyResult.Key == ConsoleKey.Enter)
                    {
                        return input;
                    }

                    if (readKeyResult.Key == ConsoleKey.Backspace)
                    {
                        if (stringIndex > 0)
                        {
                            input = input.Remove(input.Length - 1);
                            Console.Write(readKeyResult.KeyChar);
                            Console.Write(' ');
                            Console.Write(readKeyResult.KeyChar);
                            stringIndex--;
                        }
                    }

                    else
                    {
                        input += readKeyResult.KeyChar;
                        Console.Write("*");
                        stringIndex++;
                    }
                } while (true);
            }
        }
        public static string ReadLineWithOptionToGoBack()
        {
            {
                string input = "";
                int stringIndex = 0;

                do
                {
                    ConsoleKeyInfo readKeyResult = Console.ReadKey(true);

                    if (readKeyResult.Key == ConsoleKey.Escape) return null;

                    // handle Enter
                    if (readKeyResult.Key == ConsoleKey.Enter) { Console.WriteLine(); return input; }

                    if (readKeyResult.Key == ConsoleKey.Backspace)
                    {
                        if (stringIndex > 0)
                        {
                            input = input.Remove(input.Length - 1);
                            Console.Write(readKeyResult.KeyChar);
                            Console.Write(' ');
                            Console.Write(readKeyResult.KeyChar);
                            stringIndex--;
                        }
                    }

                    else
                    {
                        input += readKeyResult.KeyChar;
                        Console.Write(readKeyResult.KeyChar);
                        stringIndex++;
                    }
                } while (true);
            }
        }

    }

}
