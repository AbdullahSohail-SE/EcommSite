using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DAL.Models
{
    public class ProductDTO
    {
        public int product_id { get; set; }
       
        public string name { get; set; }
     
        public double price { get; set; }
  
        public string category { get; set; }
       
        public string description { get; set; }
    }
}