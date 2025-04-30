using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using System.IO;
using System.Windows;
using cp.views;
using cp.models;
using cp.viewModels;
using Microsoft.Extensions.Hosting;

namespace cp
{
    public partial class App : Application
    {
        private IHost _host;

        public App()
        {
            _host = Host.CreateDefaultBuilder()
                .ConfigureAppConfiguration((ctx, cfg) =>
                {
                    cfg.SetBasePath(AppContext.BaseDirectory)
                       .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
                })
                .ConfigureServices((ctx, services) =>
                {
                    services.AddDbContext<FlowerShopDbContext>(options =>
                        options.UseNpgsql(ctx.Configuration.GetConnectionString("FlowerShopDb")));

                    services.AddTransient<ProfileViewModel>();
                    services.AddTransient<ProfilePage>();
                    services.AddSingleton<MainWindow>();
                })
                .Build();
        }

        private async void OnStartup(object sender, StartupEventArgs e)
        {
            await _host.StartAsync();

            //using (var scope = _host.Services.CreateScope())
            //{
            //    var db = scope.ServiceProvider.GetRequiredService<FlowerShopDbContext>();
            //    db.Database.Migrate();
            //}

            var mainWindow = _host.Services.GetRequiredService<MainWindow>();
            mainWindow.Show();
        }

        protected override async void OnExit(ExitEventArgs e)
        {
            await _host.StopAsync();
            _host.Dispose();
            base.OnExit(e);
        }
    }
}
