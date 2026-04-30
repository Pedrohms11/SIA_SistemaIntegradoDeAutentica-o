using SIA_SistemaIntegradoDeAutenticação;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace InterfaceDeUsuarios.Data
{
    public class AppdbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<Usuarios> Usuarios { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configurações adicionais
            modelBuilder.Entity<Usuarios>()
                .HasIndex(u => u.Email)
                .IsUnique();

            modelBuilder.Entity<Usuarios>()
                .HasIndex(u => u.Username)
                .IsUnique();

            // Valores padrão
            modelBuilder.Entity<Usuarios>()
                .Property(u => u.DataCadastro)
                .HasDefaultValueSql("CURRENT_TIMESTAMP");

            modelBuilder.Entity<Usuarios>()
                .Property(u => u.EmailVerificado)
                .HasDefaultValue(false);

            // Seed data (dados iniciais para teste)
            modelBuilder.Entity<Usuarios>().HasData(
                new Usuarios
                {
                    Id = 1,
                    Username = "admin",
                    NomeCompleto = "Administrador do Sistema",
                    Email = "admin@sistema.com",
                    Senha = "Admin123@",
                    Genero = "Masculino",
                    Telefone = "(11) 99999-9999",
                    Pais = "Brasil",
                    DataNascimento = new DateTime(1980, 1, 1),
                    DataCadastro = DateTime.Now,
                    UltimoLogin = DateTime.Now,
                    EmailVerificado = true
                },
                new Usuarios
                {
                    Id = 2,
                    Username = "joaosilva",
                    NomeCompleto = "João Silva",
                    Email = "joao@email.com",
                    Senha = "Joao123@",
                    Genero = "Masculino",
                    Telefone = "(11) 98888-7777",
                    Pais = "Brasil",
                    DataNascimento = new DateTime(1995, 5, 15),
                    DataCadastro = DateTime.Now,
                    UltimoLogin = DateTime.Now.AddDays(-5),
                    EmailVerificado = true
                },
                new Usuarios
                {
                    Id = 3,
                    Username = "maria_santos",
                    NomeCompleto = "Maria Santos",
                    Email = "maria@email.com",
                    Senha = "Maria123@",
                    Genero = "Feminino",
                    Telefone = "(11) 97777-6666",
                    Pais = "Portugal",
                    DataNascimento = new DateTime(1998, 8, 20),
                    DataCadastro = DateTime.Now,
                    UltimoLogin = DateTime.Now.AddDays(-2),
                    EmailVerificado = false
                }
            );
        }
    }

}
}
