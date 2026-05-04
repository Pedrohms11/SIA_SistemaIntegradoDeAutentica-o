using ConsoleMonitor.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SIA_SistemaIntegradoDeAutenticação;
using System.Text.Json;

namespace ConsoleMonitor.Services
{
    public class MonitorService
    {
        private readonly IServiceProvider _serviceProvider;
        private List<Usuarios> _cacheUsuarios;
        private DateTime _ultimaVerificacao;
        private readonly object _lock = new object();

        public MonitorService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            _cacheUsuarios = new List<Usuarios>();
        }

        public async Task IniciarMonitoramento(CancellationToken cancellationToken)
        {
            // Criar um escopo para cada operação
            using var scope = _serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<MonitorDbContext>();

            await CarregarCacheInicial(context);

            while (!cancellationToken.IsCancellationRequested)
            {
                using var newScope = _serviceProvider.CreateScope();
                var newContext = newScope.ServiceProvider.GetRequiredService<MonitorDbContext>();
                await VerificarAlteracoes(newContext);
                await Task.Delay(2000, cancellationToken);
            }
        }

        private async Task CarregarCacheInicial(MonitorDbContext context)
        {
            _cacheUsuarios = await context.Usuarios
                .OrderBy(u => u.Id)
                .ToListAsync();

            _ultimaVerificacao = DateTime.Now;
            Console.WriteLine($"📊 Cache inicial carregado: {_cacheUsuarios.Count} usuários");
        }

        private async Task VerificarAlteracoes(MonitorDbContext context)
        {
            var usuariosAtuais = await context.Usuarios
                .Where(u => u.UltimaModificacao > _ultimaVerificacao)
                .OrderBy(u => u.Id)
                .ToListAsync();

            if (usuariosAtuais.Any())
            {
                foreach (var usuario in usuariosAtuais)
                {
                    var usuarioAntigo = _cacheUsuarios.FirstOrDefault(u => u.Id == usuario.Id);
                    await DetectarMudanca(usuarioAntigo, usuario, context);
                }

                // Atualizar cache
                lock (_lock)
                {
                    foreach (var usuario in usuariosAtuais)
                    {
                        var index = _cacheUsuarios.FindIndex(u => u.Id == usuario.Id);
                        if (index >= 0)
                            _cacheUsuarios[index] = usuario;
                        else
                            _cacheUsuarios.Add(usuario);
                    }
                }
            }

            _ultimaVerificacao = DateTime.Now;
        }

        private async Task DetectarMudanca(Usuarios usuarioAntigo, Usuarios usuarioNovo, MonitorDbContext context)
        {
            if (usuarioAntigo == null)
            {
                // NOVO USUÁRIO
                await RegistrarAlteracao(context, usuarioNovo.Id, "CREATE",
                    "Usuário criado", null, usuarioNovo);

                ExibirNotificacao("🟢 NOVO USUÁRIO", usuarioNovo, "criado");
            }
            else if (await UsuarioFoiDeletado(usuarioAntigo.Id, context))
            {
                // USUÁRIO DELETADO
                await RegistrarAlteracao(context, usuarioAntigo.Id, "DELETE",
                    "Usuário excluído", usuarioAntigo, null);

                ExibirNotificacao("🔴 USUÁRIO EXCLUÍDO", usuarioAntigo, "excluído");

                lock (_lock)
                {
                    _cacheUsuarios.RemoveAll(u => u.Id == usuarioAntigo.Id);
                }
            }
            else if (!UsuariosSaoIguais(usuarioAntigo, usuarioNovo))
            {
                // USUÁRIO ALTERADO
                var alteracoes = ObterDetalhesAlteracoes(usuarioAntigo, usuarioNovo);
                await RegistrarAlteracao(context, usuarioNovo.Id, "UPDATE",
                    $"Usuário alterado: {alteracoes}", usuarioAntigo, usuarioNovo);

                ExibirNotificacao("🟡 USUÁRIO ALTERADO", usuarioNovo, alteracoes);
            }
        }

