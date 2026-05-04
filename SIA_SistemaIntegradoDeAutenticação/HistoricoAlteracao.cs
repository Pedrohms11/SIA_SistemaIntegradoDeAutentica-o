using SIA_SistemaIntegradoDeAutenticação;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ConsoleMonitor.Data
{
    public class HistoricoAlteracao
    {
        public int Id { get; set; }

        public int UsuarioId { get; set; }

        public string TipoAcao { get; set; } = string.Empty; // 🔧 Inicializado

        public DateTime DataHora { get; set; }

        public string Descricao { get; set; } = string.Empty; // 🔧 Inicializado

        public string DadosAntigos { get; set; } = string.Empty; // 🔧 Inicializado

        public string DadosNovos { get; set; } = string.Empty; // 🔧 Inicializado

        public string IpOrigem { get; set; } = string.Empty; // 🔧 Inicializado

        public virtual Usuarios Usuario { get; set; } = null!; // 🔧 Inicializado
    }
}