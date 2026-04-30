using ApiAutenticacao.Repositories.Interfaces;
using SIA_SistemaIntegradoDeAutenticação;

namespace ApiAutenticacao.Services
{
    public class UsuarioService
    {
        private readonly IUsuarioRepository _repo;

        public UsuarioService(IUsuarioRepository repo)
        {
            _repo = repo;
        }

        public async Task<List<Usuarios>> Listar() => await _repo.Listar();

        public async Task<Usuarios> ObterPorId(int id) => await _repo.ObterPorId(id);

        public async Task Criar(Usuarios usuario) => await _repo.Add(usuario);

        // CORRIGIDO: Agora retorna Usuarios? (nullable)
        public async Task<Usuarios?> Atualizar(int id, Usuarios usuario)
        {
            // Verifica se o usuário existe antes de atualizar
            var usuarioExistente = await _repo.ObterPorId(id);
            if (usuarioExistente == null)
            {
                return null; // Retorna null se não encontrado
            }

            // Garante que o ID do usuário a ser atualizado é o correto
            usuario.Id = id;
            await _repo.Atualizar(usuario);
            return usuario; // Retorna o usuário atualizado
        }

        public async Task Deletar(int id) => await _repo.Delete(id);

        public async Task<List<Usuarios>> GetAutenticacaoDataAsync()
        {
            try
            {
                return await _repo.Listar();
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao obter dados de autenticação: {ex.Message}", ex);
            }
        }

        public async Task<Usuarios> GetAutenticacaoDataByIdAsync(int usuarioId)
        {
            try
            {
                var usuario = await _repo.ObterPorId(usuarioId);

                if (usuario == null)
                {
                    return null; // Retorna null se não encontrado
                }

                return usuario;
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao obter dados de autenticação do usuário {usuarioId}: {ex.Message}", ex);
            }
        }

        public async Task<Usuarios> CreateAutenticacaoDataAsync(Usuarios usuario)
        {
            try
            {
                var usuariosExistentes = await _repo.Listar();
                if (usuariosExistentes.Any(u => u.Email == usuario.Email))
                {
                    throw new Exception($"Email {usuario.Email} já está em uso.");
                }

                await _repo.Add(usuario);
                return usuario;
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao criar dados de autenticação: {ex.Message}", ex);
            }
        }

        // IMPLEMENTADO: Método para atualizar autenticação
        public async Task<Usuarios?> UpdateAutenticacaoDataAsync(int id, Usuarios usuario)
        {
            return await Atualizar(id, usuario);
        }

        // IMPLEMENTADO: Método para deletar autenticação
        public async Task<bool> DeleteAutenticacaoDataAsync(int id)
        {
            try
            {
                var usuario = await _repo.ObterPorId(id);
                if (usuario == null)
                {
                    return false; // Usuário não encontrado
                }

                await _repo.Delete(id);
                return true; // Excluído com sucesso
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao deletar dados de autenticação: {ex.Message}", ex);
            }
        }
    }
}