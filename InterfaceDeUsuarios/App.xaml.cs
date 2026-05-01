using ApiAutenticacaoUs.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;
using System.Windows;

namespace InterfaceDeUsuarios
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static IConfiguration Configuration { get; private set; }
        public static ServiceProvider ServiceProvider { get; private set; }

        // Flag para identificar se está em modo de design/migration
        public static bool IsDesignTime { get; private set; }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            try
            {
                // Configurar Configuration
                var builder = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

                Configuration = builder.Build();

                // Configurar DI apenas se NÃO estiver em modo de design
                if (!IsDesignTime)
                {
                    var services = new ServiceCollection();
                    services.AddDbContext<AppDbContext>(options =>
                        options.UseSqlite(Configuration.GetConnectionString("DefaultConnection")));

                    ServiceProvider = services.BuildServiceProvider();

                    // Aplicar migrações apenas em modo normal (não durante migrations)
                    using (var scope = ServiceProvider.CreateScope())
                    {
                        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                        dbContext.Database.EnsureCreated();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro na inicialização: {ex.Message}", "Erro",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}