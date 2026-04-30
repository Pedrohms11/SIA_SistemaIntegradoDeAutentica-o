using ApiAutenticacaoUs.Repositories.Interfaces;
using SIA_SistemaIntegradoDeAutenticação;

namespace ApiAutenticacaoUs.Services
{
    public class UsuarioServices
    {
        private readonly IUsuarioRepository _repo;

        public UsuarioServices(IUsuarioRepository repo)
        {
            _repo = repo;
        }

        public async Task<List<Usuarios>> Listar() => await _repo.GetAll();

        public async Task<Usuarios> ObterPorId(int id) => await _repo.GetById(id);

        public async Task Criar(Usuarios usuario) => await _repo.Add(usuario);

        public async Task Atualizar(Usuarios usuario) => await _repo.Update(usuario);

        public async Task Deletar(int id) => await _repo.Delete(id);

    }
}
