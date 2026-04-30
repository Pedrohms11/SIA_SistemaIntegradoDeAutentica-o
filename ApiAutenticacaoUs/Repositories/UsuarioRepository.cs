using ApiAutenticacaoUs.Data;
using ApiAutenticacaoUs.Repositories.Interfaces;
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

        public async Task<List<Usuarios>> GetAll() => await _context.Usuarios.ToListAsync();

        public async Task<Usuarios> GetById(int id) => await _context.Usuarios.FindAsync(id);

        public async Task add(Usuarios usuarios)
        {
            _context.Usuarios.Add(usuarios);
            await _context.SaveChangesAsync();
        }
         
        public async Task Update (Usuarios usuarios)
        {
            _context.Usuarios.Update(usuarios);
            await _context.SaveChangesAsync();

        }

        public async Task Delete(int id)
        {
            var u = await GetById(id);
            _context.Usuarios.Remove(u);
            await _context.SaveChangesAsync();
        }
    }
}
