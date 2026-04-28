using SIA_SistemaIntegradoDeAutenticação;

namespace ApiAutenticacao.Repositories.Interfaces
{
    public interface IUsuarioRepository
    {
        /// <summary>
        /// interface IUsuarioRepository - define os métodos 
        /// que o repostório de usuario deve implementar
        /// </summary>
        public interface IUsuarioRepository
        {
            Task<List<Usuarios>> GetAll();
            Task<Usuarios> GetById(int id);
            Task Add(Usuarios usuario);
            Task Update(Usuarios usuario);
            Task Delete(int id);

        }
    }
}
