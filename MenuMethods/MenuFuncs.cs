﻿using System;
using System.Threading;
using System.Reflection;

namespace MenuMethods
{
    public static class MenuFuncs
    {
        public static string RunMainMenu(string[][] mainMenuChoice, object logInMenu, string logInFunctionName)
        {
            int numberOfChoices = PrintMenu(mainMenuChoice, "main").Item1;
            Console.Clear();
            string currentPosition = PrintMenu(mainMenuChoice, "main").Item2;
            string formerPositon = currentPosition;
            int choice = Console.ReadKey(true).KeyChar - '0';

            while(true)
            {
                formerPositon = currentPosition;
                currentPosition = UserChoice(choice, numberOfChoices, mainMenuChoice, currentPosition);

                if (currentPosition == "error")
                {
                    Console.Clear();
                    Console.WriteLine("Ditt val är felaktigt.");
                    Thread.Sleep(1500);
                    Console.Clear();
                    PrintMenu(mainMenuChoice, formerPositon);
                    choice = Console.ReadKey(true).KeyChar - '0';
                }

                else if (currentPosition == "logout")
                {
                    Console.Clear();
                    MethodInfo mi = logInMenu.GetType().GetMethod(logInFunctionName);
                    object[] parametersForLogIn = new object[] { mainMenuChoice };
                    mi.Invoke(logInMenu, parametersForLogIn);
                    break;
                }

                else
                {
                   Console.Clear();
                   numberOfChoices = PrintMenu(mainMenuChoice, currentPosition).Item1;
                   Console.Clear();
                   currentPosition = PrintMenu(mainMenuChoice, currentPosition).Item2;
                   choice = Console.ReadKey(true).KeyChar - '0';

                    if(currentPosition != "main" && choice < (numberOfChoices - 1))
                    {
                        break; // använd invoke för att anropa rätt typ av funktion härifrån
                    }

                }
            }

            return currentPosition;
        }

        public static (int, string) PrintMenu(string[][] menuChoice, string currentPosition)
        {
            int counter = 1;

            for (int i = 0; i < menuChoice.Length; i++)
            {
                if (menuChoice[i][0] == currentPosition)
                {
                    for (int menuItems = 1; menuItems < menuChoice[i].Length; menuItems++)
                    {
                        Console.WriteLine(counter + ". " + menuChoice[i][menuItems]);
                        counter++;
                    }
                }
            }

            return (counter, currentPosition);
        }

        public static string UserChoice(int userChoice, int nrOfChoices, string[][] menuChoice, string currentPosition)
        {
            string newPosition = currentPosition;

            for (int i = 1; i < nrOfChoices; i++)
            {
                if (userChoice == i && userChoice < (nrOfChoices - 1))
                {
                    newPosition = menuChoice[i][0];
                    break;
                }

                else if (userChoice == (nrOfChoices - 1) && currentPosition == "main")
                {
                    // kontrollerar om vi befinner oss i huvudmenyn och således om vi ska logga ut, vid övriga går vi tillbaka till huvudmenyn
                    newPosition = "logout";
                    break;
                }

                else if (userChoice == (nrOfChoices - 1))
                {
                    newPosition = "main";
                    break;
                }

                else if (userChoice >= nrOfChoices)
                {
                    newPosition = "error";
                    break;
                }
            }

            return newPosition;
        }

    }
}