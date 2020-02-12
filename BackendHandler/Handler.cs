using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Dapper;
using System.Data.SqlClient;
using System.Linq;
using Newtonsoft.Json;
using System;
using System.Dynamic;
using System.Reflection;
using Npgsql;
using System.IO;

namespace BackendHandler
{
    public static class Helpers
    {
        /// <summary>
        /// Readonly property that gets the configfile path
        /// </summary>
        public static string ConfigfilePath { get; } = "Configfile.cfg";
        public static string[] Existingbackends { get; } = { "MSSQL", "PostgreSQL" };
        public static string StandardBackend { get; } = "MSSQL";


        /// <summary>
        /// Supported backends
        ///  : MSSQL
        ///  : PostgreSQL
        /// </summary>
        /// <param name="BackendName"></param>M
        /// <returns>if Interface wa not found, return MSSQL as standard</returns>
        public static IDatabase GetSelectedBackend()
        {
            dynamic result = null;
            foreach (Type type in Assembly.LoadFrom(Assembly.GetExecutingAssembly().GetName().Name).GetTypes())
                if (type.IsClass && !type.Name.Contains("<>") && type.Name.Equals(GetSelctedBackend()))
                {
                    result = Activator.CreateInstance(type); // Create new instance on the fly
                    IDatabase test = result as IDatabase;    // Used to check if class implements interface
                    if (test != null)                         // Convertsion was successful
                        return result;
                }
            return new PostgreSQL();
            //return new MSSQL();
        }
        public static List<Pizza> LoadPizzasAsList(IDatabase rep)
        {
            IEnumerable<Pizza> res = rep.GetAllPizzas().Result.ToList().OrderBy(p => p.PizzaID);
            foreach (Pizza p in res)
            {
                p.PizzaIngredients = rep.GetIngredientsFromSpecificPizza(p.PizzaID).Result.ToList();
                p.StandardIngredientsDeffinition = p.PizzaIngredients.Select(p => p.CondimentID).AsList();
            }
            return res.ToList();
        }

        private static void ValidateConfigFile()
        {
            if (!File.Exists(ConfigfilePath)) { using (File.Create(ConfigfilePath)) { } }

            if (File.ReadAllLines(ConfigfilePath).Where(s => !s.StartsWith("#") && !String.IsNullOrEmpty(s)).Count() == 0 ||
               File.ReadAllLines(ConfigfilePath).Where(s => !s.StartsWith("#") && !String.IsNullOrEmpty(s)).Count() > 1 ||
               File.ReadAllLines(ConfigfilePath).Where(s => !string.IsNullOrEmpty(s)).Count() != Existingbackends.Count())
            {
                string[] Commented = new string[Existingbackends.Length];
                for (int i = 0; i < Existingbackends.Count(); i++) Commented[i] = (Existingbackends[i].Equals(StandardBackend)) ? Existingbackends[i] : "#" + Existingbackends[i];
                File.WriteAllLines(ConfigfilePath, Commented);
            }
        }

        /// <summary>
        /// Get selected backend
        /// </summary>
        /// <returns>returns backendname</returns>
        private static string GetSelctedBackend()
        {
            ValidateConfigFile();
            return File.ReadAllLines(ConfigfilePath).First(s => !s.StartsWith("#"));
        }

    }

    #region BackendClasses
    public class Condiment
    {
        //Överensstämmer med databasen
        public int CondimentID { get; }
        public string Type { get; set; }
        public float Price { get; set; } // Denna strular mot postgres databas. Functions för add condiment fungerar om man har price som integer


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
        public int OrderID { get; set; }
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
        public string Pizzabase { get; set; }
        public List<int> StandardIngredientsDeffinition { get; set; }
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
        #region//Employee
        public Task AddEmployee(Employee emp, string storedProcedure = "AddEmployee");
        public Task UpdateEmployee(Employee emp, string storedProcedure = "UpdateEmployeeByID");
        public Task<IEnumerable<Employee>> GetAllEmployees(string storedProcedure = "GetAllEmployees");
        public Task<Employee> GetSingleEmployee(int ID, string storedProcedure = "GetSingleEmployee");
        public Task DeleteEmployee(Employee emp, string storedProcedure = "DeleteEmployeeByID");
        #endregion

        #region//Pizza
        public Task<Pizza> AddPizza(Pizza pizza, string storedProcedure = "AddPizza");
        public Task UpdatePizza(Pizza pizza, string storedProcedure = "UpdatePizzaByID");
        public Task<IEnumerable<Pizza>> GetAllPizzas(string storedProcedure = "GetAllPizzas");
        public Task<Pizza> GetSinglePizza(int ID, string storedProcedure = "GetSpecificPizza");
        public Task DeletePizza(Pizza pizza, string storedProcedure = "DeletePizzaByID");
        public Task<IEnumerable<Condiment>> GetIngredientsFromSpecificPizza(int ID, string storedProcedure = "GetIngredientsFromSpecificPizza");
        public Task AddCondimentToPizza(Pizza pizza, string storedProcedureToAddCondimentToPizza = "AddStandardCondimentToPizza");
        public Task<IEnumerable<(int, string)>> GetAllPizzabases(string storedProcedure = "GetAllPizzabases");
        #endregion

