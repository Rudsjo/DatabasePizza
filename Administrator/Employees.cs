using System;
using System.Collections.Generic;
using System.Text;

namespace Administrator
{
    public class Employees
    {

        //Överensstämmer med databasen
        public int UserID { get; }

        public string Password { get; set; }

        public string Role { get; set; }

    }
}
