using System;
using System.Collections.Generic;

namespace MottuFlowApi.Models
{
    public class Funcionario
    {
        public int IdFuncionario { get; set; }
        public string Nome { get; set; } = null!;
        public string Cpf { get; set; } = null!;
        public string Cargo { get; set; } = null!;
        public string Telefone { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Senha { get; set; } = null!;
        public string? RefreshToken { get; set; }
        public DateTime? ExpiracaoRefreshToken { get; set; }

        // Navigation
        public ICollection<RegistroStatus>? Statuses { get; set; }
    }
}