        private async Task<bool> UsuarioFoiDeletado(int id, MonitorDbContext context)
        {
            return !await context.Usuarios.AnyAsync(u => u.Id == id);
        }

        private bool UsuariosSaoIguais(Usuarios u1, Usuarios u2)
        {
            return u1.Username == u2.Username &&
                   u1.NomeCompleto == u2.NomeCompleto &&
                   u1.Email == u2.Email &&
                   u1.Genero == u2.Genero &&
                   u1.Telefone == u2.Telefone &&
                   u1.Pais == u2.Pais &&
                   u1.DataNascimento == u2.DataNascimento &&
                   u1.EmailVerificado == u2.EmailVerificado;
        }

        private string ObterDetalhesAlteracoes(Usuarios antigo, Usuarios novo)
        {
            var alteracoes = new List<string>();

            if (antigo.NomeCompleto != novo.NomeCompleto)
                alteracoes.Add($"Nome: '{antigo.NomeCompleto}' → '{novo.NomeCompleto}'");

            if (antigo.Email != novo.Email)
                alteracoes.Add($"Email: '{antigo.Email}' → '{novo.Email}'");

            if (antigo.Telefone != novo.Telefone)
                alteracoes.Add($"Telefone: '{antigo.Telefone}' → '{novo.Telefone}'");

            if (antigo.Genero != novo.Genero)
                alteracoes.Add($"Gênero: '{antigo.Genero}' → '{novo.Genero}'");

            if (antigo.Pais != novo.Pais)
                alteracoes.Add($"País: '{antigo.Pais}' → '{novo.Pais}'");

            if (antigo.EmailVerificado != novo.EmailVerificado)
                alteracoes.Add($"Email Verificado: {antigo.EmailVerificado} → {novo.EmailVerificado}");

            return string.Join(", ", alteracoes);
        }

        private async Task RegistrarAlteracao(MonitorDbContext context, int usuarioId, string tipoAcao,
            string descricao, Usuarios dadosAntigos, Usuarios dadosNovos)
        {
            var historico = new HistoricoAlteracao
            {
                UsuarioId = usuarioId,
                TipoAcao = tipoAcao,
                DataHora = DateTime.Now,
                Descricao = descricao ?? string.Empty,
                DadosAntigos = dadosAntigos != null ? JsonSerializer.Serialize(dadosAntigos) : null,
                DadosNovos = dadosNovos != null ? JsonSerializer.Serialize(dadosNovos) : null,
                IpOrigem = "127.0.0.1"
            };

            await context.HistoricoAlteracoes.AddAsync(historico);
            await context.SaveChangesAsync();

        }

        private void ExibirNotificacao(string titulo, Usuarios usuario, string detalhes)
        {
            var corOriginal = Console.ForegroundColor;

            Console.WriteLine(new string('─', 80));

            // Definir cor baseada no tipo
            if (titulo.Contains("NOVO"))
                Console.ForegroundColor = ConsoleColor.Green;
            else if (titulo.Contains("EXCLUÍDO"))
                Console.ForegroundColor = ConsoleColor.Red;
            else
                Console.ForegroundColor = ConsoleColor.Yellow;

            Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] {titulo}");
            Console.ResetColor();

            Console.WriteLine($"  👤 ID: {usuario.Id} | Nome: {usuario.NomeCompleto}");
            Console.WriteLine($"  📧 Email: {usuario.Email}");
            Console.WriteLine($"  🏷️  Username: {usuario.Username}");

            if (detalhes != "criado" && detalhes != "excluído")
            {
                Console.ForegroundColor = ConsoleColor.DarkYellow;
                Console.WriteLine($"  📝 Alterações: {detalhes}");
                Console.ResetColor();
            }

            Console.WriteLine(new string('─', 80));
            Console.WriteLine();
        }
    }
}