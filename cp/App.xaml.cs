using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using System.IO;
using System.Windows;
using cp.views;
using cp.models;
using cp.viewModels;
using Microsoft.Extensions.Hosting;
using System.Globalization;

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

            using (var scope = _host.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<FlowerShopDbContext>();
                db.Database.Migrate();
            }

            var mainWindow = _host.Services.GetRequiredService<MainWindow>();
            mainWindow.Show();
        }

        protected override async void OnExit(ExitEventArgs e)
        {
            await _host.StopAsync();
            _host.Dispose();
            base.OnExit(e);
        }

        public event EventHandler LanguageChanged;

        public void LoadLanguageDictionary(string culture)
        {
            var ci = new CultureInfo(culture);
            Thread.CurrentThread.CurrentCulture = ci;
            Thread.CurrentThread.CurrentUICulture = ci;

            var dictUri = new Uri($"/cp;component/localization/Resources.{culture}.xaml", UriKind.Relative);
            var langDict = new ResourceDictionary() { Source = dictUri };

            var merged = Resources.MergedDictionaries;
            if (merged.Count > 0)
                merged[0] = langDict;
            else
                merged.Add(langDict);

            LanguageChanged?.Invoke(this, EventArgs.Empty);
        }
    }

}
