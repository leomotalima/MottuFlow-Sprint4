namespace MottuFlowApi.DTOs
{
    public class FuncionarioInputDTO
    {
        public string Nome { get; set; } = null!;
        public string Cpf { get; set; } = null!;
        public string Cargo { get; set; } = null!;
        public string Telefone { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Senha { get; set; } = null!;
    }
}
