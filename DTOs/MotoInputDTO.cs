namespace MottuFlowApi.DTOs
{
    public class MotoInputDTO
    {
        public string Placa { get; set; } = null!;
        public string Modelo { get; set; } = null!;
        public string Fabricante { get; set; } = null!;
        public int Ano { get; set; }
        public int IdPatio { get; set; }
        public string LocalizacaoAtual { get; set; } = null!;
    }
}

