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

        public static PROGRAM_MENUES FORMER_MENU { get; set; } =
            PROGRAM_MENUES.MAIN_MENU;



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

        public static bool GoBackOption(char charChoice)
        {
            if (charChoice == 8)
            {
                ProgramState.CURRENT_MENU = ProgramState.FORMER_MENU;
                return true;
            }

            else
            {
                return false;
            }

        }
        public static void SetFormerPosition()
        {
            ProgramState.FORMER_MENU = ProgramState.CURRENT_MENU;
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

    }
}
