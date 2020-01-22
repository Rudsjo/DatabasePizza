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
        //Employee
        public Task AddEmployee(string PW, string role, string storedProcedure = "AddEmployee");
        public Task UpdateEmployee(Employees employee, string storedProcedure = "UpdateEmployeeByID");
        public Task<IEnumerable<object>> ShowEmployee(string storedProcedure = "ShowEmployees");
        public Task<IEnumerable<object>> ShowSingleEmployee(int ID, string storedProcedure = "ShowSingleEmployee");
        public Task DeleteEmployee(int ID, string storedProcedure = "DeleteEmployeeByID");

        //Pizza
        public Task AddPizza(string storedProcedure = "AddPizza");
        public Task UpdatePizza(string storedProcedure = "UpdatePizzaByID");
        public Task<IEnumerable<object>> ShowPizza(string storedProcedure = "ShowPizzas");
        public Task<IEnumerable<object>> ShowSinglePizza(int ID, string storedProcedure = "ShowSinglePizza");
        public Task DeletePizza(int ID, string storedProcedure = "DeletePizzaByID");

        //Condiment
        public Task AddCondiment(string storedProcedure = "AddCondiment");
        public Task UpdateCondiment(string storedProcedure = "UpdateCondimentByID");
        public Task<IEnumerable<object>> ShowCondiment(string storedProcedure = "ShowCondiments");
        public Task<IEnumerable<object>> ShowSingleCondiment(int ID, string storedProcedure = "ShowSingleCondiment");
        public Task DeleteCondiment(int ID, string storedProcedure = "DeleteCondimentByID");

        //Extra
        public Task AddExtra(string storedProcedure = "AddExtra");
        public Task UpdateExtra(string storedProcedure = "UpdateExtraByID");
        public Task<IEnumerable<object>> ShowExtra(string storedProcedure = "ShowExtras");
        public Task<IEnumerable<object>> ShowSingleExtra(int ID, string storedProcedure = "ShowSingleExtra");
        public Task DeleteExtra(int ID, string storedProcedure = "DeleteExtraByID");

        //Orders
        public Task<IEnumerable<object>> ShowSingleOldOrder(int ID , string storedProcedure = "ShowOrdersByID");
        public Task<IEnumerable<object>> ShowOldOrders(string storedProcedure = "ShowAllOrders");


    }

}
