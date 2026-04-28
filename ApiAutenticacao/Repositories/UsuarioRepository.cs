using ApiAutenticacao.Data;
using SIA_SistemaIntegradoDeAutenticação;
using static ApiAutenticacao.Repositories.UsuarioRepository;

namespace ApiAutenticacao.Repositories
{
    public class UsuarioRepository
    {
        /// <summary>
        /// ProdutoRepository - classe de repositório
        /// responsável por acessor os dados dos produtos no banco de dados
        /// </summary>
        /// <remarks>
        /// Construtor da classe - recebeu o contexto do banco de dados 
        /// </remarks>
        /// <param name="context"></param>"
        public class UsuarioRepository(AppDbContext context) : IUsuarioRepository
        {
            /// <summary>
            /// AppDbContext - contexto do banco de dados - responsável por
            /// gerenciar a conexão com o banco de dados e fornecer acesso
            /// is tabelas e entidades do banco de dados
            /// </summary>

            private readonly AppDbContext _context = context;

            /// <summary>
            /// O metódo GetAll é responsável 
            /// por retornar uma lista de todos os produtos
            /// </summary>
            /// <returns></returns>

            public async Task<List<Usuarios>> GetAll() => await _context.Usuarios.ToListAsync();

            /// <summary>
            /// GetById é responsável por retornar um produto específico
            /// </summary>
            /// <param name="id"></param>
            /// <returns></returns>

            public async Task<Usuarios> GetById(int id) => await _context.Usuarios.FindAsync(id);

            /// <summary>
            /// Add é responsável por adicionar um novo 
            /// produto ao banco de dados
            /// </summary>
            /// <param name="produto"></param>
            /// <returns></returns>

            public async Task Add(Usuarios usuario)
            {
                _context.Usuarios.Add(usuario);
                await _context.SaveChangesAsync();
            }

            /// <summary>
            /// Update é responsável por atualizar um produto 
            /// existente no banco de dados
            /// </summary>
            /// <param name="usuario"></param>
            /// <returns></returns>

            public async Task Update(Usuarios usuario)
            {
                _context.Usuarios.Update(usuario);
                await _context.SaveChangesAsync();
            }

            /// <summary>
            /// Delete é responsável por excluir um produto
            /// do banco de dados
            /// </summary>
            /// <param name="id"></param>
            /// <returns></returns>

            public async Task Delete(int id)
            {
                var p = await GetById(id);
                _context.Usuarios.Remove(p);
                await _context.SaveChangesAsync();
            }
        }
    }
}
