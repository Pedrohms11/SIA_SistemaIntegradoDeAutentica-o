using Microsoft.EntityFrameworkCore;
using SIA_SistemaIntegradoDeAutenticação;

namespace ApiAutenticacaoUs.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<Usuarios> Usuario { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options) { }
    }
}

