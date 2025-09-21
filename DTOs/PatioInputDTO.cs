namespace MottuFlowApi.DTOs
{
    public class PatioInputDTO
    {
        public string Nome { get; set; } = string.Empty;
        public string Endereco { get; set; } = string.Empty;
        public int CapacidadeMaxima { get; set; }
    }
}