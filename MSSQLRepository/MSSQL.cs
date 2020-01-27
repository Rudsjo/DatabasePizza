using System;
using System.Data;
using System.Data.SqlClient;
using IDatabasePizza;
using PizzaClassLibrary;
using Dapper;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace MSSQLRepository
{
    public class MSSQL : IDatabase
    {
        private string ConnectionString { get; set; }

        private SqlConnection connection { get; }

        public MSSQL()
        {
            string ConnectionString = "Data Source = sql6009.site4now.net; Initial Catalog = DB_A53DDD_grupp5; User ID = DB_A53DDD_grupp5_admin; Password = grupp5pizza"; //Ändrat till litet g i grupp på databas och användarnamn
            connection = new SqlConnection(ConnectionString);
            connection.Open();
        }

        //Employees
        
        public async Task AddEmployee(string password, string role, string storedProcedureToAddEmployee = "AddEmployee")
        {
            await connection.QueryAsync<Employee>(storedProcedureToAddEmployee, new { Password = password, Role = role }, commandType: CommandType.StoredProcedure);
        }

        public async Task<IEnumerable<Employee>> ShowEmployee(string storedProcedureToShowEmployees = "ShowEmployees")
        {
            //IEnumerable<Employees> employees = (await connection.QueryAsync<Employees>(storedProcedureToShowEmployees, commandType: CommandType.StoredProcedure));
            //return employees;
            return (await connection.QueryAsync<Employee>(storedProcedureToShowEmployees, commandType: CommandType.StoredProcedure));
        }

        public async Task<Employee> ShowSingleEmployee(int id, string storedProcedureToShowSingleEmployee = "ShowSingleEmployee")
        {
            return (await connection.QueryAsync<Employee>(storedProcedureToShowSingleEmployee, new { UserID = id }, commandType: CommandType.StoredProcedure)).First();
        }

        public async Task UpdateEmployee(Employee employee, string storedProcedureToUpdateEmployee = "UpdateEmployeeByID")
        {
            await connection.QueryAsync<Employee>(storedProcedureToUpdateEmployee, new { UserID = employee.UserID, Password = employee.Password, Role = employee.Role }, commandType: CommandType.StoredProcedure);
        }

        public async Task DeleteEmployee(int id, string storedProcedureToDeleteEmployee = "DeleteEmployeeByID")
        {
            await connection.QueryAsync<Employee>(storedProcedureToDeleteEmployee, new { UserID = id }, commandType: CommandType.StoredProcedure);
        }
    
        //Pizzas

        public async Task AddPizza(string storedProcedureToAddPizza, float price, string type, string pizzabase, string ingredients)
        {
            await connection.QueryAsync<Pizza>(storedProcedureToAddPizza, new { Type = type, Price = price, Base = pizzabase, Ingredients = ingredients }, commandType: CommandType.StoredProcedure);
        }

        public async Task<IEnumerable<Pizza>> ShowPizza(string storedProcedureToShowPizzas)
        {
            return (await connection.QueryAsync<Pizza>(storedProcedureToShowPizzas, commandType: CommandType.StoredProcedure));
        }

        public async Task<Pizza> ShowSinglePizza(int id, string storedProcedureToShowSinglePizza)
        {
            return (await connection.QueryAsync<Pizza>(storedProcedureToShowSinglePizza, new { PizzaID = id }, commandType: CommandType.StoredProcedure)).First();
        }

        public async Task UpdatePizza(Pizza pizza, string storedProcedureToUpdatePizza)
        {
            await connection.QueryAsync<Pizza>(storedProcedureToUpdatePizza, new { PizzaID = pizza.PizzaID, Type = pizza.Type ,Price = pizza.Price, Base = pizza.Base, Ingredients = pizza.Ingredients}, commandType: CommandType.StoredProcedure);
        }

        public async Task DeletePizza(int id, string storedProcedureToDeletePizza)
        {
            await connection.QueryAsync<Pizza>(storedProcedureToDeletePizza, new { PizzaID = id }, commandType: CommandType.StoredProcedure);
        }

        //Condiments
        public async Task AddCondiment(string storedProcedureToAddCondiment, float price, string type)
        {
            await connection.QueryAsync<Condiment>(storedProcedureToAddCondiment, new { Type = type, Price = price }, commandType: CommandType.StoredProcedure);
        }

        public async Task<IEnumerable<Condiment>> ShowCondiments(string storedProcedureToShowCondiment = "ShowCondiments")
        {
            return (await connection.QueryAsync<Condiment>(storedProcedureToShowCondiment, commandType: CommandType.StoredProcedure));
        }

        public async Task<Condiment> ShowSingleCondiment(int id, string storedProcedureToShowSingleCondiment)
        {
            return (await connection.QueryAsync<Condiment>(storedProcedureToShowSingleCondiment, new { CondimentID = id }, commandType: CommandType.StoredProcedure)).First();
        }

        public async Task UpdateCondiment(Condiment condiment, string storedProcedureToUpdateCondiment)
        {
            await connection.QueryAsync<Condiment>(storedProcedureToUpdateCondiment, new { CondimentID = condiment.CondimentID, Type = condiment.Type, Price = condiment.Price, }, commandType: CommandType.StoredProcedure);
        }

        public async Task DeleteCondiment(int id, string storedProcedureToDeleteCondiment = "DeleteCondiment")
        {
            await connection.QueryAsync<Condiment>(storedProcedureToDeleteCondiment, new { CondimentID = id }, commandType: CommandType.StoredProcedure);
        }

        //Extras
        public async Task AddExtra(string storedProcedureToAddExtra, float price, string type)
        {
            await connection.QueryAsync<Extra>(storedProcedureToAddExtra, new { Type = type, Price = price }, commandType: CommandType.StoredProcedure);
        }

        public async Task<IEnumerable<Extra>> ShowExtra(string storedProcedureToShowExtra)
        {
            return (await connection.QueryAsync<Extra>(storedProcedureToShowExtra, commandType: CommandType.StoredProcedure));
        }

        public async Task<Extra> ShowSingleExtra(int id, string storedProcedureToShowSingleExtra)
        {
            return (await connection.QueryAsync<Extra>(storedProcedureToShowSingleExtra, new { ProductID = id }, commandType: CommandType.StoredProcedure)).First();
        }

        public async Task UpdateExtra(Extra extra, string storedProcedureToUpdateExtra)
        {
            await connection.QueryAsync<Extra>(storedProcedureToUpdateExtra, new { ProductID = extra.ProductID, Type = extra.Type, Price = extra.Price, }, commandType: CommandType.StoredProcedure);
        }

        public async Task DeleteExtra(int id, string storedProcedureToDeleteExtra)
        {
            await connection.QueryAsync<Extra>(storedProcedureToDeleteExtra, new { ProductID = id }, commandType: CommandType.StoredProcedure);
        }

        //OldOrders
        public async Task<OldOrder> ShowSingleOldOrder(int id, string storedProcedureToShowSingleOldOrder)
        {
            return (await connection.QueryAsync<OldOrder>(storedProcedureToShowSingleOldOrder, new { OldOrderID = id }, commandType: CommandType.StoredProcedure)).First();
        }

        public async Task<IEnumerable<OldOrder>> ShowOldOrders(string storedProcedureToShowOldOrders)
        {
            return (await connection.QueryAsync<OldOrder>(storedProcedureToShowOldOrders, commandType: CommandType.StoredProcedure));
        }

        //Checking functions for existing ID and user/password.

        public async Task<(bool, string)> CheckUserIdAndPassword(int ID, string password, string storedProcedureToCheckLogin = "CheckPassword", string storedProcedureToCheckRole = "CheckRole")
        {
            string passCheck = (await connection.QueryAsync<string>(storedProcedureToCheckLogin, new { id = ID, pass = password }, commandType: CommandType.StoredProcedure)).First();
            bool correctLogInCredentials = false;
            string role = "";
            
            if (passCheck == "true")
            {
                correctLogInCredentials = true;
                var checkRole = await connection.QueryAsync<Employee>(storedProcedureToCheckRole, new { id = ID }, commandType: CommandType.StoredProcedure);
                role = checkRole.First().Role;
            }

            return (correctLogInCredentials, role);
        }

        public async Task<bool> CheckIfUserIDExists(int ID, string storedProcedureToCheckForExistingID = "CheckForExistingID")
        {
            bool IDExists = (await connection.QueryAsync<bool>(storedProcedureToCheckForExistingID, new { UserID = ID }, commandType: CommandType.StoredProcedure)).First();
            if (IDExists == true)
                return true;
            else
                return false;
        }

        public async Task<bool> CheckIfProductIDExists(int ID, string storedProcedureToCheckForExistingProductID = "CheckForExistingProductID")
        {
            bool IDExists = (await connection.QueryAsync<bool>(storedProcedureToCheckForExistingProductID, new { ProductID = ID }, commandType: CommandType.StoredProcedure)).First();
            if (IDExists == true)
                return true;
            else
                return false;
        }

        public async Task<bool> CheckIfCondimentIDExists(int ID, string storedProcedureToCheckForExistingCondimentID = "CheckForExistingCondimentID")
        {
            bool IDExists = (await connection.QueryAsync<bool>(storedProcedureToCheckForExistingCondimentID, new { CondimentID = ID }, commandType: CommandType.StoredProcedure)).First();
            if (IDExists == true)
                return true;
            else
                return false;
        }

        public async Task<bool> CheckIfPizzaIDExists(int ID, string storedProcedureToCheckForExistingPizzaID = "CheckForExistingPizzaID")
        {
            bool IDExists = (await connection.QueryAsync<bool>(storedProcedureToCheckForExistingPizzaID, new { PizzaID = ID }, commandType: CommandType.StoredProcedure)).First();
            if (IDExists == true)
                return true;
            else
                return false;
        }

        //Lägga till funtioner för resterande tabeller, för att kolla om ID existerar.

        //Orders Måste fixas. Både interface och funktioner


    }
}
