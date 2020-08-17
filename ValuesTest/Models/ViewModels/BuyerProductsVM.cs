using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DALA;

namespace Web.Models.ViewModels
{
    public class BuyerProductsVM :BaseModel
    {
        public List<ProductDTO> ProductsList { get; set; }
        public List<string> Categories { get; set; }

        public int TotalPages { get; set; }


    }
}