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
        public Task UpdateEmployee(Employees employee, string storedProcedure = "UpdateEmployeeByID");
        public Task<IEnumerable<Employees>> ShowEmployee(string storedProcedure = "ShowEmployees");
        public Task<Employees> ShowSingleEmployee(int ID, string storedProcedure = "ShowSingleEmployee");
        public Task DeleteEmployee(int ID, string storedProcedure = "DeleteEmployeeByID");

        //Pizza
        public Task AddPizza(string type, float price, string pizzabase, string ingredients, string storedProcedure = "AddPizza");
        public Task UpdatePizza(Pizzas pizza, string storedProcedure = "UpdatePizzaByID");
        public Task<IEnumerable<Pizzas>> ShowPizza(string storedProcedure = "ShowPizzas");
        public Task<Pizzas> ShowSinglePizza(int ID, string storedProcedure = "ShowSinglePizza");
        public Task DeletePizza(int ID, string storedProcedure = "DeletePizzaByID");

        //Condiment
        public Task AddCondiment(string type, float price, string storedProcedure = "AddCondiment");
        public Task UpdateCondiment(Condiments condiment, string storedProcedure = "UpdateCondimentByID");
        public Task<IEnumerable<Condiments>> ShowCondiments(string storedProcedure = "ShowCondiments");
        public Task<Condiments> ShowSingleCondiment(int ID, string storedProcedure = "ShowSingleCondiment");
        public Task DeleteCondiment(int ID, string storedProcedure = "DeleteCondimentByID");

        //Extra
        public Task AddExtra(string type, float price, string storedProcedure = "AddExtra");
        public Task UpdateExtra(Extras extra, string storedProcedure = "UpdateExtraByID");
        public Task<IEnumerable<Extras>> ShowExtra(string storedProcedure = "ShowExtras");
        public Task<Extras> ShowSingleExtra(int ID, string storedProcedure = "ShowSingleExtra");
        public Task DeleteExtra(int ID, string storedProcedure = "DeleteExtraByID");

        //OldOrders
        public Task<OldOrders> ShowSingleOldOrder(int ID , string storedProcedure = "ShowOldOrderByID");
        public Task<IEnumerable<OldOrders>> ShowOldOrders(string storedProcedure = "ShowOldOrders");

        public Task<(bool, string)> CheckUserIdAndPassword(int ID, string password, string storedProcedure = "CheckPassword", string secondStoredProcedure = "CheckRole");

        //Order interfaceimplementation måste göras. 
    }

}
