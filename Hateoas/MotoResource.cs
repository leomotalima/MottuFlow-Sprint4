using MottuFlow.Hateoas;
using MottuFlow.DTOs;
using System.Collections.Generic; // necessário para List<>

namespace MottuFlow.Hateoas
{
    public class MotoResource : ResourceBase
    {
        public string Placa { get; set; } = string.Empty;
        public string Modelo { get; set; } = string.Empty;
        public string Fabricante { get; set; } = string.Empty;
        public int Ano { get; set; }
        public int IdPatio { get; set; }
        public string LocalizacaoAtual { get; set; } = string.Empty;
        public List<StatusDTO> Statuses { get; set; } = new List<StatusDTO>();
    }
}