        #region//Condiment
        public Task AddCondiment(Condiment cond, string storedProcedure = "AddCondiment");
        public Task UpdateCondiment(Condiment cond, string storedProcedure = "UpdateCondimentByID");
        public Task<IEnumerable<Condiment>> GetAllCondiments(string storedProcedure = "GetAllCondiments");
        public Task<Condiment> GetSingleCondiment(int ID, string storedProcedure = "GetSingleCondiment");
        public Task DeleteCondiment(Condiment cond, string storedProcedure = "DeleteCondimentByID");
        public Task DeleteCondimentFromPizza(Pizza pizza, Condiment condiment, string storedProcedure = "DeleteCondimentFromPizza");
        #endregion

        #region//Extra
        public Task AddExtra(Extra extra, string storedProcedure = "AddExtra");
        public Task UpdateExtra(Extra extra, string storedProcedure = "UpdateExtraByID");
        public Task<IEnumerable<Extra>> GetAllExtras(string storedProcedure = "GetAllExtras");
        public Task<Extra> GetSingleExtra(int ID, string storedProcedure = "GetSingleExtra");
        public Task DeleteExtra(Extra extra, string storedProcedure = "DeleteExtraByID");
        #endregion

        #region//Functions for checking user/password and for checking if an ID exists in the database.
        public Task<(bool, string)> CheckUserIdAndPassword(int ID, string password, string storedProcedure = "CheckPassword", string secondStoredProcedure = "CheckRole");
        public Task<bool> CheckIfUserIDExists(int ID, string storedProcedure = "CheckForExistingID");
        public Task<bool> CheckIfProductIDExists(int ID, string storedProcedure = "CheckForExistingProductID");
        public Task<bool> CheckIfCondimentIDExists(int ID, string storedProcedure = "CheckForExistingCondimentID");
        public Task<bool> CheckIfPizzaIDExists(int ID, string storedProcedure = "CheckForExistingPizzaID");
        #endregion

        #region//Orders
        public Task<IEnumerable<Order>> GetAllOrders(string storedProcedureToShowOrders = "GetAllOrders");
        public Task<int> AddOrder(Order order, string storedProcedureToAddOrder = "AddOrder");
        public Task<IEnumerable<Order>> GetOrderByStatus(int statusID, string storedProcedureToGetOrderByStatus = "GetOrderByStatus");
        public Task UpdateOrderStatusWhenOrderIsServed(Employee emp, Order order, string storedProcedure = "UpdateOrderStatusWhenOrderIsServed");
        public Task UpdateOrderStatusWhenOrderIsCooked(Order order, string storedProcedure = "UpdateOrderStatusWhenOrderIsCooked");
        public Task<Order> GetSingleOrder(int ID, string storedProcedure = "GetSingleOrder");
        public Task DeleteOldOrder(Order order, string storedProcedure = "DeleteOldOrderByID");
        public Task BakerChoosingOrderToCook(Employee employee, Order order, string storedProcedure = "BakerChoosingOrderToCook");
        public Task BakerCancellingCooking(Employee employee, Order order, string storedProcedure = "BakerCancellingCooking");
        #endregion
    }

    #region Backends
    public class MSSQL : IDatabase
    {
        /// <summary>
        /// Vi sätter connectionstring till fast värde och implementerar bara en tom constructor.
        /// Detta kanske inte är det bästa sättet men allt fungerar och nu har vi using på alla funktioner,
        /// och därmed gör vi oss av med connection så fort den är använd.
        /// Att sätta ConnectionString i constructorn fungerar inte, och nu gör vi ny SqlConnection direkt i Using statement.
        /// </summary>
        /// 

        //Vi är medvetna om att lösenordet ligger hårdkodat, men har val att inte åtgärda det i denna version. Nästa gång kommer lösenordet behövas skrivas in.
        private string ConnectionString = "Data Source = sql6009.site4now.net; Initial Catalog = DB_A53DDD_grupp5; User ID = DB_A53DDD_grupp5_admin; Password = grupp5pizza";

        /*private SqlConnection connection { get; }

        public MSSQL()
        {
            //string ConnectionString = "Data Source = sql6009.site4now.net; Initial Catalog = DB_A53DDD_grupp5; User ID = DB_A53DDD_grupp5_admin; Password = grupp5pizza";
            //connection = new SqlConnection(ConnectionString);
            //connection.Open();
        }*/

        //Employees
        #region Employees
        public async Task AddEmployee(Employee emp, string storedProcedureToAddEmployee = "AddEmployee")
        {
            using (IDbConnection connection = new SqlConnection(ConnectionString))
            {
                await connection.QueryAsync<Employee>(storedProcedureToAddEmployee, new { Password = emp.Password, Role = emp.Role }, commandType: CommandType.StoredProcedure);
            }
        }
        public async Task<IEnumerable<Employee>> GetAllEmployees(string storedProcedureToShowEmployees = "GetAllEmployees")
        {
            using (IDbConnection connection = new SqlConnection(ConnectionString))
            {
                return (await connection.QueryAsync<Employee>(storedProcedureToShowEmployees, commandType: CommandType.StoredProcedure));
            }
        }
        public async Task<Employee> GetSingleEmployee(int id, string storedProcedureToShowSingleEmployee = "GetSingleEmployee")
        {
            using (IDbConnection connection = new SqlConnection(ConnectionString))
            {
                return (await connection.QueryAsync<Employee>(storedProcedureToShowSingleEmployee, new { UserID = id }, commandType: CommandType.StoredProcedure)).First();
            }
        }
        public async Task UpdateEmployee(Employee emp, string storedProcedureToUpdateEmployee = "UpdateEmployeeByID")
        {
            using (IDbConnection connection = new SqlConnection(ConnectionString))
            {
                await connection.QueryAsync<Employee>(storedProcedureToUpdateEmployee, new { UserID = emp.UserID, Password = emp.Password, Role = emp.Role }, commandType: CommandType.StoredProcedure);
            }
        }
        public async Task DeleteEmployee(Employee emp, string storedProcedureToDeleteEmployee = "DeleteEmployeeByID")
        {
            using (IDbConnection connection = new SqlConnection(ConnectionString))
            {
                await connection.QueryAsync<Employee>(storedProcedureToDeleteEmployee, new { UserID = emp.UserID }, commandType: CommandType.StoredProcedure);
            }
        }
        #endregion
        //Pizzas
        #region Pizzas
        public async Task<Pizza> AddPizza(Pizza pizza, string storedProcedureToAddPizza = "AddPizza")
        {
            using (IDbConnection connection = new SqlConnection(ConnectionString))
            {
                return (await connection.QueryAsync<Pizza>(storedProcedureToAddPizza, new { Type = pizza.Type, Price = pizza.Price, PizzabaseID = pizza.PizzabaseID }, commandType: CommandType.StoredProcedure)).First();
            }
        }

