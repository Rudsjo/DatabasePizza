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
    }
    public class Pizza    
    {
        //Överensstämmer med databasen
        public int PizzaID { get; }
        public int PizzabaseID { get; set; }
        public string Type { get; set; }
        public float Price { get; set; }
        public string Base { get; set; }
        public List<Condiment> PizzaIngredients { get; set; }


        //Ska vi ha ovverride till alla klasser?
        public override string ToString()
        {
            return $"{this.PizzaID} {this.Type} {this.Price} {this.Base}";
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
        public Task DeleteEmployee(int ID, string storedProcedure = "DeleteEmployeeByID");

        //Pizza
        public Task<Pizza> AddPizza(Pizza pizza, string storedProcedure = "AddPizza");
        public Task UpdatePizza(Pizza pizza, string storedProcedure = "UpdatePizzaByID");
        public Task<IEnumerable<Pizza>> GetAllPizzas(string storedProcedure = "GetAllPizzas");
        public Task<Pizza> GetSinglePizza(int ID, string storedProcedure = "GetSpecificPizza");
        //public Task<Pizza> GetSinglePizza(string PizzaName, string storedProcedure = "GetSpecificPizza");
        public Task DeletePizza(int ID, string storedProcedure = "DeletePizzaByID");
        public Task<IEnumerable<Condiment>> GetIngredientsFromSpecificPizza(int ID, string storedProcedure = "GetIngredientsFromSpecificPizza");
        public Task AddCondimentToPizza(Pizza pizza, string storedProcedureToAddCondimentToPizza = "AddStandardCondimentToPizza");

        //Condiment
        public Task AddCondiment(Condiment cond, string storedProcedure = "AddCondiment");
        public Task UpdateCondiment(Condiment cond, string storedProcedure = "UpdateCondimentByID");
        public Task<IEnumerable<Condiment>> GetAllCondiments(string storedProcedure = "GetAllCondiments");
        public Task<Condiment> GetSingleCondiment(int ID, string storedProcedure = "GetSingleCondiment");
        public Task DeleteCondiment(int ID, string storedProcedure = "DeleteCondimentByID");

        //Extra
        public Task AddExtra(Extra extra, string storedProcedure = "AddExtra");
        public Task UpdateExtra(Extra extra, string storedProcedure = "UpdateExtraByID");
        public Task<IEnumerable<Extra>> GetAllExtras(string storedProcedure = "GetAllExtras");
        public Task<Extra> GetSingleExtra(int ID, string storedProcedure = "GetSingleExtra");
        public Task DeleteExtra(int ID, string storedProcedure = "DeleteExtraByID");


        //Functions for checking user/password and for checking if an ID exists in the database.
        public Task<(bool, string)> CheckUserIdAndPassword(int ID, string password, string storedProcedure = "CheckPassword", string secondStoredProcedure = "CheckRole");
        public Task<bool> CheckIfUserIDExists(int ID, string storedProcedure = "CheckForExistingID");
        public Task<bool> CheckIfProductIDExists(int ID, string storedProcedure = "CheckForExistingProductID");
        public Task<bool> CheckIfCondimentIDExists(int ID, string storedProcedure = "CheckForExistingCondimentID");
        public Task<bool> CheckIfPizzaIDExists(int ID, string storedProcedure = "CheckForExistingPizzaID");

        //Order interfaceimplementation måste göras. 
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

        public async Task AddEmployee(Employee emp, string storedProcedureToAddEmployee = "AddEmployee")
        {
            await connection.QueryAsync<Employee>(storedProcedureToAddEmployee, new { Password = emp.Password, Role = emp.Role }, commandType: CommandType.StoredProcedure);
        }

        public async Task<IEnumerable<Employee>> GetAllEmployees(string storedProcedureToShowEmployees = "GetAllEmployees")
        {
            //IEnumerable<Employees> employees = (await connection.QueryAsync<Employees>(storedProcedureToShowEmployees, commandType: CommandType.StoredProcedure));
            //return employees;
            return (await connection.QueryAsync<Employee>(storedProcedureToShowEmployees, commandType: CommandType.StoredProcedure));
        }

        public async Task<Employee> GetSingleEmployee(int id, string storedProcedureToShowSingleEmployee = "GetSingleEmployee")
        {
            return (await connection.QueryAsync<Employee>(storedProcedureToShowSingleEmployee, new { UserID = id }, commandType: CommandType.StoredProcedure)).First();
        }

        public async Task UpdateEmployee(Employee emp, string storedProcedureToUpdateEmployee = "UpdateEmployeeByID")
        {
            await connection.QueryAsync<Employee>(storedProcedureToUpdateEmployee, new { UserID = emp.UserID, Password = emp.Password, Role = emp.Role }, commandType: CommandType.StoredProcedure);
        }

        public async Task DeleteEmployee(int id, string storedProcedureToDeleteEmployee = "DeleteEmployeeByID")
        {
            await connection.QueryAsync<Employee>(storedProcedureToDeleteEmployee, new { UserID = id }, commandType: CommandType.StoredProcedure);
        }

        //Pizzas

        public async Task<Pizza> AddPizza(Pizza pizza, string storedProcedureToAddPizza = "AddPizza")
        {
            return (await connection.QueryAsync<Pizza>(storedProcedureToAddPizza, new { Type = pizza.Type, Price = pizza.Price, PizzabaseID = pizza.PizzabaseID }, commandType: CommandType.StoredProcedure)).First();
        }

        public async Task AddCondimentToPizza(Pizza pizza, string storedProcedureToAddCondimentToPizza = "AddStandardCondimentToPizza")
        {
            foreach (var item in pizza.PizzaIngredients)
            {
                await connection.QueryAsync(storedProcedureToAddCondimentToPizza, new {CondimentID = item.CondimentID, PizzaID = pizza.PizzaID }, commandType: CommandType.StoredProcedure);
            }          
        }

        public async Task<IEnumerable<Pizza>> GetAllPizzas(string storedProcedureToShowPizzas = "GetAllPizzas")
        {
            return (await connection.QueryAsync<Pizza>(storedProcedureToShowPizzas, commandType: CommandType.StoredProcedure));
        }

        public async Task<Pizza> GetSinglePizza(int id, string storedProcedureToShowSinglePizza = "GetSpecificPizza")
        {
            return (await connection.QueryAsync<Pizza>(storedProcedureToShowSinglePizza, new { PizzaID = id }, commandType: CommandType.StoredProcedure)).First();
        }

        public async Task UpdatePizza(Pizza pizza, string storedProcedureToUpdatePizza = "UpdatePizzaByID")
        {
            await connection.QueryAsync<Pizza>(storedProcedureToUpdatePizza, new { PizzaID = pizza.PizzaID, Type = pizza.Type, Price = pizza.Price, Base = pizza.Base, Ingredients = JsonConvert.SerializeObject(pizza.PizzaIngredients) }, commandType: CommandType.StoredProcedure);
        } //Tillagt SP

        public async Task DeletePizza(int id, string storedProcedureToDeletePizza = "DeletePizzaByID")
        {
            await connection.QueryAsync<Pizza>(storedProcedureToDeletePizza, new { PizzaID = id }, commandType: CommandType.StoredProcedure);
        }
        public async Task<IEnumerable<Condiment>> GetIngredientsFromSpecificPizza(int id, string storedProcedureToGetIngredientFromSpecifikPizza = "GetIngredientsFromSpecificPizza")
        {
            return (await connection.QueryAsync<Condiment>(storedProcedureToGetIngredientFromSpecifikPizza, new { PizzaID = id }, commandType: CommandType.StoredProcedure));
        }

        //Condiments
        public async Task AddCondiment(Condiment cond, string storedProcedureToAddCondiment = "AddCondiment")
        {
            await connection.QueryAsync<Condiment>(storedProcedureToAddCondiment, new { Type = cond.Type, Price = cond.Price }, commandType: CommandType.StoredProcedure);
        } //Tillagt SP

        public async Task<IEnumerable<Condiment>> GetAllCondiments(string storedProcedureToShowCondiment = "GetAllCondiments")
        {
            return (await connection.QueryAsync<Condiment>(storedProcedureToShowCondiment, commandType: CommandType.StoredProcedure));
        }

        public async Task<Condiment> GetSingleCondiment(int id, string storedProcedureToShowSingleCondiment = "GetSingleCondiment")
        {
            return (await connection.QueryAsync<Condiment>(storedProcedureToShowSingleCondiment, new { CondimentID = id }, commandType: CommandType.StoredProcedure)).First();
        }

        public async Task UpdateCondiment(Condiment cond, string storedProcedureToUpdateCondiment = "UpdateCondimentByID") //Tillagt SP
        {
            await connection.QueryAsync<Condiment>(storedProcedureToUpdateCondiment, new { CondimentID = cond.CondimentID, Type = cond.Type, Price = cond.Price, }, commandType: CommandType.StoredProcedure);
        }

        public async Task DeleteCondiment(int id, string storedProcedureToDeleteCondiment = "DeleteCondimentByID")
        {
            await connection.QueryAsync<Condiment>(storedProcedureToDeleteCondiment, new { CondimentID = id }, commandType: CommandType.StoredProcedure);
        } //Tillagt SP   

        //Extras
        public async Task AddExtra(Extra extra, string storedProcedureToAddExtra = "AddExtra")
        {
            await connection.QueryAsync<Extra>(storedProcedureToAddExtra, new { Type = extra.Type, Price = extra.Price }, commandType: CommandType.StoredProcedure);
        } //Tillagt SP

        public async Task<IEnumerable<Extra>> GetAllExtras(string storedProcedureToShowExtra = "GetAllExtras")
        {
            return (await connection.QueryAsync<Extra>(storedProcedureToShowExtra, commandType: CommandType.StoredProcedure));
        }

        public async Task<Extra> GetSingleExtra(int id, string storedProcedureToShowSingleExtra = "GetSingleExtra")
        {
            return (await connection.QueryAsync<Extra>(storedProcedureToShowSingleExtra, new { ProductID = id }, commandType: CommandType.StoredProcedure)).First();
        } //Tillagt SP

        public async Task UpdateExtra(Extra extra, string storedProcedureToUpdateExtra = "UpdateExtraByID")
        {
            await connection.QueryAsync<Extra>(storedProcedureToUpdateExtra, new { ProductID = extra.ProductID, Type = extra.Type, Price = extra.Price, }, commandType: CommandType.StoredProcedure);
        } //Tillagt SP

        public async Task DeleteExtra(int id, string storedProcedureToDeleteExtra = "DeleteExtraByID")
        {
            await connection.QueryAsync<Extra>(storedProcedureToDeleteExtra, new { ProductID = id }, commandType: CommandType.StoredProcedure);
        } //Tillagt SP


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
    }
    #endregion

}

