using System;

namespace PizzaClassLibrary
{
    public class Condiments
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

        //Ska vi ha ovverride till alla klasser?
        public override string ToString()
        {
            return $"{this.ProductID} {this.Type} {this.Price}";
        }
    }

    public class OldOrders
    {
        public int OldOrderID { get; }
    }

    public class Orders
    {
        //Överensstämmer med databasen
        public int OrderID { get; }
        public string Pizzas { get; set; }
        public string Extras { get; set; }
        public decimal Price { get; set; }
    }
    public class Pizzas
    {
        //Överensstämmer med databasen
        public int PizzaID { get; }
        public string Type { get; set; }
        public float Price { get; set; }
        public string Base { get; set; }
        public string Ingredients { get; set; }

        //Ska vi ha ovverride till alla klasser?
        public override string ToString()
        {
            return $"{this.PizzaID} {this.Type} {this.Price} {this.Base} {this.Ingredients}";
        }
    }
}
