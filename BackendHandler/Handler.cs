﻿using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Dapper;
using System.Data.SqlClient;
using System.Linq;
using Newtonsoft.Json;
using System;
using System.Dynamic;
using System.Reflection;

namespace BackendHandler
{
    public static class Helpers
    {
        /// <summary>
        /// Supported backends
        ///  : MSSQL
        ///  : PostgreSQL
        /// </summary>
        /// <param name="BackendName"></param>
        /// <returns>if Interface wa not found, return MSSQL as standard</returns>
        public static IDatabase GetSelectedBackend(string BackendName)
        {
            dynamic result = null;
            foreach(Type type in Assembly.LoadFrom(Assembly.GetExecutingAssembly().GetName().Name).GetTypes())
                if(type.IsClass && type.Name.Equals(BackendName))
                {
                    result = Activator.CreateInstance(type); // Create new instance on the fly
                    IDatabase test = result as IDatabase;    // Used to check if class implements interface
                    if(test != null)                         // Convertsion was successful
                        return result;
                }
            return new MSSQL();
        }

        public static List<Pizza> LoadPizzasAsList(IDatabase rep)
        {
            IEnumerable<Pizza> res = rep.GetAllPizzas().Result.ToList();
            foreach (Pizza p in res)
                p.PizzaIngredients = rep.GetIngredientsFromSpecificPizza(p.PizzaID).Result.ToList();
            return res.ToList();
        }
    }

    #region BackendClasses
    public class Condiment
    {
        //Överensstämmer med databasen
        public int CondimentID { get; }
        public string Type { get; set; }
        public float Price { get; set; }


        //Ska vi ha ovverride till alla klasser?
        public override string ToString()
        {
            return $"{this.CondimentID} {this.Type} {this.Price}";
        }
    }
    public class Employee 
    {
        //Överensstämmer med databasen
        public int UserID { get; }
        public string Password { get; set; }
        public string Role { get; set; }

        public override string ToString()
        {
            return $"{this.UserID} {this.Role}";
        }
    }
    public class Extra    
    {
        //Överensstämmer med databasen
        public int ProductID { get; }
        public string Type { get; set; }
        public float Price { get; set; }

        //Ska vi ha ovverride till alla klasser?
        public override string ToString()
        {
            return $"{this.ProductID} {this.Type} {this.Price}";
        }
    }
    public class Order    
    {
        //Överensstämmer med databasen
        public int OrderID { get; }
        public float Price { get; set; }
        public int Status { get; set; }
        public List<Extra> ExtraList { get; set; }
        public List<Pizza> PizzaList { get; set; }
        public override string ToString()
        {
            return $"ID: {this.OrderID}\nTillbehör: {this.ExtraList}\nPizzor: {this.PizzaList}\nPris: {this.Price}";
        }
    }
    public class Pizza    
    {
        //Överensstämmer med databasen
        public int PizzaID { get; }
        public string Type { get; set; }
        public float Price { get; set; }
        public int PizzabaseID { get; set; }
        public List<Condiment> PizzaIngredients { get; set; }


        //Ska vi ha ovverride till alla klasser?
        public override string ToString()
        {
            return $"{this.PizzaID} {this.Type} {this.Price}";
        }
    }
    #endregion

    /// <summary>
    /// Backend interface
    /// </summary>
    public interface IDatabase 
    {
        //De funktioner som bara skall visa innehåll har en ToString() override som stringar ex. type,role,price,id
        //Employee
        public Task AddEmployee(Employee emp, string storedProcedure = "AddEmployee");
        public Task UpdateEmployee(Employee emp, string storedProcedure = "UpdateEmployeeByID");
        public Task<IEnumerable<Employee>> GetAllEmployees(string storedProcedure = "GetAllEmployees");
        public Task<Employee> GetSingleEmployee(int ID, string storedProcedure = "GetSingleEmployee");
        public Task DeleteEmployee(Employee emp, string storedProcedure = "DeleteEmployeeByID");

        //Pizza
        public Task<Pizza> AddPizza(Pizza pizza, string storedProcedure = "AddPizza");
        public Task UpdatePizza(Pizza pizza, string storedProcedure = "UpdatePizzaByID");
        public Task<IEnumerable<Pizza>> GetAllPizzas(string storedProcedure = "GetAllPizzas");
        public Task<Pizza> GetSinglePizza(int ID, string storedProcedure = "GetSpecificPizza");
        //public Task<Pizza> GetSinglePizza(string PizzaName, string storedProcedure = "GetSpecificPizza");
        public Task DeletePizza(Pizza pizza, string storedProcedure = "DeletePizzaByID");
        public Task<IEnumerable<Condiment>> GetIngredientsFromSpecificPizza(int ID, string storedProcedure = "GetIngredientsFromSpecificPizza");
        public Task AddCondimentToPizza(Pizza pizza, string storedProcedureToAddCondimentToPizza = "AddStandardCondimentToPizza");

