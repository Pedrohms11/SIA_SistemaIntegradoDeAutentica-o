using Microsoft.EntityFrameworkCore;
using SIA_SistemaIntegradoDeAutenticação;

namespace ApiAutenticacao.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<Usuarios> Usuarios { get; set; }
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        { }
             
    
    }
}
