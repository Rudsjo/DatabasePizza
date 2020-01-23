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
        
        public async Task AddEmployee(string storedProcedureToAddEmployee, string password, string role)
        {
            await connection.QueryAsync<Employees>(storedProcedureToAddEmployee, new { Password = password, Role = role }, commandType: CommandType.StoredProcedure);
        }

        public async Task<IEnumerable<Employees>> ShowEmployee(string storedProcedureToShowEmployees)
        {
            //IEnumerable<Employees> employees = (await connection.QueryAsync<Employees>(storedProcedureToShowEmployees, commandType: CommandType.StoredProcedure));
            //return employees;
            return (await connection.QueryAsync<Employees>(storedProcedureToShowEmployees, commandType: CommandType.StoredProcedure));
        }

        public async Task<Employees> ShowSingleEmployee(int id, string storedProcedureToShowSingleEmployee)
        {
            return (await connection.QueryAsync<Employees>(storedProcedureToShowSingleEmployee, new { UserID = id }, commandType: CommandType.StoredProcedure)).First();
        }

        public async Task UpdateEmployee(Employees employee, string storedProcedureToUpdateEmployee)
        {
            await connection.QueryAsync<Employees>(storedProcedureToUpdateEmployee, new { UserID = employee.UserID, Password = employee.Password, Role = employee.Role }, commandType: CommandType.StoredProcedure);
        }

        public async Task DeleteEmployee(int id, string storedProcedureToDeleteEmployee)
        {
            await connection.QueryAsync<Employees>(storedProcedureToDeleteEmployee, new { UserID = id }, commandType: CommandType.StoredProcedure);
        }

        //Pizzas

        public async Task AddPizza(string storedProcedureToAddPizza, float price, string type, string pizzabase, string ingredients)
        {
            await connection.QueryAsync<Pizzas>(storedProcedureToAddPizza, new { Type = type, Price = price, Base = pizzabase, Ingredients = ingredients }, commandType: CommandType.StoredProcedure);
        }

        public async Task<IEnumerable<Pizzas>> ShowPizza(string storedProcedureToShowPizzas)
        {
            return (await connection.QueryAsync<Pizzas>(storedProcedureToShowPizzas, commandType: CommandType.StoredProcedure));
        }

        public async Task<Pizzas> ShowSinglePizza(int id, string storedProcedureToShowSinglePizza)
        {
            return (await connection.QueryAsync<Pizzas>(storedProcedureToShowSinglePizza, new { PizzaID = id }, commandType: CommandType.StoredProcedure)).First();
        }

        public async Task UpdatePizza(Pizzas pizza, string storedProcedureToUpdatePizza)
        {
            await connection.QueryAsync<Pizzas>(storedProcedureToUpdatePizza, new { PizzaID = pizza.PizzaID, Type = pizza.Type ,Price = pizza.Price, Base = pizza.Base, Ingredients = pizza.Ingredients}, commandType: CommandType.StoredProcedure);
        }

        public async Task DeletePizza(int id, string storedProcedureToDeletePizza)
        {
            await connection.QueryAsync<Pizzas>(storedProcedureToDeletePizza, new { PizzaID = id }, commandType: CommandType.StoredProcedure);
        }

        //Condiments
        public async Task AddCondiment(string storedProcedureToAddCondiment, float price, string type)
        {
            await connection.QueryAsync<Condiments>(storedProcedureToAddCondiment, new { Type = type, Price = price }, commandType: CommandType.StoredProcedure);
        }

        public async Task<IEnumerable<Condiments>> ShowCondiment(string storedProcedureToShowCondiment)
        {
            return (await connection.QueryAsync<Condiments>(storedProcedureToShowCondiment, commandType: CommandType.StoredProcedure));
        }

        public async Task<Condiments> ShowSingleCondiment(int id, string storedProcedureToShowSingleCondiment)
        {
            return (await connection.QueryAsync<Condiments>(storedProcedureToShowSingleCondiment, new { CondimentID = id }, commandType: CommandType.StoredProcedure)).First();
        }

        public async Task UpdateCondiment(Condiments condiment, string storedProcedureToUpdateCondiment)
        {
            await connection.QueryAsync<Condiments>(storedProcedureToUpdateCondiment, new { CondimentID = condiment.CondimentID, Type = condiment.Type, Price = condiment.Price, }, commandType: CommandType.StoredProcedure);
        }

        public async Task DeleteCondiment(int id, string storedProcedureToDeleteCondiment)
        {
            await connection.QueryAsync<Condiments>(storedProcedureToDeleteCondiment, new { CondimentID = id }, commandType: CommandType.StoredProcedure);
        }

        //Extras
        public async Task AddExtra(string storedProcedureToAddExtra, float price, string type)
        {
            await connection.QueryAsync<Extras>(storedProcedureToAddExtra, new { Type = type, Price = price }, commandType: CommandType.StoredProcedure);
        }

        public async Task<IEnumerable<Extras>> ShowExtra(string storedProcedureToShowExtra)
        {
            return (await connection.QueryAsync<Extras>(storedProcedureToShowExtra, commandType: CommandType.StoredProcedure));
        }

        public async Task<Extras> ShowSingleExtra(int id, string storedProcedureToShowSingleExtra)
        {
            return (await connection.QueryAsync<Extras>(storedProcedureToShowSingleExtra, new { ProductID = id }, commandType: CommandType.StoredProcedure)).First();
        }

        public async Task UpdateExtra(Extras extra, string storedProcedureToUpdateExtra)
        {
            await connection.QueryAsync<Extras>(storedProcedureToUpdateExtra, new { ProductID = extra.ProductID, Type = extra.Type, Price = extra.Price, }, commandType: CommandType.StoredProcedure);
        }

        public async Task DeleteExtra(int id, string storedProcedureToDeleteExtra)
        {
            await connection.QueryAsync<Extras>(storedProcedureToDeleteExtra, new { ProductID = id }, commandType: CommandType.StoredProcedure);
        }

        //OldOrders
        public async Task<OldOrders> ShowSingleOldOrder(int id, string storedProcedureToShowSingleOldOrder)
        {
            return (await connection.QueryAsync<OldOrders>(storedProcedureToShowSingleOldOrder, new { OldOrderID = id }, commandType: CommandType.StoredProcedure)).First();
        }

        public async Task<IEnumerable<OldOrders>> ShowOldOrders(string storedProcedureToShowOldOrders)
        {
            return (await connection.QueryAsync<OldOrders>(storedProcedureToShowOldOrders, commandType: CommandType.StoredProcedure));
        }

        //Orders Måste fixas. Både interface och funktioner


    }
}
