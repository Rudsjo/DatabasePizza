using IDatabasePizza;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
namespace IDatabasePizza
{
    public interface IDatabase
    {
        //Employee
        public Task AddEmployee(string storedProcedure = "AddEmployee");
        public Task UpdateEmployee(string storedProcedure = "UpdateEmployeeByID");
        public Task<IEnumerable<object>> ShowEmployee(string storedProcedure = "ShowEmployees");
        public Task<IEnumerable<object>> ShowSingleEmployee(int ID, string storedProcedure = "ShowSingleEmployee");
        public Task DeleteEmployee(int ID, string storedProcedure = "DeleteEmployeeByID");

        //Pizza
        public Task AddPizza(string storedProcedure = "AddPizza");
        public Task UpdatePizza(string storedProcedure = "UpdatePizzaByID");
        public Task<IEnumerable<object>> ShowPizza(string storedProcedure = "ShowPizzas");
        public Task DeletePizza(int ID, string storedProcedure = "DeletePizzaByID");

        //Condiment
        public Task AddCondiment(string storedProcedure = "AddCondiment");
        public Task UpdateCondiment(string storedProcedure = "UpdateCondimentByID");
        public Task<IEnumerable<object>> ShowCondiment(string storedProcedure = "ShowCondiments");
        public Task DeleteCondiment(int ID, string storedProcedure = "DeleteCondimentByID");

        //Extra
        public Task AddExtra(string storedProcedure = "AddExtra");
        public Task UpdateExtra(string storedProcedure = "UpdateExtraByID");
        public Task<IEnumerable<object>> ShowExtra(string storedProcedure = "ShowExtras");
        public Task DeleteExtra(int ID, string storedProcedure = "DeleteExtraByID");

        //Orders
        public Task<IEnumerable<object>> ShowOrdersByID(int ID , string storedProcedure = "ShowOrdersByID");
        public Task<IEnumerable<object>> ShowOrders(string storedProcedure = "ShowAllOrders");


    }

}
