using Microsoft.VisualBasic.FileIO;
using SIA_SistemaIntegradoDeAutenticação;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace InterfaceDeAutenticação.ViewModel
{
    /// <summary>
    /// Controller responsável pelo gerenciamento de leituras de sensores de temperatura e ângulo
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class AutenticacaoController : ControllerBase
    {
        private readonly AutenticacaoService _autenticacaoService;

        public AutenticacaoController(AutenticacaoService autenticacaoService)
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
        public async Task<IActionResult> GetById()
        {
            try
            {
                var autenticacaoData = await _autenticacaoService.GetAutenticacaoDataByIdAsync();
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
        public async Task<IActionResult> Post()
        {
            try
            {
                var autenticacaoData = await _autenticacaoService.CreateAutenticacaoDataAsync();
                return Ok(autenticacaoData);

            }
            catch (Exception ex)
            {
                // Log do erro
                return StatusCode(500, "Erro ao criar dados de autenticação: " + ex.Message);
            }
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> Put()
        {
            try
            {
                var autenticacaoData = await _autenticacaoService.UpdateAutenticacaoDataAsync();
                if (autenticacaoData == null)
                    return NotFound("Dados de autenticação não encontrados para atualização.");
                return Ok(autenticacaoData);
            }
            catch (Exception ex)
            {
                // Log do erro
                return StatusCode(500, "Erro ao atualizar dados de autenticação: " + ex.Message);
            }

        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete()
        {
            try
            {
                var success = await _autenticacaoService.DeleteAutenticacaoDataAsync();
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
