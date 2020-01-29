using System;
using System.Data;
using System.Data.SqlClient;
using IDatabasePizza;
using PizzaClassLibrary;
using Dapper;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace PostgreSQLRepository
{
    public class PostgreSQL : IDatabase
    {
        public Task AddCondiment(string type, float price, string storedProcedure = "AddCondiment")
        {
            throw new NotImplementedException();
        }

        public Task AddEmployee(string PW, string role, string storedProcedure = "AddEmployee")
        {
            throw new NotImplementedException();
        }

        public Task AddExtra(string type, float price, string storedProcedure = "AddExtra")
        {
            throw new NotImplementedException();
        }

        public Task AddPizza(string type, float price, string pizzabase, List<Condiment> ingredients, string storedProcedure = "AddPizza")
        {
            throw new NotImplementedException();
        }

        public Task<bool> CheckIfCondimentIDExists(int ID, string storedProcedure = "CheckForExistingCondimentID")
        {
            throw new NotImplementedException();
        }

        public Task<bool> CheckIfPizzaIDExists(int ID, string storedProcedure = "CheckForExistingPizzaID")
        {
            throw new NotImplementedException();
        }

        public Task<bool> CheckIfProductIDExists(int ID, string storedProcedure = "CheckForExistingProductID")
        {
            throw new NotImplementedException();
        }

        public Task<bool> CheckIfUserIDExists(int ID, string storedProcedure = "CheckForExistingID")
        {
            throw new NotImplementedException();
        }

        public Task<(bool, string)> CheckUserIdAndPassword(int ID, string password, string storedProcedure = "CheckPassword", string secondStoredProcedure = "CheckRole")
        {
            throw new NotImplementedException();
        }

        public Task DeleteCondiment(int ID, string storedProcedure = "DeleteCondimentByID")
        {
            throw new NotImplementedException();
        }

        public Task DeleteEmployee(int ID, string storedProcedure = "DeleteEmployeeByID")
        {
            throw new NotImplementedException();
        }

        public Task DeleteExtra(int ID, string storedProcedure = "DeleteExtraByID")
        {
            throw new NotImplementedException();
        }

        public Task DeletePizza(int ID, string storedProcedure = "DeletePizzaByID")
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Condiment>> GetIngredientsFromSpecificPizza(int ID, string storedProcedure = "GetIngredientsFromSpecificPizza")
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Condiment>> ShowCondiments(string storedProcedure = "GetAllCondiments")
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Employee>> ShowEmployee(string storedProcedure = "GetAllEmployees")
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Extra>> ShowExtra(string storedProcedure = "GetAllExtras")
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Pizza>> ShowPizza(string storedProcedure = "GetAllPizzas")
        {
            throw new NotImplementedException();
        }

        public Task<Condiment> ShowSingleCondiment(int ID, string storedProcedure = "GetSingleCondiment")
        {
            throw new NotImplementedException();
        }

        public Task<Employee> ShowSingleEmployee(int ID, string storedProcedure = "GetSingleEmployee")
        {
            throw new NotImplementedException();
        }

        public Task<Extra> ShowSingleExtra(int ID, string storedProcedure = "GetSingleExtra")
        {
            throw new NotImplementedException();
        }

        public Task<Pizza> ShowSinglePizza(int ID, string storedProcedure = "GetSpecificPizza")
        {
            throw new NotImplementedException();
        }

        public Task UpdateCondiment(Condiment condiment, string storedProcedure = "UpdateCondimentByID")
        {
            throw new NotImplementedException();
        }

        public Task UpdateEmployee(Employee employee, string storedProcedure = "UpdateEmployeeByID")
        {
            throw new NotImplementedException();
        }

        public Task UpdateExtra(Extra extra, string storedProcedure = "UpdateExtraByID")
        {
            throw new NotImplementedException();
        }

        public Task UpdatePizza(Pizza pizza, string storedProcedure = "UpdatePizzaByID")
        {
            throw new NotImplementedException();
        }
    }
}
