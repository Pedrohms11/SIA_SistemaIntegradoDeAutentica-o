using SIA_SistemaIntegradoDeAutenticação;


namespace ConsoleMonitor.Services
{
    public class MonitorDbContext : DbContext
    {
        public MonitorDbContext(DbContextOptions<MonitorDbContext> options) : base(options)
        {
        }

        public DbSet<Usuarios> Usuarios { get; set; }
        public DbSet<HistoricoAlteracao> HistoricoAlteracoes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configurar índices para melhor performance
            modelBuilder.Entity<Usuario>()
                .HasIndex(u => u.UltimaModificacao);

            modelBuilder.Entity<HistoricoAlteracao>()
                .HasIndex(h => h.DataHora);

            modelBuilder.Entity<HistoricoAlteracao>()
                .HasIndex(h => new { h.UsuarioId, h.DataHora });
        }
    }

}
}