        public async Task AddCondimentToPizza(Pizza pizza, string storedProcedureToAddCondimentToPizza = "AddStandardCondimentToPizza")
        {
            using (IDbConnection connection = new SqlConnection(ConnectionString))
            {
                foreach (var item in pizza.PizzaIngredients)
                {
                    await connection.QueryAsync(storedProcedureToAddCondimentToPizza, new { CondimentID = item.CondimentID, PizzaID = pizza.PizzaID }, commandType: CommandType.StoredProcedure);
                }
            }
        }

        public async Task<IEnumerable<Pizza>> GetAllPizzas(string storedProcedureToShowPizzas = "GetAllPizzas")
        {
            using (IDbConnection connection = new SqlConnection(ConnectionString))
            {
                return (await connection.QueryAsync<Pizza>(storedProcedureToShowPizzas, commandType: CommandType.StoredProcedure));
            }
        }

        public async Task<Pizza> GetSinglePizza(int id, string storedProcedureToShowSinglePizza = "GetSpecificPizza")
        {
            using (IDbConnection connection = new SqlConnection(ConnectionString))
            {
                Pizza newPizza = new Pizza();
                newPizza = (await connection.QueryAsync<Pizza>(storedProcedureToShowSinglePizza, new { PizzaID = id }, commandType: CommandType.StoredProcedure)).First();
                newPizza.PizzaIngredients = (await connection.QueryAsync<Condiment>("GetIngredientsFromSpecificPizza", new { PizzaID = id }, commandType: CommandType.StoredProcedure)).ToList();
                return newPizza;
            }
        }

        public async Task UpdatePizza(Pizza pizza, string storedProcedureToUpdatePizza = "UpdatePizzaByID")
        {
            using (IDbConnection connection = new SqlConnection(ConnectionString))
            {
                await connection.QueryAsync<Pizza>(storedProcedureToUpdatePizza, new { PizzaID = pizza.PizzaID, Type = pizza.Type, Price = pizza.Price, PizzabaseID = pizza.PizzabaseID }, commandType: CommandType.StoredProcedure);
            }
        }

        public async Task DeletePizza(Pizza pizza, string storedProcedureToDeletePizza = "DeletePizzaByID")
        {

            using (IDbConnection connection = new SqlConnection(ConnectionString))
            {
                await connection.QueryAsync<Pizza>(storedProcedureToDeletePizza, new { PizzaID = pizza.PizzaID }, commandType: CommandType.StoredProcedure);
            }
        }
        public async Task<IEnumerable<Condiment>> GetIngredientsFromSpecificPizza(int id, string storedProcedureToGetIngredientFromSpecifikPizza = "GetIngredientsFromSpecificPizza")
        {
            using (IDbConnection connection = new SqlConnection(ConnectionString))
            {
                return (await connection.QueryAsync<Condiment>(storedProcedureToGetIngredientFromSpecifikPizza, new { PizzaID = id }, commandType: CommandType.StoredProcedure));
            }
        }
        public async Task DeleteCondimentFromPizza(Pizza pizza, Condiment condiment, string storedProcedure = "DeleteCondimentFromPizza")
        {
            using (IDbConnection connection = new SqlConnection(ConnectionString))
            {
                await connection.QueryAsync<Pizza>(storedProcedure, new { PizzaID = pizza.PizzaID, CondimentID = condiment.CondimentID }, commandType: CommandType.StoredProcedure);
            }
        }
        public async Task<IEnumerable<(int, string)>> GetAllPizzabases(string storedProcedure = "GetAllPizzabases")
        {
            using (IDbConnection connection = new SqlConnection(ConnectionString))
            {
                return (await connection.QueryAsync<(int, string)>(storedProcedure, commandType: CommandType.StoredProcedure));
            }
        }
        #endregion

        #region Condiments
        public async Task AddCondiment(Condiment cond, string storedProcedureToAddCondiment = "AddCondiment")
        {
            using (IDbConnection connection = new SqlConnection(ConnectionString))
            {
                await connection.QueryAsync<Condiment>(storedProcedureToAddCondiment, new { Type = cond.Type, Price = cond.Price }, commandType: CommandType.StoredProcedure);
            }
        }

