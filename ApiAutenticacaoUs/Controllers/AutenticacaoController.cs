using ApiAutenticacaoUs.Services;
using Microsoft.AspNetCore.Mvc;
using SIA_SistemaIntegradoDeAutenticação;
using System.Text.RegularExpressions;

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
            if (id <= 0)
                return BadRequest(new { mensagem = "ID inválido. O ID deve ser maior que zero." });

            var usuarios = await _service.ObterPorId(id);

            if (usuarios == null)
                return NotFound(new { mensagem = $"Usuário com ID {id} não encontrado." });

            return Ok(usuarios);
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Usuarios usuarios)
        {
            // Validações de modelo
            if (usuarios == null)
                return BadRequest(new { mensagem = "Dados do usuário não foram fornecidos." });

            // Validar Username
            if (string.IsNullOrWhiteSpace(usuarios.Username))
                return BadRequest(new { mensagem = "O campo Username é obrigatório." });

            if (usuarios.Username.Length < 3 || usuarios.Username.Length > 50)
                return BadRequest(new { mensagem = "O Username deve ter entre 3 e 50 caracteres." });

            // Validar Nome Completo
            if (string.IsNullOrWhiteSpace(usuarios.NomeCompleto))
                return BadRequest(new { mensagem = "O campo Nome Completo é obrigatório." });

            if (usuarios.NomeCompleto.Length < 3 || usuarios.NomeCompleto.Length > 100)
                return BadRequest(new { mensagem = "O Nome Completo deve ter entre 3 e 100 caracteres." });

            // Validar Email
            if (string.IsNullOrWhiteSpace(usuarios.Email))
                return BadRequest(new { mensagem = "O campo Email é obrigatório." });

            if (!IsValidEmail(usuarios.Email))
                return BadRequest(new { mensagem = "O email fornecido é inválido." });

            // Validar Senha
            if (string.IsNullOrWhiteSpace(usuarios.Senha))
                return BadRequest(new { mensagem = "O campo Senha é obrigatório." });

            if (usuarios.Senha.Length < 6)
                return BadRequest(new { mensagem = "A senha deve ter no mínimo 6 caracteres." });

            // Validar Telefone (se fornecido)
            if (!string.IsNullOrWhiteSpace(usuarios.Telefone))
            {
                if (!IsValidPhoneNumber(usuarios.Telefone))
                    return BadRequest(new { mensagem = "O telefone fornecido é inválido. Use o formato (DD) 99999-9999" });
            }

            // Validar Data de Nascimento
            if (usuarios.DataNascimento == default)
                return BadRequest(new { mensagem = "A data de nascimento é obrigatória." });

            if (!IsValidAge(usuarios.DataNascimento))
                return BadRequest(new { mensagem = "O usuário deve ter pelo menos 18 anos." });

            // Definir campos automáticos
            usuarios.DataCadastro = DateTime.Now;
            usuarios.UltimoLogin = DateTime.Now;
            usuarios.UltimaModificacao = DateTime.Now;
            usuarios.EmailVerificado = false;

            try
            {
                await _service.Criar(usuarios);
                return CreatedAtAction(nameof(GetById), new { id = usuarios.Id }, usuarios);
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("UNIQUE") && ex.Message.Contains("Username"))
                    return Conflict(new { mensagem = "Este Username já está em uso." });

                if (ex.Message.Contains("UNIQUE") && ex.Message.Contains("Email"))
                    return Conflict(new { mensagem = "Este Email já está cadastrado." });

                return StatusCode(500, new { mensagem = $"Erro interno ao criar usuário: {ex.Message}" });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] Usuarios usuarios)
        {
            // Validações de modelo
            if (usuarios == null)
                return BadRequest(new { mensagem = "Dados do usuário não foram fornecidos." });

            if (id != usuarios.Id)
                return BadRequest(new { mensagem = "O ID da URL não corresponde ao ID do usuário." });

            if (id <= 0)
                return BadRequest(new { mensagem = "ID inválido. O ID deve ser maior que zero." });

            // Verificar se usuário existe
            var existente = await _service.ObterPorId(id);
            if (existente == null)
                return NotFound(new { mensagem = $"Usuário com ID {id} não encontrado." });

            // Validar Username
            if (string.IsNullOrWhiteSpace(usuarios.Username))
                return BadRequest(new { mensagem = "O campo Username é obrigatório." });

            if (usuarios.Username.Length < 3 || usuarios.Username.Length > 50)
                return BadRequest(new { mensagem = "O Username deve ter entre 3 e 50 caracteres." });

            // Validar Nome Completo
            if (string.IsNullOrWhiteSpace(usuarios.NomeCompleto))
                return BadRequest(new { mensagem = "O campo Nome Completo é obrigatório." });

            if (usuarios.NomeCompleto.Length < 3 || usuarios.NomeCompleto.Length > 100)
                return BadRequest(new { mensagem = "O Nome Completo deve ter entre 3 e 100 caracteres." });

            // Validar Email
            if (string.IsNullOrWhiteSpace(usuarios.Email))
                return BadRequest(new { mensagem = "O campo Email é obrigatório." });

            if (!IsValidEmail(usuarios.Email))
                return BadRequest(new { mensagem = "O email fornecido é inválido." });

            // Validar Senha (apenas se foi fornecida)
            if (!string.IsNullOrWhiteSpace(usuarios.Senha) && usuarios.Senha.Length < 6)
                return BadRequest(new { mensagem = "A senha deve ter no mínimo 6 caracteres." });

            // Se a senha foi fornecida vazia, manter a senha existente
            if (string.IsNullOrWhiteSpace(usuarios.Senha))
                usuarios.Senha = existente.Senha;

            // Validar Telefone (se fornecido)
            if (!string.IsNullOrWhiteSpace(usuarios.Telefone))
            {
                if (!IsValidPhoneNumber(usuarios.Telefone))
                    return BadRequest(new { mensagem = "O telefone fornecido é inválido. Use o formato (DD) 99999-9999" });
            }

            // Validar Data de Nascimento
            if (usuarios.DataNascimento == default)
                return BadRequest(new { mensagem = "A data de nascimento é obrigatória." });

            if (!IsValidAge(usuarios.DataNascimento))
                return BadRequest(new { mensagem = "O usuário deve ter pelo menos 18 anos." });

            // Manter campos originais que não devem ser alterados
            usuarios.DataCadastro = existente.DataCadastro;
            usuarios.UltimaModificacao = DateTime.Now;

            try
            {
                await _service.Atualizar(usuarios);
                return Ok(new { mensagem = "Usuário atualizado com sucesso.", usuario = usuarios });
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("UNIQUE") && ex.Message.Contains("Username"))
                    return Conflict(new { mensagem = "Este Username já está em uso." });

                if (ex.Message.Contains("UNIQUE") && ex.Message.Contains("Email"))
                    return Conflict(new { mensagem = "Este Email já está cadastrado." });

                return StatusCode(500, new { mensagem = $"Erro interno ao atualizar usuário: {ex.Message}" });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            if (id <= 0)
                return BadRequest(new { mensagem = "ID inválido. O ID deve ser maior que zero." });

            var existente = await _service.ObterPorId(id);
            if (existente == null)
                return NotFound(new { mensagem = $"Usuário com ID {id} não encontrado." });

            try
            {
                await _service.Deletar(id);
                return Ok(new { mensagem = $"Usuário {existente.NomeCompleto} excluído com sucesso." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensagem = $"Erro interno ao excluir usuário: {ex.Message}" });
            }
        }

        [HttpPatch("{id}/verificar-email")]
        public async Task<IActionResult> VerificarEmail(int id)
        {
            if (id <= 0)
                return BadRequest(new { mensagem = "ID inválido. O ID deve ser maior que zero." });

            var existente = await _service.ObterPorId(id);
            if (existente == null)
                return NotFound(new { mensagem = $"Usuário com ID {id} não encontrado." });

            existente.EmailVerificado = true;
            existente.UltimaModificacao = DateTime.Now;

            await _service.Atualizar(existente);
            return Ok(new { mensagem = "Email verificado com sucesso." });
        }

        // Métodos privados de validação
        private bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            try
            {
                var regex = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$");
                return regex.IsMatch(email);
            }
            catch
            {
                return false;
            }
        }

        private bool IsValidPhoneNumber(string phone)
        {
            if (string.IsNullOrWhiteSpace(phone))
                return true; // Telefone é opcional

            // Remove caracteres não numéricos
            var numbers = new string(phone.Where(char.IsDigit).ToArray());

            // Verifica se tem 10 ou 11 dígitos (com ou sem DDD)
            return numbers.Length == 10 || numbers.Length == 11;
        }

        private bool IsValidAge(DateTime birthDate)
        {
            var today = DateTime.Today;
            var age = today.Year - birthDate.Year;

            if (birthDate.Date > today.AddYears(-age))
                age--;

            return age >= 18;
        }
    }
}