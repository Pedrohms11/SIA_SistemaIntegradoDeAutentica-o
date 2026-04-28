using Microsoft.EntityFrameworkCore;
using SIA_SistemaIntegradoDeAutenticação;

namespace ApiAutenticacao.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        { }
             public DbSet<Usuarios> Usuarios { get; set; }
    
    }
}
