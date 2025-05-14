using cp.commands;
using cp.models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cp.viewModels
{
    class OrderDetailsViewModel
    {
        public Order Order {  get; set; }
        public OrderDetailsViewModel(Order order)
        {
            Order = order;
        }
    }
}
