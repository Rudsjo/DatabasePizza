using System;
using System.Threading;
using System.Reflection;

namespace MenuFunctions
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
