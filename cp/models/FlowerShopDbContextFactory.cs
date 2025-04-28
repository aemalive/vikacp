using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace cp.models
{
    public class FlowerShopDbContextFactory
        : IDesignTimeDbContextFactory<FlowerShopDbContext>
    {
        public FlowerShopDbContext CreateDbContext(string[] args)
        {
            var cfg = new ConfigurationBuilder()
                      .SetBasePath(Directory.GetCurrentDirectory())
                      .AddJsonFile("appsettings.json")
                      .Build();

            var opts = new DbContextOptionsBuilder<FlowerShopDbContext>()
                       .UseNpgsql(cfg.GetConnectionString("FlowerShopDb"))
                       .Options;

            return new FlowerShopDbContext(opts);
        }
    }
}