        //Condiment
        public Task AddCondiment(Condiment cond, string storedProcedure = "AddCondiment");
        public Task UpdateCondiment(Condiment cond, string storedProcedure = "UpdateCondimentByID");
        public Task<IEnumerable<Condiment>> GetAllCondiments(string storedProcedure = "GetAllCondiments");
        public Task<Condiment> GetSingleCondiment(int ID, string storedProcedure = "GetSingleCondiment");
        public Task DeleteCondiment(Condiment cond, string storedProcedure = "DeleteCondimentByID");

        public Task DeleteCondimentFromPizza(Pizza pizza, Condiment condiment, string storedProcedure = "DeleteCondimentFromPizza");

        //Extra
        public Task AddExtra(Extra extra, string storedProcedure = "AddExtra");
        public Task UpdateExtra(Extra extra, string storedProcedure = "UpdateExtraByID");
        public Task<IEnumerable<Extra>> GetAllExtras(string storedProcedure = "GetAllExtras");
        public Task<Extra> GetSingleExtra(int ID, string storedProcedure = "GetSingleExtra");
        public Task DeleteExtra(Extra extra, string storedProcedure = "DeleteExtraByID");


        //Functions for checking user/password and for checking if an ID exists in the database.
        public Task<(bool, string)> CheckUserIdAndPassword(int ID, string password, string storedProcedure = "CheckPassword", string secondStoredProcedure = "CheckRole");
        public Task<bool> CheckIfUserIDExists(int ID, string storedProcedure = "CheckForExistingID");
        public Task<bool> CheckIfProductIDExists(int ID, string storedProcedure = "CheckForExistingProductID");
        public Task<bool> CheckIfCondimentIDExists(int ID, string storedProcedure = "CheckForExistingCondimentID");
        public Task<bool> CheckIfPizzaIDExists(int ID, string storedProcedure = "CheckForExistingPizzaID");

        //Order interfaceimplementation måste göras. 
        public Task<IEnumerable<Order>> GetAllOrders(string storedProcedureToShowOrders = "GetAllOrders");
        public Task AddOrder(Order order, string storedProcedureToAddOrder = "AddOrder");

        public Task<IDbTransaction> Transaction();
    }

    #region Backends
    public class MSSQL      : IDatabase
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
        #region Employees
        public async Task AddEmployee(Employee emp, string storedProcedureToAddEmployee = "AddEmployee")
        {
            MSSQL rep = new MSSQL();
            using (rep.connection) { await connection.QueryAsync<Employee>(storedProcedureToAddEmployee, new { Password = emp.Password, Role = emp.Role }, commandType: CommandType.StoredProcedure); }
        }
        public async Task<IEnumerable<Employee>> GetAllEmployees(string storedProcedureToShowEmployees = "GetAllEmployees")
        {
            MSSQL rep = new MSSQL();
            using (rep.connection) { return (await connection.QueryAsync<Employee>(storedProcedureToShowEmployees, commandType: CommandType.StoredProcedure)); }
        }
        public async Task<Employee> GetSingleEmployee(int id, string storedProcedureToShowSingleEmployee = "GetSingleEmployee")
        {
            MSSQL rep = new MSSQL();
            using (rep.connection) { return (await connection.QueryAsync<Employee>(storedProcedureToShowSingleEmployee, new { UserID = id }, commandType: CommandType.StoredProcedure)).First(); }
        }
        public async Task UpdateEmployee(Employee emp, string storedProcedureToUpdateEmployee = "UpdateEmployeeByID")
        {
            MSSQL rep = new MSSQL();
            using (rep.connection) { await connection.QueryAsync<Employee>(storedProcedureToUpdateEmployee, new { UserID = emp.UserID, Password = emp.Password, Role = emp.Role }, commandType: CommandType.StoredProcedure); }
        }
        public async Task DeleteEmployee(Employee emp, string storedProcedureToDeleteEmployee = "DeleteEmployeeByID")
        {
            MSSQL rep = new MSSQL();
            using (rep.connection) { await connection.QueryAsync<Employee>(storedProcedureToDeleteEmployee, new { UserID = emp.UserID }, commandType: CommandType.StoredProcedure); }
        }
        #endregion
        //Pizzas
        #region Pizzas
        public async Task<Pizza> AddPizza(Pizza pizza, string storedProcedureToAddPizza = "AddPizza")
        {
            MSSQL rep = new MSSQL();
            using (rep.connection) { return (await connection.QueryAsync<Pizza>(storedProcedureToAddPizza, new { Type = pizza.Type, Price = pizza.Price, PizzabaseID = pizza.PizzabaseID }, commandType: CommandType.StoredProcedure)).First(); }
        }

