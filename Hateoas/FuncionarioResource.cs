namespace MottuFlow.Hateoas
{
    public class FuncionarioResource : ResourceBase
    {
        public string? Nome { get; set; }  // Agora é permitido que seja nulo
        public string? Cpf { get; set; }   // Agora é permitido que seja nulo
        public string? Cargo { get; set; } // Agora é permitido que seja nulo
        public string? Telefone { get; set; } // Agora é permitido que seja nulo
        public string? Email { get; set; }  // Agora é permitido que seja nulo
    }  
}
