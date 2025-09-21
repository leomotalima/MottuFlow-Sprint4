using MottuFlow.Services;
using MottuFlow.Models;
using MottuFlow.Repositories;
using MottuFlow.Data;

public class FuncionarioServiceIntegrationTests
{
    private readonly FuncionarioService _service;
    private readonly AppDbContext _context;

    public FuncionarioServiceIntegrationTests()
    {
        // Configura DbContext InMemory para testes
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDb")
            .Options;

        _context = new AppDbContext(options);

        // Cria o repositório e injeta no service
        var repository = new FuncionarioRepository(_context);
        _service = new FuncionarioService(repository);
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
        await _service.Adicionar(funcionario); // método do service

        // Assert
        var criado = await _context.Funcionarios.FirstOrDefaultAsync(f => f.Cpf == "12345678900");
        Assert.NotNull(criado);
        Assert.Equal("Leo", criado!.Nome);
    }
}
