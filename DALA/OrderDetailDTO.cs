using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DALA
{
    public class OrderDetailDTO
    {
        public int Order_Id { get; set; }
        public int Amount { get; set; }
        public DateTime Date { get; set; }
        public List<OrderProductDTO> Products { get; set; }
        public OrderDetailDTO()
        {
            Products = new List<OrderProductDTO>();
        }

    }
}
