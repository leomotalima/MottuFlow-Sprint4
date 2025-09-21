using Xunit;
using MottuFlow.Services;
using MottuFlow.Models;

public class FuncionarioServiceIntegrationTests
{
    private readonly FuncionarioService _service;

    public FuncionarioServiceIntegrationTests()
    {
        _service = new FuncionarioService(); // assume que o service conecta ao Oracle
    }

    [Fact]
    public async void CriarFuncionario_DeveInserirNoBanco()
    {
        // Arrange
        var funcionario = new Funcionario
        {
            Nome = "Leo",
            Cpf = "12345678900",
            Cargo = "Desenvolvedor",
            Telefone = "11999999999",
            Email = "leo@test.com",
            Senha = "senha123"
        };

        // Act
        var criado = await _service.CreateFuncionarioAsync(funcionario);

        // Assert
        Assert.NotNull(criado);
        Assert.True(criado.Id > 0); // assumindo que o Id é gerado pelo banco
    }
}
