using ApiAutenticacao.Services;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using SIA_SistemaIntegradoDeAutenticação;
using System.Linq.Expressions;
using System.Security.Cryptography.X509Certificates;

namespace ApiAutenticacao.Controllers
{
    public class AutenticacaoController : ControllerBase
    {
        private readonly UsuarioService _service;

        public AutenticacaoController(UsuarioService service)
        {
            _service = service;
        }
        [HttpGet]
        public async Task<IActionResult> GetUsuarios()
        {
            try
            {
                var usuarios = await _service.Listar();
                if (usuarios == null || usuarios.Count == 0)
                {
                    return Ok(new { Message = "Nenhum usuário encontrado." });
                }
                return Ok(new { Message = "Usuários encontrados com sucesso.", Data = usuarios });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Ocorreu um erro ao listar os usuários.", Error = ex.Message });
            }
        }
        public async Task<IActionResult> GetUsuarioById(int id)
        {
            if (id <= 0)
            {
                return BadRequest(new { Message = "ID inválido. O ID deve ser um número positivo." });
            }
            try
            {
                var usuario = await _service.ObterPorId(id);
                if (usuario == null)
                {
                    return NotFound(new { Message = $"Usuário com ID {id} não encontrado." });
                }
                return Ok(new { Message = "Usuário encontrado com sucesso.", Data = usuario });

            }
            catch
            {
                return StatusCode(500, new { Message = "Ocorreu um erro ao obter o usuário." });
            }

        }
        public async Task<IActionResult> Post([FromBody]Usuarios usuario)
        {
            try
            {


                if (ModelState.IsValid)
                {
                    return BadRequest(new
                    {
                        Message = "Dados de usuário inválidos. Verifique as informações fornecidas.",
                        erros = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)
                    });
                }
                else
                {
                    await _service.Criar(usuario);
                    return Ok(new { Message = "Usuário criado com sucesso." });
                }

            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Ocorreu um erro ao criar o usuário.", Error = ex.Message });
            }
        }
        public async  Task<IActionResult> Put (int id,[FromBody] Usuarios usuario)
        {
            if (id <= 0)
            {
                return BadRequest(new { Message = "ID inválido. O ID deve ser um número positivo." });
            }
            try
            {
                var usuarioExistente = await _service.ObterPorId(id);
                if (usuarioExistente == null)
                {
                    return NotFound(new { Message = $"Usuário com ID {id} não encontrado." });
                }
                await _service.Atualizar(id, usuarioExistente);
                return Ok(new { Message = "Usuário atualizado com sucesso." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Ocorreu um erro ao atualizar o usuário.", Error = ex.Message });
            }
        }
        public async Task<IActionResult> Delete(int Id)
        {
            var usuarioExistente = await _service.ObterPorId(Id);
            await _service.Deletar(Id);
            return Ok(new { Message = "Usuário deletado com sucesso." });

        }


    }
    
}
