using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cp.models
{
    public class OrderItem
    {
        public int Id { get; set; }

        public int OrderId { get; set; }
        public Order Order { get; set; }

        public int FlowerId { get; set; }
        public Flower Flower { get; set; }

        public int Quantity { get; set; }
        public decimal Price { get; set; }
    }
}
