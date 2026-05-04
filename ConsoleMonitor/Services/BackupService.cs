using ConsoleMonitor.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.IO.Compression;
using System.Text.Json;

namespace ConsoleMonitor.Services
{
    public class BackupService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly string _backupFolder;

        public BackupService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            _backupFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Backups");

            if (!Directory.Exists(_backupFolder))
                Directory.CreateDirectory(_backupFolder);
        }

        public async Task IniciarBackupAutomatico(TimeSpan intervalo, CancellationToken cancellationToken)
        {
            Console.WriteLine("💾 Sistema de backup automático ativado (a cada 1 hora)\n");

            while (!cancellationToken.IsCancellationRequested)
            {
                await Task.Delay(intervalo, cancellationToken);
                await RealizarBackup();
            }
        }

        public async Task RealizarBackup()
        {
            try
            {
                var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
                var backupNome = $"Backup_Usuarios_{timestamp}.db";
                var backupPath = Path.Combine(_backupFolder, backupNome);
                var zipPath = Path.Combine(_backupFolder, $"Backup_Usuarios_{timestamp}.zip");

                // Criar um escopo para obter o DbContext
                using var scope = _serviceProvider.CreateScope();
                var context = scope.ServiceProvider.GetRequiredService<MonitorDbContext>();

                // Fazer backup dos dados via JSON
                var usuarios = await context.Usuarios.ToListAsync();
                var historicos = await context.HistoricoAlteracoes.ToListAsync();

                var backupData = new
                {
                    DataBackup = DateTime.Now,
                    Usuarios = usuarios,
                    HistoricoAlteracoes = historicos,
                    TotalUsuarios = usuarios.Count,
                    TotalAlteracoes = historicos.Count
                };

                var json = JsonSerializer.Serialize(backupData, new JsonSerializerOptions
                {
                    WriteIndented = true
                });

                await File.WriteAllTextAsync(backupPath, json);

                // Compactar o arquivo
                using (var zipArchive = ZipFile.Open(zipPath, ZipArchiveMode.Create))
                {
                    zipArchive.CreateEntryFromFile(backupPath, Path.GetFileName(backupPath));
                }

                // Remover o JSON não compactado
                File.Delete(backupPath);

                // Manter apenas os últimos 10 backups
                var backups = Directory.GetFiles(_backupFolder, "*.zip")
                    .OrderByDescending(f => File.GetCreationTime(f))
                    .Skip(10);

                foreach (var backup in backups)
                    File.Delete(backup);

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] 💾 Backup automático realizado com sucesso!");
                Console.WriteLine($"   📁 Arquivo: {Path.GetFileName(zipPath)}");
                Console.WriteLine($"   📊 Usuários: {usuarios.Count} | Alterações: {historicos.Count}");
                Console.ResetColor();
                Console.WriteLine();
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] ❌ Erro no backup: {ex.Message}");
                Console.ResetColor();
            }
        }
    }
}