﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace PizzaClassLibrary
{
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
    
}