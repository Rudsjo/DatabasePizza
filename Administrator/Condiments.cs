using System;
using System.Collections.Generic;
using System.Text;

namespace Administrator
{
    public class Condiments
    {
        //Överensstämmer med databasen

        public int CondimentID { get; }

        public string Type { get; set; }

        public int Price { get; set; }

    }
}
