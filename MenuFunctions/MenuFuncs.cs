using System;
using System.Threading;
using System.Text.RegularExpressions;
using BackendHandler;
using System.Collections.Generic;

namespace MenuFunctions
{
    public static class MenuFuncs
    {
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
