using IDatabasePizza;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using PizzaClassLibrary;

namespace IDatabasePizza
{
    public interface IDatabase
    {
        //De funktioner som bara skall visa innehåll har en ToString() override som stringar ex. type,role,price,id
        //Employee
        public Task AddEmployee(string PW, string role, string storedProcedure = "AddEmployee");
        public Task UpdateEmployee(Employee employee, string storedProcedure = "UpdateEmployeeByID");
        public Task<IEnumerable<Employee>> ShowEmployee(string storedProcedure = "GetAllEmployees");
        public Task<Employee> ShowSingleEmployee(int ID, string storedProcedure = "GetSingleEmployee");
        public Task DeleteEmployee(int ID, string storedProcedure = "DeleteEmployeeByID");

        //Pizza
        public Task AddPizza(string type, float price, string pizzabase, List<Condiment> ingredients, string storedProcedure = "AddPizza");
        public Task UpdatePizza(Pizza pizza, string storedProcedure = "UpdatePizzaByID");
        public Task<IEnumerable<Pizza>> ShowPizza(string storedProcedure = "GetAllPizzas");
        public Task<Pizza> ShowSinglePizza(int ID, string storedProcedure = "GetSpecificPizza");
        public Task DeletePizza(int ID, string storedProcedure = "DeletePizzaByID");
        public Task<IEnumerable<Condiment>> GetIngredientsFromSpecificPizza(int ID, string storedProcedure = "GetIngredientsFromSpecificPizza");

        //Condiment
        public Task AddCondiment(string type, float price, string storedProcedure = "AddCondiment");
        public Task UpdateCondiment(Condiment condiment, string storedProcedure = "UpdateCondimentByID");
        public Task<IEnumerable<Condiment>> ShowCondiments(string storedProcedure = "GetAllCondiments");
        public Task<Condiment> ShowSingleCondiment(int ID, string storedProcedure = "GetSingleCondiment");
        public Task DeleteCondiment(int ID, string storedProcedure = "DeleteCondimentByID");

        //Extra
        public Task AddExtra(string type, float price, string storedProcedure = "AddExtra");
        public Task UpdateExtra(Extra extra, string storedProcedure = "UpdateExtraByID");
        public Task<IEnumerable<Extra>> ShowExtra(string storedProcedure = "GetAllExtras");
        public Task<Extra> ShowSingleExtra(int ID, string storedProcedure = "GetSingleExtra");
        public Task DeleteExtra(int ID, string storedProcedure = "DeleteExtraByID");


        //Functions for checking user/password and for checking if an ID exists in the database.
        public Task<(bool, string)> CheckUserIdAndPassword(int ID, string password, string storedProcedure = "CheckPassword", string secondStoredProcedure = "CheckRole");
        public Task<bool> CheckIfUserIDExists(int ID, string storedProcedure = "CheckForExistingID");
        public Task<bool> CheckIfProductIDExists(int ID, string storedProcedure = "CheckForExistingProductID");
        public Task<bool> CheckIfCondimentIDExists(int ID, string storedProcedure = "CheckForExistingCondimentID");
        public Task<bool> CheckIfPizzaIDExists(int ID, string storedProcedure = "CheckForExistingPizzaID");

        //Order interfaceimplementation måste göras. 
    }

}
