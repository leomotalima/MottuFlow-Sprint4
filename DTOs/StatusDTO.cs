using System;

namespace MottuFlow.DTOs
{
    // DTO para representar os status das motos
    public class StatusDTO
    {
        public int IdStatus { get; set; }
        public string TipoStatus { get; set; } = string.Empty;
        public string Descricao { get; set; } = string.Empty;
        public DateTime DataStatus { get; set; }
        public int IdFuncionario { get; set; }
    }
}