        public async Task AddCondimentToPizza(Pizza pizza, string storedProcedureToAddCondimentToPizza = "AddStandardCondimentToPizza")
        {
            MSSQL rep = new MSSQL();
            using (rep.connection)
            {
                foreach (var item in pizza.PizzaIngredients)
                {
                    await connection.QueryAsync(storedProcedureToAddCondimentToPizza, new { CondimentID = item.CondimentID, PizzaID = pizza.PizzaID }, commandType: CommandType.StoredProcedure);
                }
            }
        }
        
        public async Task<IEnumerable<Pizza>> GetAllPizzas(string storedProcedureToShowPizzas = "GetAllPizzas")
        {
            MSSQL rep = new MSSQL();
            using (rep.connection) { return (await connection.QueryAsync<Pizza>(storedProcedureToShowPizzas, commandType: CommandType.StoredProcedure)); }
        }

        public async Task<Pizza> GetSinglePizza(int id, string storedProcedureToShowSinglePizza = "GetSpecificPizza")
        {
            MSSQL rep = new MSSQL();
            using (rep.connection) { return (await connection.QueryAsync<Pizza>(storedProcedureToShowSinglePizza, new { PizzaID = id }, commandType: CommandType.StoredProcedure)).First(); }
        }

        public async Task UpdatePizza(Pizza pizza, string storedProcedureToUpdatePizza = "UpdatePizzaByID")
        {
            MSSQL rep = new MSSQL();
            using (rep.connection) { await connection.QueryAsync<Pizza>(storedProcedureToUpdatePizza, new { PizzaID = pizza.PizzaID, Type = pizza.Type, Price = pizza.Price, PizzabaseID = pizza.PizzabaseID }, commandType: CommandType.StoredProcedure); }
        } 

        public async Task DeletePizza(Pizza pizza, string storedProcedureToDeletePizza = "DeletePizzaByID")
        {
            MSSQL rep = new MSSQL();
            using (rep.connection) { await connection.QueryAsync<Pizza>(storedProcedureToDeletePizza, new { PizzaID = pizza.PizzaID }, commandType: CommandType.StoredProcedure); }
        }
        public async Task<IEnumerable<Condiment>> GetIngredientsFromSpecificPizza(int id, string storedProcedureToGetIngredientFromSpecifikPizza = "GetIngredientsFromSpecificPizza")
        {
            MSSQL rep = new MSSQL();
            using (rep.connection) { return (await connection.QueryAsync<Condiment>(storedProcedureToGetIngredientFromSpecifikPizza, new { PizzaID = id }, commandType: CommandType.StoredProcedure)); }
        }
        #endregion

        #region Condiments
        public async Task AddCondiment(Condiment cond, string storedProcedureToAddCondiment = "AddCondiment")
        {
            MSSQL rep = new MSSQL();
            using (rep.connection) { await connection.QueryAsync<Condiment>(storedProcedureToAddCondiment, new { Type = cond.Type, Price = cond.Price }, commandType: CommandType.StoredProcedure); }
        } 

        public async Task<IEnumerable<Condiment>> GetAllCondiments(string storedProcedureToShowCondiment = "GetAllCondiments")
        {
            MSSQL rep = new MSSQL();
            using (rep.connection) { return (await connection.QueryAsync<Condiment>(storedProcedureToShowCondiment, commandType: CommandType.StoredProcedure)); }
        }

        public async Task<Condiment> GetSingleCondiment(int id, string storedProcedureToShowSingleCondiment = "GetSingleCondiment")
        {
            MSSQL rep = new MSSQL();
            using (rep.connection) { return (await connection.QueryAsync<Condiment>(storedProcedureToShowSingleCondiment, new { CondimentID = id }, commandType: CommandType.StoredProcedure)).First(); }
        }

        public async Task UpdateCondiment(Condiment cond, string storedProcedureToUpdateCondiment = "UpdateCondimentByID")
        {
            MSSQL rep = new MSSQL();
            using (rep.connection) { await connection.QueryAsync<Condiment>(storedProcedureToUpdateCondiment, new { CondimentID = cond.CondimentID, Type = cond.Type, Price = cond.Price, }, commandType: CommandType.StoredProcedure); }
        }

