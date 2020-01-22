using System;

namespace PizzaClassLibrary
{
    public class Condiments : ICondiments
    {
        //Överensstämmer med databasen

        public int CondimentID { get; }

        public string Type { get; set; }

        public float Price { get; set; }
    }

    public class Employees
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

    public class Extras
    {
        //Överensstämmer med databasen

        public int ProductID { get; }

        public string Type { get; set; }

        public float Price { get; set; }
    }

    public class OldOrders
    {
        public int ID { get; }
    }

    public class Pizzas
    {
        //Överensstämmer med databasen

        public int PizzaID { get; }

        public string Type { get; set; }

        public float Price { get; set; }

        public string Base { get; set; }

        public string Ingredients { get; set; }
    }
}
