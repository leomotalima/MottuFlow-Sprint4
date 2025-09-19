namespace MottuFlowApi.Models
{
    public class Camera
    {
        public int IdCamera { get; set; }
        public string StatusOperacional { get; set; } = null!;
        public string LocalizacaoFisica { get; set; } = null!;
        public int IdPatio { get; set; }

        // Navigation
        public Patio? Patio { get; set; }
        public ICollection<Localidade>? Localidades { get; set; }
    }
}
