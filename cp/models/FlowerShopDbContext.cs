using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;

namespace cp.models
{
    public class FlowerShopDbContext : DbContext
        {
        public FlowerShopDbContext(DbContextOptions<FlowerShopDbContext> options) : base(options) { }
        public DbSet<Flower> Flowers { get; set; }
        public DbSet<User> Users { get; set; }

    }
  
}
