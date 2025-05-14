using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cp.models
{
    public class Cart
    {
        public int Id { get; set; }

        public int UserId { get; set; }
        public User User { get; set; }

        public ICollection<CartItem> Items { get; set; }
        public decimal TotalPrice => Items?.Sum(i => i.Flower != null ? i.Flower.Price * i.Quantity : 0) ?? 0;
    }
}
