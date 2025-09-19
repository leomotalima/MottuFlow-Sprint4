using System;

namespace MottuFlowApi.Models
{
    public class RegistroStatus
    {
        public int IdStatus { get; set; }
        public string TipoStatus { get; set; } = null!;
        public string? Descricao { get; set; }
        public DateTime DataStatus { get; set; } = DateTime.Now;

        public int IdMoto { get; set; }
        public int IdFuncionario { get; set; }

        // Navegação
        public Moto? Moto { get; set; }
        public Funcionario? Funcionario { get; set; }
    }
}
