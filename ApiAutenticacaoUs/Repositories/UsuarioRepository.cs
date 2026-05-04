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

        public async Task<List<Usuarios>> GetAll()
        {
            return await _context.Usuario
                .OrderBy(u => u.NomeCompleto)
                .ToListAsync();
        }

        public async Task<Usuarios> GetById(int id)
        {
            return await _context.Usuario
                .FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task Add(Usuarios usuario)
        {
            usuario.DataCadastro = DateTime.Now;
            usuario.UltimoLogin = DateTime.Now;
            usuario.UltimaModificacao = DateTime.Now;
            usuario.EmailVerificado = false;

            await _context.Usuario.AddAsync(usuario);
            await _context.SaveChangesAsync();
        }

        public async Task Update(Usuarios usuario)
        {
            usuario.UltimaModificacao = DateTime.Now;
            _context.Usuario.Update(usuario);
            await _context.SaveChangesAsync();
        }

        public async Task Delete(int id)
        {
            var usuario = await GetById(id);
            if (usuario != null)
            {
                _context.Usuario.Remove(usuario);
                await _context.SaveChangesAsync();
            }
        }
    }
}
