using System;
using System.Collections.Generic;
using System.Text;
using Dapper;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;
using System.Data;

namespace Administrator
{
    public class Repository
    {
        private string ConnectionString { get; set; }

        private SqlConnection connection { get; }

        public Repository()
        {
            string ConnectionString = "Data Source = sql6009.site4now.net; Initial Catalog = DB_A53DDD_Grupp5; User ID = DB_A53DDD_Grupp5_admin; Password = grupp5pizza";
            connection = new SqlConnection(ConnectionString);
            connection.Open();
        }

        



    }
}