        public async Task<IEnumerable<Condiment>> GetAllCondiments(string storedProcedureToShowCondiment = "GetAllCondiments")
        {
            using (IDbConnection connection = new SqlConnection(ConnectionString))
            {
                return (await connection.QueryAsync<Condiment>(storedProcedureToShowCondiment, commandType: CommandType.StoredProcedure));
            }
        }

        public async Task<Condiment> GetSingleCondiment(int id, string storedProcedureToShowSingleCondiment = "GetSingleCondiment")
        {
            using (IDbConnection connection = new SqlConnection(ConnectionString))
            {
                return (await connection.QueryAsync<Condiment>(storedProcedureToShowSingleCondiment, new { CondimentID = id }, commandType: CommandType.StoredProcedure)).First();
            }
        }

        public async Task UpdateCondiment(Condiment cond, string storedProcedureToUpdateCondiment = "UpdateCondimentByID")
        {
            using (IDbConnection connection = new SqlConnection(ConnectionString))
            {
                await connection.QueryAsync<Condiment>(storedProcedureToUpdateCondiment, new { CondimentID = cond.CondimentID, Type = cond.Type, Price = cond.Price, }, commandType: CommandType.StoredProcedure);
            }
        }

        public async Task DeleteCondiment(Condiment cond, string storedProcedureToDeleteCondiment = "DeleteCondimentByID")
        {
            using (IDbConnection connection = new SqlConnection(ConnectionString))
            {
                await connection.QueryAsync<Condiment>(storedProcedureToDeleteCondiment, new { CondimentID = cond.CondimentID }, commandType: CommandType.StoredProcedure);
            }
        }
        #endregion

        #region Extras
        public async Task AddExtra(Extra extra, string storedProcedureToAddExtra = "AddExtra")
        {
            using (IDbConnection connection = new SqlConnection(ConnectionString))
            {
                await connection.QueryAsync<Extra>(storedProcedureToAddExtra, new { Type = extra.Type, Price = extra.Price }, commandType: CommandType.StoredProcedure);
            }
        }

        public async Task<IEnumerable<Extra>> GetAllExtras(string storedProcedureToShowExtra = "GetAllExtras")
        {
            using (IDbConnection connection = new SqlConnection(ConnectionString))
            {
                return (await connection.QueryAsync<Extra>(storedProcedureToShowExtra, commandType: CommandType.StoredProcedure));
            }
        }

        public async Task<Extra> GetSingleExtra(int id, string storedProcedureToShowSingleExtra = "GetSingleExtra")
        {
            using (IDbConnection connection = new SqlConnection(ConnectionString))
            {
                return (await connection.QueryAsync<Extra>(storedProcedureToShowSingleExtra, new { ProductID = id }, commandType: CommandType.StoredProcedure)).First();
            }
        }

        public async Task UpdateExtra(Extra extra, string storedProcedureToUpdateExtra = "UpdateExtraByID")
        {

            using (IDbConnection connection = new SqlConnection(ConnectionString))
            {
                await connection.QueryAsync<Extra>(storedProcedureToUpdateExtra, new { ProductID = extra.ProductID, Type = extra.Type, Price = extra.Price, }, commandType: CommandType.StoredProcedure);
            }
        }

        public async Task DeleteExtra(Extra extra, string storedProcedureToDeleteExtra = "DeleteExtraByID")
        {

            using (IDbConnection connection = new SqlConnection(ConnectionString))
            {
                await connection.QueryAsync<Extra>(storedProcedureToDeleteExtra, new { ProductID = extra.ProductID }, commandType: CommandType.StoredProcedure);
            }
        }
        #endregion
        //Checking functions for existing ID and user/password.
        #region CheckingID functions
        public async Task<(bool, string)> CheckUserIdAndPassword(int ID, string password, string storedProcedureToCheckLogin = "CheckPassword", string storedProcedureToCheckRole = "CheckRole")
        {

            using (IDbConnection connection = new SqlConnection(ConnectionString))
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

            using (IDbConnection connection = new SqlConnection(ConnectionString))
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

            using (IDbConnection connection = new SqlConnection(ConnectionString))
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

            using (IDbConnection connection = new SqlConnection(ConnectionString))
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

            using (IDbConnection connection = new SqlConnection(ConnectionString))
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
            using (IDbConnection connection = new SqlConnection(ConnectionString))
            {
                return (await connection.QueryAsync<Order>(storedProcedureToShowOrders, commandType: CommandType.StoredProcedure));
            }
        }
        public async Task<int> AddOrder(Order order, string storedProcedureToAddOrder = "AddOrder")
        {
            using (IDbConnection connection = new SqlConnection(ConnectionString))
            {
                return (await connection.QueryAsync<int>(storedProcedureToAddOrder, new
                {
                    PizzasJSON = JsonConvert.SerializeObject(order.PizzaList),
                    ExtrasJSON = JsonConvert.SerializeObject(order.ExtraList),
                    OrderPrice = order.Price
                },
                      commandType: CommandType.StoredProcedure)).First();
            }
        }
        public async Task<IEnumerable<Order>> GetOrderByStatus(int statusID, string storedProcedureToGetOrderByStatus = "GetOrderByStatus")
        {
            using (IDbConnection connection = new SqlConnection(ConnectionString))
            {
                List<Order> Result = new List<Order>();
                IEnumerable<(int, string, string, float, int)> SerializedOrders = (await connection.QueryAsync<(int, string, string, float, int)>(storedProcedureToGetOrderByStatus, new { Status = statusID }, commandType: CommandType.StoredProcedure));
                foreach (var o in SerializedOrders)
                {
                    Order CurrentOrder = new Order();
                    CurrentOrder.OrderID = o.Item1;
                    CurrentOrder.PizzaList = JsonConvert.DeserializeObject<List<Pizza>>(o.Item2);
                    CurrentOrder.ExtraList = JsonConvert.DeserializeObject<List<Extra>>(o.Item3);
                    CurrentOrder.Price = o.Item4;
                    CurrentOrder.Status = o.Item5;
                    Result.Add(CurrentOrder);
                }
                return Result;
            }
        }