        public async Task DeleteCondiment(Condiment cond, string storedProcedureToDeleteCondiment = "DeleteCondimentByID")
        {
            MSSQL rep = new MSSQL();
            using (rep.connection) { await connection.QueryAsync<Condiment>(storedProcedureToDeleteCondiment, new { CondimentID = cond.CondimentID }, commandType: CommandType.StoredProcedure); }
        }
        #endregion

        #region Extras
        public async Task AddExtra(Extra extra, string storedProcedureToAddExtra = "AddExtra")
        {
            MSSQL rep = new MSSQL();
            using (rep.connection) { await connection.QueryAsync<Extra>(storedProcedureToAddExtra, new { Type = extra.Type, Price = extra.Price }, commandType: CommandType.StoredProcedure); }
        } 

        public async Task<IEnumerable<Extra>> GetAllExtras(string storedProcedureToShowExtra = "GetAllExtras")
        {
            MSSQL rep = new MSSQL();
            using (rep.connection) { return (await connection.QueryAsync<Extra>(storedProcedureToShowExtra, commandType: CommandType.StoredProcedure)); }
        }

        public async Task<Extra> GetSingleExtra(int id, string storedProcedureToShowSingleExtra = "GetSingleExtra")
        {
            MSSQL rep = new MSSQL();
            using (rep.connection) { return (await connection.QueryAsync<Extra>(storedProcedureToShowSingleExtra, new { ProductID = id }, commandType: CommandType.StoredProcedure)).First(); }
        } 

        public async Task UpdateExtra(Extra extra, string storedProcedureToUpdateExtra = "UpdateExtraByID")
        {
            MSSQL rep = new MSSQL();
            using (rep.connection) { await connection.QueryAsync<Extra>(storedProcedureToUpdateExtra, new { ProductID = extra.ProductID, Type = extra.Type, Price = extra.Price, }, commandType: CommandType.StoredProcedure); }
        } 

        public async Task DeleteExtra(Extra extra, string storedProcedureToDeleteExtra = "DeleteExtraByID")
        {
            MSSQL rep = new MSSQL();
            using (rep.connection) { await connection.QueryAsync<Extra>(storedProcedureToDeleteExtra, new { ProductID = extra.ProductID }, commandType: CommandType.StoredProcedure); }
        }
        #endregion
        //Checking functions for existing ID and user/password.
        #region CheckingID functions
        public async Task<(bool, string)> CheckUserIdAndPassword(int ID, string password, string storedProcedureToCheckLogin = "CheckPassword", string storedProcedureToCheckRole = "CheckRole")
        {
            MSSQL rep = new MSSQL();
            using (rep.connection)
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
        }

        public async Task<bool> CheckIfUserIDExists(int ID, string storedProcedureToCheckForExistingID = "CheckForExistingID")
        {
            MSSQL rep = new MSSQL();
            using (rep.connection)
            {
                bool IDExists = (await connection.QueryAsync<bool>(storedProcedureToCheckForExistingID, new { UserID = ID }, commandType: CommandType.StoredProcedure)).First();
                if (IDExists == true)
                    return true;
                else
                    return false;
            }
        }

        public async Task<bool> CheckIfProductIDExists(int ID, string storedProcedureToCheckForExistingProductID = "CheckForExistingProductID")
        {
            MSSQL rep = new MSSQL();
            using (rep.connection)
            {
                bool IDExists = (await connection.QueryAsync<bool>(storedProcedureToCheckForExistingProductID, new { ProductID = ID }, commandType: CommandType.StoredProcedure)).First();
                if (IDExists == true)
                    return true;
                else
                    return false;
            }
        }

        public async Task<bool> CheckIfCondimentIDExists(int ID, string storedProcedureToCheckForExistingCondimentID = "CheckForExistingCondimentID")
        {
            MSSQL rep = new MSSQL();
            using (rep.connection)
            {
                bool IDExists = (await connection.QueryAsync<bool>(storedProcedureToCheckForExistingCondimentID, new { CondimentID = ID }, commandType: CommandType.StoredProcedure)).First();
                if (IDExists == true)
                    return true;
                else
                    return false;
            }
        }

