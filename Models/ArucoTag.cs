namespace MottuFlowApi.Models
{
    public class ArucoTag
    {
        public int IdTag { get; set; }
        public string Codigo { get; set; } = null!;
        public string Status { get; set; } = null!;
        public int IdMoto { get; set; }

        // Navigation
        public Moto? Moto { get; set; }
    }
}
