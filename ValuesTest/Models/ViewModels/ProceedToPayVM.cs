using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DALA;
using System.Data;
using System.ComponentModel.DataAnnotations;

namespace Web.Models.ViewModels
{
    public class ProceedToPayVM :BaseModel
    {
        public List<OrderProductDTO> productsList { get; set; }
        public int? Total { get; set; }
        
        public List<AddressDTO> addressList { get; set; }
        [Required]
        
        public int Id { get; set; }


    }
}