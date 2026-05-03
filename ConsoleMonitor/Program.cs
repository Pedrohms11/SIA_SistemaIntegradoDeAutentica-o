using ConsoleMonitor.Data;
using ConsoleMonitor.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using SIA_SistemaIntegradoDeAutenticação;

class Program
{
    static async Task Main(string[] args)
    {
        
        // Configurar DI
        var services = new ServiceCollection();
        services.AddDbContext<MonitorDbContext>(static options =>
            options.UseSqlite("Data Source=InterfaceUs.db"));

        services.AddSingleton<BackupService>();
        services.AddSingleton<MonitorService>();

        var serviceProvider = services.BuildServiceProvider();
        Console.Title = "Monitor de Usuários - SIA Sistema Integrado";
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine(@"
╔══════════════════════════════════════════════════════════════╗
║     SISTEMA DE MONITORAMENTO DE USUÁRIOS - TEMPO REAL       ║
║                                                              ║
║  Monitorando alterações na base de usuários...              ║
║  Pressione 'Q' para sair | 'B' para backup manual          ║
╚══════════════════════════════════════════════════════════════╝
");
        Console.ResetColor();
        Console.WriteLine();

        // Iniciar monitoramento

        var monitor = serviceProvider.GetRequiredService<MonitorService>();
        var backupService = serviceProvider.GetRequiredService<BackupService>();

        // Iniciar monitoramento em background
        var cts = new CancellationTokenSource();
        var monitorTask = monitor.IniciarMonitoramento(cts.Token);

        // Configurar backup automático (a cada 1 hora)
        var backupTask = backupService.IniciarBackupAutomatico(TimeSpan.FromHours(1), cts.Token);

        // Loop para comandos do usuário
        while (true)
        {
            if (Console.KeyAvailable)
            {
                var key = Console.ReadKey(true).Key;
                if (key == ConsoleKey.Q)
                {
                    Console.WriteLine("\n🛑 Encerrando monitoramento...");
                    cts.Cancel();
                    break;
                }
                else if (key == ConsoleKey.B)
                {
                    await backupService.RealizarBackup();
                }
            }
            await Task.Delay(100);
        }

        await Task.WhenAll(monitorTask, backupTask);
        Console.WriteLine("✅ Monitoramento encerrado com sucesso!");
    }
}

