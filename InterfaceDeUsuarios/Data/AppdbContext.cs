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
    public class AppdbContext : DbContext
    {
        public AppdbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<Usuarios> Usuarios { get; set; }        
    }

}

