using SIA_SistemaIntegradoDeAutenticação;

namespace ApiAutenticacao.Repositories.Interfaces
{
    public interface IUsuarioRepository
    {
        /// <summary>
        /// interface IUsuarioRepository - define os métodos 
        /// que o repostório de usuario deve implementar
        /// </summary>
        
            Task<List<Usuarios>> Listar();
            Task<Usuarios> ObterPorId(int id);
            Task Add(Usuarios usuario);
            Task Atualizar(Usuarios usuario);
            Task Delete(int id);

        
    }
}
