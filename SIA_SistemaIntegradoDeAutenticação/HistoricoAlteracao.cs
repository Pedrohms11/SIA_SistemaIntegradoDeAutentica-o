
namespace SIA_SistemaIntegradoDeAutenticação
{
    public class HistoricoAlteracao
    {             

            public int Id { get; set; }

            public int UsuarioId { get; set; }

            public string TipoAcao { get; set; } // CREATE, UPDATE, DELETE

            public DateTime DataHora { get; set; }

            public string Descricao { get; set; }

            public string DadosAntigos { get; set; } // JSON com os dados antes da alteração

            public string DadosNovos { get; set; } // JSON com os dados depois da alteração

            public string IpOrigem { get; set; } // IP de onde veio a alteração

            // Chave estrangeira
            public virtual Usuarios Usuario { get; set; }
        
    }
}