        public async Task UpdateOrderStatusWhenOrderIsServed(Employee emp, Order order, string storedProcedure = "UpdateOrderStatusWhenOrderIsServed")
        {
            using (IDbConnection connection = new SqlConnection(ConnectionString))
            {
                await connection.QueryAsync<Order>(storedProcedure, new { EmployeeID = emp.UserID, OrderID = order.OrderID }, commandType: CommandType.StoredProcedure);
            }
        }

        public async Task UpdateOrderStatusWhenOrderIsCooked(Order order, string storedProcedure = "UpdateOrderStatusWhenOrderIsCooked")
        {
            using (IDbConnection connection = new SqlConnection(ConnectionString))
            {
                await connection.QueryAsync<Order>(storedProcedure, new { OrderID = order.OrderID }, commandType: CommandType.StoredProcedure);
            }
        }

        public async Task<Order> GetSingleOrder(int ID, string storedProcedure = "GetSingleOrder")
        {
            using (IDbConnection connection = new SqlConnection(ConnectionString))
            {
                return (await connection.QueryAsync<Order>(storedProcedure, new { OrderID = ID }, commandType: CommandType.StoredProcedure)).First();
            }
        }

        public async Task DeleteOldOrder(Order order, string storedProcedureToDeleteOldOrder = "DeleteOldOrderByID")
        {
            using (IDbConnection connection = new SqlConnection(ConnectionString))
            {
                await connection.QueryAsync<Order>(storedProcedureToDeleteOldOrder, new { OrderID = order.OrderID }, commandType: CommandType.StoredProcedure);
            }
        }

        public async Task BakerChoosingOrderToCook(Employee employee, Order order, string storedProcedure = "BakerChoosingOrderToCook")
        {
            using (IDbConnection connection = new SqlConnection(ConnectionString))
            {
                await connection.QueryAsync<Order>(storedProcedure, new { EmployeeID = employee.UserID, OrderID = order.OrderID }, commandType: CommandType.StoredProcedure);
            }
        }

        public async Task BakerCancellingCooking(Employee employee, Order order, string storedProcedure = "BakerCancellingCooking")
        {
            using (IDbConnection connection = new SqlConnection(ConnectionString))
            {
                await connection.QueryAsync<Order>(storedProcedure, new { EmployeeID = employee.UserID, OrderID = order.OrderID }, commandType: CommandType.StoredProcedure);
            }
        }
        #endregion
    }

    public class PostgreSQL : IDatabase
    {
        /// <summary>
        /// Samma gäller här som för MSSQL Repository
        /// Vi är medvetna om att detta kan skötas på ett snyggare och säkrare sätt 
        /// men har valt detta för att spara tid och få allt att funka
        /// </summary>
        private string ConnectionString = "Host=weboholics-demo.dyndns-ip.com;Port=5433;Username=grupp5;Password=grupp5;Database=grupp5";

        public async Task AddCondiment(Condiment cond, string storedProcedureToAddCondiment = "\"AddCondiment\"")
        {
            using (IDbConnection connection = new NpgsqlConnection(ConnectionString))
            {
                await connection.QueryAsync<Condiment>(storedProcedureToAddCondiment, new { _Type = cond.Type, _Price = cond.Price }, commandType: CommandType.StoredProcedure);
            }
        }

        public async Task AddEmployee(Employee emp, string storedProcedureToAddEmployee = "\"AddEmployee\"")
        {
            using (IDbConnection connection = new NpgsqlConnection(ConnectionString))
            {
                await connection.QueryAsync<Employee>(storedProcedureToAddEmployee, new { _Password = emp.Password, _Role = emp.Role }, commandType: CommandType.StoredProcedure);
            }
        }

        public async Task AddExtra(Extra extra, string storedProcedureToAddExtra = "\"AddExtra\"")
        {
            using (IDbConnection connection = new NpgsqlConnection(ConnectionString))
            {
                await connection.QueryAsync<Extra>(storedProcedureToAddExtra, new { _Type = extra.Type, _Price = extra.Price }, commandType: CommandType.StoredProcedure);
            }
        }

        public async Task<Pizza> AddPizza(Pizza pizza, string storedProcedureToAddPizza = "\"AddPizza\"")
        {
            using (IDbConnection connection = new NpgsqlConnection(ConnectionString))
            {
                return (await connection.QueryAsync<Pizza>(storedProcedureToAddPizza, new { _Type = pizza.Type, _Price = pizza.Price, _PizzabaseID = pizza.PizzabaseID }, commandType: CommandType.StoredProcedure)).First();
            }
        }

