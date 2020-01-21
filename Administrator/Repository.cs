using System;
using System.Collections.Generic;
using System.Text;
using Dapper;
using System.Data.SqlClient;
using System.Threading.Tasks;
using System.Data;
using MenuMethods;
using System.Threading;
using System.Linq;

namespace Administrator
{
    public class Repository
    {
        private string ConnectionString { get; set; }

        private SqlConnection connection { get; }

        public Repository()
        {
            string ConnectionString = "Data Source = sql6009.site4now.net; Initial Catalog = DB_A53DDD_grupp5; User ID = DB_A53DDD_grupp5_admin; Password = grupp5pizza"; //Ändrat till litet g i grupp på databas och användarnamn
            connection = new SqlConnection(ConnectionString);
            connection.Open();
        }

        public static bool logIn()
        {
            // DENNA FUNKTION KAN FLYTTAS TILL MENUMETHODS SENARE OM VI SKAPAR ETT GEMENSAMT KLASSBIBLIOTEK FÖR REPON FÖR ATT CONNECTA TILL DATABAS.
            // DÅ KOMMER OCKSÅ IF-SATSEN SOM KOLLAR HURUVIDA ANVÄNDAREN ÄR ADMIN ATT ÄNDRAS FÖR ATT ÄVEN KUNNA NYTTJA FUNKTIONEN I BAGAR/KASSA-TERMINAL

            Repository rep = new Repository();

            while (true)
            {
                
                Console.Write("AnvändarID: ");
                int id = int.Parse(Console.ReadLine());
                Console.Write("Lösenord: ");
                string password = Console.ReadLine();

                string role = rep.CheckPassword("CheckPassword", "CheckRole", id, password);


                if (role == "Admin")
                {
                    break;
                }

                else
                {
                    Console.Clear();
                    Console.WriteLine("Inloggning misslyckades. Försök igen.");
                    Thread.Sleep(1500);
                    Console.Clear();
                    continue;
                }
            }

            return true;
        }

        public string CheckPassword(string storedProcedureToCheckPass, string storedProcedureToCheckRole, int id, string pass)
        {
            string passCheck = connection.Query<string>(storedProcedureToCheckPass, new { id = id, pass = pass }, commandType: CommandType.StoredProcedure).First();

            string role = "";

            if(passCheck == "true")
            {
                var checkRole = connection.Query<Employees>(storedProcedureToCheckRole, new { id = id }, commandType: CommandType.StoredProcedure);

                role = checkRole.First().Role; 
            }            

            return role;
        }

        public void AddEmployee(string storedProcedureToAddEmployee)
        {
            // FUNKTION MÅSTE FELSÄKRAS

            string password = "", role = "";

            Console.Clear();
            Console.WriteLine("Lägg till anställd");
            Console.WriteLine("--------------------");
            Console.WriteLine();
            Console.Write("Lösenord: ");
            password = Console.ReadLine();
            Console.Write("Arbetsroll: ");
            role = Console.ReadLine();

            connection.Query<Employees>(storedProcedureToAddEmployee, new { Password = password, Role = role }, commandType: CommandType.StoredProcedure);

            Console.Clear();
            Console.WriteLine("Anställd tillagd.");
            Thread.Sleep(1500);
            Console.Clear();
            MenuFuncs.PrintMenu(Menus.menuChoices, "users");

        }
    }
}
