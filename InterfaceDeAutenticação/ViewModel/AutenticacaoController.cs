

using ApiAutenticacaoUs.Services;
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
        private readonly UsuarioServices _autenticacaoService;

        public AutenticacaoController(UsuarioServices autenticacaoService)
        {
            _autenticacaoService = autenticacaoService;
        }
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            try
            {
                var usuarios = await _autenticacaoService.Listar();
                return Ok(usuarios);
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
                var usuarios = await _autenticacaoService.ObterPorId(id);
                if (usuarios == null)
                    return NotFound("Dados de usuario não encontrados.");
                return Ok(usuarios);

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
                await _autenticacaoService.Criar(usuarios);
                return CreatedAtAction(nameof(GetById), new { id = usuarios.Id }, usuarios);

            }
            catch (Exception ex)
            {
                // Log do erro
                return StatusCode(500, "Erro ao criar dados de autenticação: " + ex.Message);
            }
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] Usuarios usuarios)
        {
            if (id != usuarios.Id)
                return BadRequest("ID do usuário não corresponde ao ID fornecido ");
            try
            {
                var usuarioExistente = await _autenticacaoService.ObterPorId(id);
                if (usuarioExistente == null)
                    return NotFound("Dados de autenticação não encontrados para atualização.");
                
                await _autenticacaoService.Atualizar(usuarios);
                return NoContent();
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
                var usuarioExistente = await _autenticacaoService.ObterPorId(id);
                if (usuarioExistente == null)
                    return NotFound("Dados de usuario não encontrados para exclusão.");

                await _autenticacaoService.Deletar(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                // Log do erro
                return StatusCode(500, "Erro ao excluir dados de autenticação: " + ex.Message);
            }
        }
    }
}
