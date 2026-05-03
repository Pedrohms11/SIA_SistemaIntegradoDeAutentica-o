using ConsoleMonitor.Data;
using Microsoft.EntityFrameworkCore;
using System.IO.Compression;

namespace ConsoleMonitor.Services
{
    public class BackupService
    {
        private readonly IDbContextFactory<MonitorDbContext> _contextFactory;
        private readonly string _backupFolder;

        public BackupService(IDbContextFactory<MonitorDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
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

                // Fazer backup dos dados via JSON (mais seguro que copiar arquivo .db em uso)
                using var context = await _contextFactory.CreateDbContextAsync();

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

                var json = System.Text.Json.JsonSerializer.Serialize(backupData, new System.Text.Json.JsonSerializerOptions
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

        public async Task RestaurarBackup(string arquivoZip)
        {
            // Implementar restauração se necessário
            Console.WriteLine("Função de restauração em desenvolvimento...");
        }
    }
}