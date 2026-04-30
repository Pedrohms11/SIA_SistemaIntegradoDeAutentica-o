

using ApiAutenticacao.Services;
using Microsoft.AspNetCore.Mvc;
using SIA_SistemaIntegradoDeAutenticação;

namespace InterfaceDeAutenticação.ViewModel
{
    /// <summary>
    /// Controller responsável pelo gerenciamento de leituras de sensores de temperatura e ângulo
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class AutenticacaoController : ControllerBase
    {
        private readonly UsuarioService _autenticacaoService;

        public AutenticacaoController(UsuarioService autenticacaoService)
        {
            _autenticacaoService = autenticacaoService;
        }
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            try
            {
                var autenticacaoData = await _autenticacaoService.GetAutenticacaoDataAsync();
                return Ok(autenticacaoData);
            }
            catch (Exception ex)
            {
                // Log do erro
                return StatusCode(500, "Erro ao obter dados de autenticação: " + ex.Message);
            }

        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var autenticacaoData = await _autenticacaoService.GetAutenticacaoDataByIdAsync(id);
                if (autenticacaoData == null)
                    return NotFound("Dados de autenticação não encontrados.");
                return Ok(autenticacaoData);

            }
            catch (Exception ex)
            {
                // Log do erro
                return StatusCode(500, "Erro ao obter dados de autenticação: " + ex.Message);
            }
        }
        [HttpPost]
        public async Task<IActionResult> Post(Usuarios usuarios)
        {
            try
            {
                var _usuarioService = await _autenticacaoService.CreateAutenticacaoDataAsync(usuarios);
                return Ok(_usuarioService);

            }
            catch (Exception ex)
            {
                // Log do erro
                return StatusCode(500, "Erro ao criar dados de autenticação: " + ex.Message);
            }
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, Usuarios usuarios)
        {
            try
            {
                var usuarioExistente = await _autenticacaoService.Atualizar(id, usuarios);
                if (usuarioExistente == null)
                    return NotFound("Dados de autenticação não encontrados para atualização.");
                return Ok(usuarioExistente);
            }
            catch (Exception ex)
            {
                // Log do erro
                return StatusCode(500, "Erro ao atualizar dados de autenticação: " + ex.Message);
            }

        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var success = await _autenticacaoService.DeleteAutenticacaoDataAsync(id);
                if (!success)
                    return NotFound("Dados de autenticação não encontrados para exclusão.");
                return Ok("Dados de autenticação excluídos com sucesso.");
            }
            catch (Exception ex)
            {
                // Log do erro
                return StatusCode(500, "Erro ao excluir dados de autenticação: " + ex.Message);
            }
        }
    }
}
