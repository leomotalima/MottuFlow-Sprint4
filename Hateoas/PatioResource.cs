using MottuFlow.Hateoas;

namespace MottuFlow.Hateoas
{
    public class PatioResource : ResourceBase
    {
        public new int Id { get; set; }  // Identificador único do pátio
        public string Nome { get; set; } = string.Empty;
        public string Endereco { get; set; } = string.Empty;
        public int CapacidadeMaxima { get; set; }
    }
}
