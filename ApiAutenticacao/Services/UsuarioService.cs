using ApiAutenticacao.Repositories.Interfaces;
using SIA_SistemaIntegradoDeAutenticação;

namespace ApiAutenticacao.Services
{
    public class UsuarioService
    {
        /// <summary>
        /// repository de produtos - responsável por acessar os 
        /// dados dos produtos no banco de dados
        /// </summary>

        private readonly IUsuarioRepository _repo;

        /// <summary>
        /// construtor da classe - recebe o repository de produtos
        /// via injeção de dependência
        /// </summary>
        /// <param name="repo"></param>

        public UsuarioService(IUsuarioRepository repo)
        {
            _repo = repo;
        }

        /// <summary>
        /// listar todos os produtos - chmaa o metódo GetAll do 
        /// repository para obter a lista de produtos do banco de dados
        /// </summary>
        /// <returns></returns>

        public async Task<List<Usuarios>> Listar() => await _repo.Listar();

        /// <summary>
        /// obter um produto por id - chama o método GetById
        /// do repository para obter um produto específico
        /// do banco de dados com base no id fornecido
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>

        public async Task<Usuarios> ObterPorId(int id) => await _repo.ObterPorId(id);

        /// <summary>
        /// Criar um novo usuario - Chama o método Add
        /// do repository para adicionar um novo produto ao banco de dados
        /// </summary>
        /// <param name="produto"></param>
        /// <returns></returns>

        public async Task Criar(Usuarios usuario) => await _repo.Add(usuario);

        /// <summary>
        /// Atualizar um produto existente - chama
        /// o método Update do repository
        /// </summary>
        /// <param name="produto"></param>
        /// <returns></returns>
        public async Task Atualizar(int id, Usuarios usuario)
        {


            await _repo.Atualizar(usuario);
        }

        /// <summary>
        /// Deletar um produto por id -  Chama
        /// o método Delete do repository
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task Deletar(int id) => await _repo.Delete(id);

    }
}