        public async Task<bool> CheckIfCondimentIDExists(int ID, string storedProcedureToCheckForExistingCondimentID = "\"CheckForExistingCondimentID\"")
        {
            using (IDbConnection connection = new NpgsqlConnection(ConnectionString))
            {
                bool IDExists = (await connection.QueryAsync<bool>(storedProcedureToCheckForExistingCondimentID, new { _CondimentID = ID }, commandType: CommandType.StoredProcedure)).First();
                if (IDExists == true)
                    return true;
                else
                    return false;
            }
        }

        public async Task<bool> CheckIfPizzaIDExists(int ID, string storedProcedureToCheckForExistingPizzaID = "\"CheckForExistingPizzaID\"")
        {
            using (IDbConnection connection = new NpgsqlConnection(ConnectionString))
            {
                bool IDExists = (await connection.QueryAsync<bool>(storedProcedureToCheckForExistingPizzaID, new { _PizzaID = ID }, commandType: CommandType.StoredProcedure)).First();
                if (IDExists == true)
                    return true;
                else
                    return false;
            }
        }

        public async Task<bool> CheckIfProductIDExists(int ID, string storedProcedureToCheckForExistingProductID = "\"CheckForExistingProductID\"")
        {
            using (IDbConnection connection = new NpgsqlConnection(ConnectionString))
            {
                bool IDExists = (await connection.QueryAsync<bool>(storedProcedureToCheckForExistingProductID, new { _ProductID = ID }, commandType: CommandType.StoredProcedure)).First();
                if (IDExists == true)
                    return true;
                else
                    return false;
            }
        }

        public async Task<bool> CheckIfUserIDExists(int ID, string storedProcedureToCheckForExistingID = "\"CheckForExistingID\"")
        {
            using (IDbConnection connection = new NpgsqlConnection(ConnectionString))
            {
                bool IDExists = (await connection.QueryAsync<bool>(storedProcedureToCheckForExistingID, new { _UserID = ID }, commandType: CommandType.StoredProcedure)).First();
                if (IDExists == true)
                    return true;
                else
                    return false;
            }
        }

        public async Task<(bool, string)> CheckUserIdAndPassword(int ID, string password, string storedProcedureToCheckLogin = "checkpassword", string storedProcedureToCheckRole = "checkrole")
        {
            using (IDbConnection connection = new NpgsqlConnection(ConnectionString))
            {
                bool passCheck = (await connection.QueryAsync<bool>(storedProcedureToCheckLogin, new { _UserID = ID, _Password = password }, commandType: CommandType.StoredProcedure)).First();
                bool correctLogInCredentials = false;
                string role = "";

                if (passCheck == true)
                {
                    correctLogInCredentials = true;
                    role = (await connection.QueryAsync<string>(storedProcedureToCheckRole, new { _UserID = ID }, commandType: CommandType.StoredProcedure)).First();
                }

                return (correctLogInCredentials, role);
            }
        }

        public async Task DeleteCondiment(Condiment cond, string storedProcedureToDeleteCondiment = "\"DeleteCondimentByID\"")
        {
            using (IDbConnection connection = new NpgsqlConnection(ConnectionString))
            {
                await connection.QueryAsync<Condiment>(storedProcedureToDeleteCondiment, new { _CondimentID = cond.CondimentID }, commandType: CommandType.StoredProcedure);
            }
        }

        public async Task DeleteEmployee(Employee emp, string storedProcedureToDeleteEmployee = "\"DeleteEmployeeByID\"")
        {
            using (IDbConnection connection = new NpgsqlConnection(ConnectionString))
            {
                await connection.QueryAsync<Employee>(storedProcedureToDeleteEmployee, new { _UserID = emp.UserID }, commandType: CommandType.StoredProcedure);
            }
        }

        public async Task DeleteExtra(Extra extra, string storedProcedureToDeleteExtra = "\"DeleteExtraByID\"")
        {
            using (IDbConnection connection = new NpgsqlConnection(ConnectionString))
            {
                await connection.QueryAsync<Extra>(storedProcedureToDeleteExtra, new { _ProductID = extra.ProductID }, commandType: CommandType.StoredProcedure);
            }
        }

        public async Task DeletePizza(Pizza pizza, string storedProcedureToDeletePizza = "\"DeletePizzaByID\"")
        {
            using (IDbConnection connection = new NpgsqlConnection(ConnectionString))
            {
                await connection.QueryAsync<Pizza>(storedProcedureToDeletePizza, new { _PizzaID = pizza.PizzaID }, commandType: CommandType.StoredProcedure);
            }
        }

        public async Task<IEnumerable<Condiment>> GetIngredientsFromSpecificPizza(int ID, string storedProcedureToGetIngredientFromSpecifikPizza = "\"GetIngredientsFromSpecificPizza\"")
        {
            using (IDbConnection connection = new NpgsqlConnection(ConnectionString))
            {
                return (await connection.QueryAsync<Condiment>(storedProcedureToGetIngredientFromSpecifikPizza, new { _PizzaID = ID }, commandType: CommandType.StoredProcedure));
            }
        }

        public async Task<IEnumerable<Condiment>> GetAllCondiments(string storedProcedureToShowCondiment = "\"GetAllCondiments\"")
        {
            using (IDbConnection connection = new NpgsqlConnection(ConnectionString))
            {
                return (await connection.QueryAsync<Condiment>(storedProcedureToShowCondiment, commandType: CommandType.StoredProcedure));
            }
        }

