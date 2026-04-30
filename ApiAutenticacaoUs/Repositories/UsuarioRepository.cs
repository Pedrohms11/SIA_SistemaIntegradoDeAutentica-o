using ApiAutenticacaoUs.Data;
using ApiAutenticacaoUs.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using SIA_SistemaIntegradoDeAutenticação;

namespace ApiAutenticacaoUs.Repositories
{
    public class UsuarioRepository : IUsuarioRepository
    {
        private readonly AppDbContext _context;

        public UsuarioRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Usuarios>> GetAll() => await _context.Usuario.ToListAsync();

        public async Task<Usuarios> GetById(int id) => await _context.Usuario.FindAsync(id);

        public async Task Add(Usuarios usuario)
        {
            _context.Usuario.Add(usuario);
            await _context.SaveChangesAsync();
        }
         
        public async Task Update (Usuarios usuarios)
        {
            _context.Usuario.Update(usuarios);
            await _context.SaveChangesAsync();

        }

        public async Task Delete(int id)
        {
            var u = await GetById(id);
            _context.Usuario.Remove(u);
            await _context.SaveChangesAsync();
        }
    }
}
