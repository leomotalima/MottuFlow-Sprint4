namespace MottuFlowApi.Models
{
    public class MotoData
    {
        public float Quilometragem { get; set; }
        public float TempoUsoMeses { get; set; }
        public bool PrecisaManutencao { get; set; }
    }

    public class MotoPrediction
    {
        public bool Predicao { get; set; }
        public float Probabilidade { get; set; }
    }
}