        public async Task<IEnumerable<Employee>> GetAllEmployees(string storedProcedureToShowEmployees = "\"GetAllEmployees\"")
        {
            using (IDbConnection connection = new NpgsqlConnection(ConnectionString))
            {
                return (await connection.QueryAsync<Employee>(storedProcedureToShowEmployees, commandType: CommandType.StoredProcedure));
            }
        }

        public async Task<IEnumerable<Extra>> GetAllExtras(string storedProcedureToShowExtra = "\"GetAllExtras\"")
        {
            using (IDbConnection connection = new NpgsqlConnection(ConnectionString))
            {
                return (await connection.QueryAsync<Extra>(storedProcedureToShowExtra, commandType: CommandType.StoredProcedure));
            }
        }

        public async Task<IEnumerable<Pizza>> GetAllPizzas(string storedProcedureToShowPizzas = "\"GetAllPizzas\"")
        {
            using (IDbConnection connection = new NpgsqlConnection(ConnectionString))
            {
                return (await connection.QueryAsync<Pizza>(storedProcedureToShowPizzas, commandType: CommandType.StoredProcedure));
            }
        }

        public async Task<Condiment> GetSingleCondiment(int ID, string storedProcedureToShowSingleCondiment = "\"GetSingleCondiment\"")
        {
            using (IDbConnection connection = new NpgsqlConnection(ConnectionString))
            {
                return (await connection.QueryAsync<Condiment>(storedProcedureToShowSingleCondiment, new { _CondimentID = ID }, commandType: CommandType.StoredProcedure)).First();
            }
        }

        public async Task<Employee> GetSingleEmployee(int ID, string storedProcedureToShowSingleEmployee = "\"GetSingleEmployee\"")
        {
            using (IDbConnection connection = new NpgsqlConnection(ConnectionString))
            {
                return (await connection.QueryAsync<Employee>(storedProcedureToShowSingleEmployee, new { _UserID = ID }, commandType: CommandType.StoredProcedure)).First();
            }
        }

        public async Task<Extra> GetSingleExtra(int ID, string storedProcedureToShowSingleExtra = "\"GetSingleExtra\"")
        {
            using (IDbConnection connection = new NpgsqlConnection(ConnectionString))
            {
                return (await connection.QueryAsync<Extra>(storedProcedureToShowSingleExtra, new { _ProductID = ID }, commandType: CommandType.StoredProcedure)).First();
            }
        }

        public async Task<Pizza> GetSinglePizza(int ID, string storedProcedureToShowSinglePizza = "\"GetSpecificPizza\"")
        {
            using (IDbConnection connection = new NpgsqlConnection(ConnectionString))
            {
                Pizza newPizza = new Pizza();
                newPizza = (await connection.QueryAsync<Pizza>(storedProcedureToShowSinglePizza, new { _PizzaID = ID }, commandType: CommandType.StoredProcedure)).First();
                newPizza.PizzaIngredients = (await connection.QueryAsync<Condiment>("getingredientsfromspecificpizza", new { _PizzaID = ID }, commandType: CommandType.StoredProcedure)).ToList();
                return newPizza;
            }
        }

        public async Task UpdateCondiment(Condiment cond, string storedProcedureToUpdateCondiment = "\"UpdateCondimentByID\"")
        {
            using (IDbConnection connection = new NpgsqlConnection(ConnectionString))
            {
                await connection.QueryAsync<Condiment>(storedProcedureToUpdateCondiment, new { _CondimentID = cond.CondimentID, _Type = cond.Type, _Price = cond.Price, }, commandType: CommandType.StoredProcedure);
            }
        }

        public async Task UpdateEmployee(Employee emp, string storedProcedureToUpdateEmployee = "\"UpdateEmployeeByID\"")
        {
            using (IDbConnection connection = new NpgsqlConnection(ConnectionString))
            {
                await connection.QueryAsync<Employee>(storedProcedureToUpdateEmployee, new { _UserID = emp.UserID, _Password = emp.Password, _Role = emp.Role }, commandType: CommandType.StoredProcedure);
            }
        }

        public async Task UpdateExtra(Extra extra, string storedProcedureToUpdateExtra = "\"UpdateExtraByID\"")
        {
            using (IDbConnection connection = new NpgsqlConnection(ConnectionString))
            {
                await connection.QueryAsync<Extra>(storedProcedureToUpdateExtra, new { _ProductID = extra.ProductID, _Type = extra.Type, _Price = extra.Price, }, commandType: CommandType.StoredProcedure);
            }
        }

        public async Task UpdatePizza(Pizza pizza, string storedProcedureToUpdatePizza = "\"UpdatePizzaByID\"")
        {
            using (IDbConnection connection = new NpgsqlConnection(ConnectionString))
            {
                await connection.QueryAsync<Pizza>(storedProcedureToUpdatePizza, new { _PizzaID = pizza.PizzaID, _Type = pizza.Type, _Price = pizza.Price, _PizzabaseID = pizza.PizzabaseID }, commandType: CommandType.StoredProcedure);
            }
        }

        public async Task AddCondimentToPizza(Pizza pizza, string storedProcedureToAddCondimentToPizza = "\"AddStandardCondimentToPizza\"")
        {
            using (IDbConnection connection = new NpgsqlConnection(ConnectionString))
            {
                foreach (var item in pizza.PizzaIngredients)
                {
                    await connection.QueryAsync(storedProcedureToAddCondimentToPizza, new { _CondimentID = item.CondimentID, _PizzaID = pizza.PizzaID }, commandType: CommandType.StoredProcedure);
                }
            }
        }

