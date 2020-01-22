using System;
using System.Data.SqlClient;

namespace ConnnectionConfig
{

    /// <summary>
    /// Används för att öppna en connection mot MSSDS
    /// </summary>
    public class MSSQLConnection
    {
        private string ConnectionString { get; set; }

        private SqlConnection connection { get; }

        public MSSQLConnection()
        {
            string ConnectionString = "Data Source = sql6009.site4now.net; Initial Catalog = DB_A53DDD_grupp5; User ID = DB_A53DDD_grupp5_admin; Password = grupp5pizza"; //Ändrat till litet g i grupp på databas och användarnamn
            connection = new SqlConnection(ConnectionString);
            connection.Open();
        }
    }
    public class PostgreSQLConnection
    {

    }
}
