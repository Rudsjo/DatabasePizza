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
            return (await connection.QueryAsync<Employees>(storedProcedureToShowEmployees, commandType: CommandType.StoredProcedure));
        }

        public async Task<Employees> ShowSingleEmployee(int ID, string storedProcedureToShowSingleEmployee)
        {
            return (await connection.QueryAsync<Employees>(storedProcedureToShowSingleEmployee, new { UserID = ID }, commandType: CommandType.StoredProcedure)).First();
        }

        public async Task UpdateEmployee(string storedProcedureToUpdateEmployee, Employees employee)
        {
            await connection.QueryAsync<Employees>(storedProcedureToUpdateEmployee, new { UserID = employee.UserID, Password = employee.Password, Role = employee.Role }, commandType: CommandType.StoredProcedure);
        }

        public async Task DeleteEmployee(int ID, string storedProcedureToDeleteEmployee)
        {
            await connection.QueryAsync<Employees>(storedProcedureToDeleteEmployee, new { UserID = ID }, commandType: CommandType.StoredProcedure);
        }
    
        //Pizzas

    }
}
