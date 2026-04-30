using ApiAutenticacaoUs.Services;
using Microsoft.AspNetCore.Mvc;
using SIA_SistemaIntegradoDeAutenticação;

namespace ApiAutenticacaoUs.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AutenticacaoController : ControllerBase
    {
        private readonly UsuarioServices _service;

        public AutenticacaoController(UsuarioServices services)
        {
            _service = services;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {

            var usuarios = await _service.Listar();
            return Ok(usuarios);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var usuarios = await _service.ObterPorId(id);

            if(usuarios == null)
            
                return NotFound();

            return Ok(usuarios);
                                      
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Usuarios usuarios)
        {
            await _service.Criar(usuarios);
            return CreatedAtAction(nameof(GetById), new { id = usuarios.Id }, usuarios);

        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] Usuarios usuarios)
        {,
            if (id != usuarios.Id)
                return BadRequest();
            var existente = await _service.ObterPorId(id);
            if (existente == null)
                return NotFound();

            await _service.Atualizar(usuarios);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int Id)
        {
            var existente = await _service.ObterPorId(Id);
            if( existente == null)
                return NotFound();

            await _service.Deletar(Id);
            return NoContent();
        }
    }
}
