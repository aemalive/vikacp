using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;

namespace cp.models
{
    public class FlowerShopDbContext : DbContext
    {
        public FlowerShopDbContext(DbContextOptions<FlowerShopDbContext> options)
            : base(options)
        {
        }

        public FlowerShopDbContext()
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Flower> Flowers { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                var config = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                    .Build();

                var connectionString = config.GetConnectionString("FlowerShopDb");
                optionsBuilder.UseNpgsql(connectionString);
            }
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>().HasData(
                new User
                {
                    Id = 1,
                    Username = "admin",
                    Password = "admin",
                    FullName = "Administrator",
                    Role = "ADMIN",
                    Email = "admin@flowershop.local"
                },
                new User
                {
                    Id = 2,
                    Username = "user",
                    Password = "1234",
                    FullName = "User",
                    Role = "USER",
                    Email = "user@flowershop.local"
                }
            );

            modelBuilder.Entity<Flower>().HasData(
                new Flower
                {
                    Id = 1,
                    Name = "flower1",
                    Price = 123,
                    Description = "Пример цветка",
                    ImageURL = "https://example.com/image.jpg"
                }
            );

        }

    }
}
