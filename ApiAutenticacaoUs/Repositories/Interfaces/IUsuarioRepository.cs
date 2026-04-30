using SIA_SistemaIntegradoDeAutenticação;

namespace ApiAutenticacaoUs.Repositories.Interfaces
{
    public interface IUsuarioRepository
    {
        Task<List<Usuarios>> GetAll();
        Task<Usuarios> GetById(int id);
        Task Add(Usuarios usuario);
        Task Update(Usuarios usuario);
        Task Delete(int id);
    }
}
