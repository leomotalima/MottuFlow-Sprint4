using System.Net;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using MottuFlowApi.Data;
using Xunit;

namespace MottuFlow.Tests.Integration
{
    [Trait("Category", "Integration")]
    public class FuncionarioControllerIntegrationTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _client;
        private readonly CustomWebApplicationFactory _factory;

        public FuncionarioControllerIntegrationTests(CustomWebApplicationFactory factory)
        {
            _factory = factory;
            _client = factory.CreateClient();
        }

        [Fact(DisplayName = "Banco InMemory deve estar criado e acessível")]
        public void BancoDeveEstarCriado()
        {
            using var scope = _factory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            bool podeConectar = db.Database.CanConnect();
            int total = db.Funcionarios.Count();

            Assert.True(podeConectar, "❌ O banco InMemory não pôde ser acessado.");
            Assert.True(total > 0, "❌ Nenhum funcionário foi inicializado no banco InMemory.");

            Console.WriteLine($"✅ Banco InMemory acessível com {total} funcionário(s).");
        }

        [Fact(DisplayName = "GET /api/v1/funcionarios deve retornar 200 OK")]
        public async Task GetFuncionarios_DeveRetornarStatus200()
        {
            // Executa requisição GET na API
            var response = await _client.GetAsync("/api/v1/funcionarios");
            var content = await response.Content.ReadAsStringAsync();

            if (response.StatusCode != HttpStatusCode.OK)
                Console.WriteLine($"❌ Erro ao requisitar endpoint: {content}");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Console.WriteLine("✅ Endpoint /api/v1/funcionarios retornou 200 OK com sucesso.");
        }
    }
}
