using MottuFlowApi.DTOs;
namespace MottuFlowApi.DTOs
{
    public class FuncionarioOutputDTO
    {
        public int IdFuncionario { get; set; }
        public string Nome { get; set; } = null!;
        public string Cpf { get; set; } = null!;
        public string Cargo { get; set; } = null!;
        public string Telefone { get; set; } = null!;
        public string Email { get; set; } = null!;
    }
}