        public async Task<bool> CheckIfPizzaIDExists(int ID, string storedProcedureToCheckForExistingPizzaID = "CheckForExistingPizzaID")
        {
            MSSQL rep = new MSSQL();
            using (rep.connection)
            {
                bool IDExists = (await connection.QueryAsync<bool>(storedProcedureToCheckForExistingPizzaID, new { PizzaID = ID }, commandType: CommandType.StoredProcedure)).First();
                if (IDExists == true)
                    return true;
                else
                    return false;
            }
        }
        #endregion
        //Orders Måste fixas. Både interface och funktioner
        #region Orders
        public async Task<IEnumerable<Order>> GetAllOrders(string storedProcedureToShowOrders = "GetAllOrders")
        {
            MSSQL rep = new MSSQL();
            using (rep.connection) { return (await connection.QueryAsync<Order>(storedProcedureToShowOrders, commandType: CommandType.StoredProcedure)); }
        }
        public async Task AddOrder(Order order, string storedProcedureToAddOrder = "AddOrder")
        {
            MSSQL rep = new MSSQL();
            using (rep.connection) { await connection.QueryAsync<Order>(storedProcedureToAddOrder, new { pizzas = order.PizzaList, extras = order.ExtraList, price = order.Price }, commandType: CommandType.StoredProcedure); }
        }
        #endregion

        public async Task<IDbTransaction> Transaction()
        {
            MSSQL rep = new MSSQL();
            using (rep.connection)
            {
                return await connection.BeginTransactionAsync();
            }
        }

        public async Task DeleteCondimentFromPizza(Pizza pizza, Condiment condiment, string storedProcedure = "DeleteCondimentFromPizza")
        {
            MSSQL rep = new MSSQL();
            using (rep.connection) { await connection.QueryAsync<Pizza>(storedProcedure, new { PizzaID = pizza.PizzaID, CondimentID = condiment.CondimentID }, commandType: CommandType.StoredProcedure); }
        }
    }
    public class PostgreSQL : IDatabase
    {
        public Task AddCondiment(Condiment cond, string storedProcedure = "AddCondiment")
        {
            throw new NotImplementedException();
        }

        public Task AddEmployee(Employee emp, string storedProcedure = "AddEmployee")
        {
            throw new NotImplementedException();
        }

        public Task AddExtra(Extra extra, string storedProcedure = "AddExtra")
        {
            throw new NotImplementedException();
        }

        public Task<Pizza> AddPizza(Pizza pizza, string storedProcedure = "AddPizza")
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

        public Task DeleteCondiment(Condiment cond, string storedProcedure = "DeleteCondimentByID")
        {
            throw new NotImplementedException();
        }

        public Task DeleteEmployee(Employee emp, string storedProcedure = "DeleteEmployeeByID")
        {
            throw new NotImplementedException();
        }

        public Task DeleteExtra(Extra extra, string storedProcedure = "DeleteExtraByID")
        {
            throw new NotImplementedException();
        }

        public Task DeletePizza(Pizza pizza, string storedProcedure = "DeletePizzaByID")
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Condiment>> GetIngredientsFromSpecificPizza(int ID, string storedProcedure = "GetIngredientsFromSpecificPizza")
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Condiment>> GetAllCondiments(string storedProcedure = "GetAllCondiments")
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Employee>> GetAllEmployees(string storedProcedure = "GetAllEmployees")
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Extra>> GetAllExtras(string storedProcedure = "GetAllExtras")
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Pizza>> GetAllPizzas(string storedProcedure = "GetAllPizzas")
        {
            throw new NotImplementedException();
        }

        public Task<Condiment> GetSingleCondiment(int ID, string storedProcedure = "GetSingleCondiment")
        {
            throw new NotImplementedException();
        }

        public Task<Employee> GetSingleEmployee(int ID, string storedProcedure = "GetSingleEmployee")
        {
            throw new NotImplementedException();
        }

        public Task<Extra> GetSingleExtra(int ID, string storedProcedure = "GetSingleExtra")
        {
            throw new NotImplementedException();
        }

        public Task<Pizza> GetSinglePizza(int ID, string storedProcedure = "GetSpecificPizza")
        {
            throw new NotImplementedException();
        }
        //public Task<Pizza> GetSinglePizza(string PizzaName, string storedProcedure = "GetSpecificPizza")
        //{
        //    throw new NotImplementedException();
        //}

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

        public Task AddCondimentToPizza(Pizza pizza, string storedProcedureToAddCondimentToPizza = "AddStandardCondimentToPizza")
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Order>> GetAllOrders(string storedProcedureToShowExtra = "GetAllOrders")
        {
            throw new NotImplementedException();
        }

        public Task AddOrder(Order order, string storedProcedureToAddOrder = "AddOrder")
        {
            throw new NotImplementedException();
        }

        public Task<IDbTransaction> Transaction()
        {
            throw new NotImplementedException();
        }

        public Task DeleteCondimentFromPizza(Pizza pizza, Condiment condiment, string storedProcedure = "DeleteCondimentFromPizza")
        {
            throw new NotImplementedException();
        }
    }
    #endregion

}
