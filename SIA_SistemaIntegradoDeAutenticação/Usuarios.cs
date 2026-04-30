

namespace SIA_SistemaIntegradoDeAutenticação
{
    public class Usuarios
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string NomeCompleto { get; set; }
        public string Email { get; set; }
        public string Senha { get; set; }
        public string Genero { get; set; }
        public string Telefone { get; set; }
        public string Pais { get; set; }
        public DateTime DataNascimento { get; set; }
        public DateTime DataCadastro { get; set; }
        public DateTime UltimoLogin { get; set; }
        public bool EmailVerificado { get; set; }






    }
}