        public async Task<IEnumerable<Order>> GetAllOrders(string storedProcedureToShowOrders = "\"GetAllOrders\"")
        {
            using (IDbConnection connection = new NpgsqlConnection(ConnectionString))
            {
                return (await connection.QueryAsync<Order>(storedProcedureToShowOrders, commandType: CommandType.StoredProcedure));
            }
        }

        public async Task<int> AddOrder(Order order, string storedProcedureToAddOrder = "\"AddOrder\"")
        {
            using (IDbConnection connection = new NpgsqlConnection(ConnectionString))
            {
                return (await connection.QueryAsync<int>(storedProcedureToAddOrder, new
                {
                    _PizzasJSON = JsonConvert.SerializeObject(order.PizzaList),
                    _ExtrasJSON = JsonConvert.SerializeObject(order.ExtraList),
                    _OrderPrice = order.Price
                },
                        commandType: CommandType.StoredProcedure)).First();
            }
        }

        public async Task DeleteCondimentFromPizza(Pizza pizza, Condiment condiment, string storedProcedure = "\"DeleteCondimentFromPizza\"")
        {
            using (IDbConnection connection = new NpgsqlConnection(ConnectionString))
            {
                await connection.QueryAsync<Pizza>(storedProcedure, new { _PizzaID = pizza.PizzaID, _CondimentID = condiment.CondimentID }, commandType: CommandType.StoredProcedure);
            }
        }

        public async Task<IEnumerable<Order>> GetOrderByStatus(int statusID, string storedProcedureToGetOrderByStatus = "\"GetOrderByStatus\"")
        {
            using (IDbConnection connection = new NpgsqlConnection(ConnectionString))
            {
                List<Order> Result = new List<Order>();
                IEnumerable<(int, string, string, float, int)> SerializedOrders = (await connection.QueryAsync<(int, string, string, float, int)>(storedProcedureToGetOrderByStatus, new { _Status = statusID }, commandType: CommandType.StoredProcedure));
                foreach (var o in SerializedOrders)
                {
                    Order CurrentOrder = new Order();
                    CurrentOrder.OrderID = o.Item1;
                    CurrentOrder.PizzaList = JsonConvert.DeserializeObject<List<Pizza>>(o.Item2);
                    CurrentOrder.ExtraList = JsonConvert.DeserializeObject<List<Extra>>(o.Item3);
                    CurrentOrder.Price = o.Item4;
                    CurrentOrder.Status = o.Item5;
                    Result.Add(CurrentOrder);
                }
                return Result;
            }
        }

        public async Task UpdateOrderStatusWhenOrderIsServed(Employee emp, Order order, string storedProcedure = "\"UpdateOrderStatusWhenOrderIsServed\"")
        {
            using (IDbConnection connection = new NpgsqlConnection(ConnectionString))
            {
                await connection.QueryAsync<Order>(storedProcedure, new { _EmployeeID = emp.UserID, _OrderID = order.OrderID }, commandType: CommandType.StoredProcedure);
            }
        }

        public async Task UpdateOrderStatusWhenOrderIsCooked(Order order, string storedProcedure = "\"UpdateOrderStatusWhenOrderIsCooked\"")
        {
            using (IDbConnection connection = new NpgsqlConnection(ConnectionString))
            {
                await connection.QueryAsync<Order>(storedProcedure, new { _OrderID = order.OrderID }, commandType: CommandType.StoredProcedure);
            }
        }

        public async Task<Order> GetSingleOrder(int ID, string storedProcedure = "\"GetSingleOrder\"")
        {
            using (IDbConnection connection = new NpgsqlConnection(ConnectionString))
            {
                return (await connection.QueryAsync<Order>(storedProcedure, new { _OrderID = ID }, commandType: CommandType.StoredProcedure)).First();
            }
        }

        public async Task<IEnumerable<(int, string)>> GetAllPizzabases(string storedProcedure = "\"GetAllPizzabases\"")
        {
            using (IDbConnection connection = new NpgsqlConnection(ConnectionString))
            {
                return (await connection.QueryAsync<(int, string)>(storedProcedure, commandType: CommandType.StoredProcedure));
            }
        }

        public async Task DeleteOldOrder(Order order, string storedProcedureToDeleteOldOrder = "\"DeleteOldOrderByID\"")
        {
            using (IDbConnection connection = new NpgsqlConnection(ConnectionString))
            {
                await connection.QueryAsync<Order>(storedProcedureToDeleteOldOrder, new { _OrderID = order.OrderID }, commandType: CommandType.StoredProcedure);
            }
        }

        public async Task BakerChoosingOrderToCook(Employee employee, Order order, string storedProcedure = "BakerChoosingOrderToCook")
        {
            using (IDbConnection connection = new NpgsqlConnection(ConnectionString))
            {
                await connection.QueryAsync<Order>(storedProcedure, new { _UserID = employee.UserID, _OrderID = order.OrderID }, commandType: CommandType.StoredProcedure);

            }
        }

        public async Task BakerCancellingCooking(Employee employee, Order order, string storedProcedure = "BakerCancellingCooking")
        {
            using (IDbConnection connection = new NpgsqlConnection(ConnectionString))
            {
                await connection.QueryAsync<Order>(storedProcedure, new { _UserID = employee.UserID, _OrderID = order.OrderID }, commandType: CommandType.StoredProcedure);

            }
        }
        #endregion
    }


}

