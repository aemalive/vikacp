using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cp.models
{
    public class Order
    {
        public int Id { get; set; }
        public string Address { get; set; }
        public string DeliveryTime { get; set; }
        public string Comment { get; set; }

        public decimal TotalAmount { get; set; }

        public int UserId { get; set; }
        public User User { get; set; }

        public ICollection<OrderItem> OrderItems { get; set; }
    }
}
