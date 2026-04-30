using SIA_SistemaIntegradoDeAutenticação;

namespace ApiAutenticacaoUs.Repositories.Interfaces
{
    public interface IUsuarioRepository
    {
        Task<List<Usuarios>> GetAll();
        Task<Usuarios> GetById(int id);
        Task Add(Usuarios usuarios);
        Task Update(Usuarios usuarios);
        Task Delete(int id);
    }
}
