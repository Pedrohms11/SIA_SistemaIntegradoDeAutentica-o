using ApiAutenticacaoUs.Data;
using Microsoft.EntityFrameworkCore;
using SIA_SistemaIntegradoDeAutenticação;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace InterfaceDeUsuarios.Data
{
    public class AppDbContext : DbContext
    {
        // O nome dentro do < > DEVE ser idêntico ao nome desta classe
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Usuarios> Usuarios { get; set; }        
    }

}

