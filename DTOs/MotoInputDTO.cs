// MotoInputDTO.cs
namespace MottuFlowApi.DTOs
{
    public class MotoInputDTO
    {
        public string Placa { get; set; } = string.Empty;
        public string Modelo { get; set; } = string.Empty;
        public string Fabricante { get; set; } = string.Empty;
        public int Ano { get; set; }
        public int IdPatio { get; set; }
        public string LocalizacaoAtual { get; set; } = string.Empty;
    }
}