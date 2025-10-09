using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace MottuFlow.Tests.Integration
{
    public class FuncionarioControllerIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;

        public FuncionarioControllerIntegrationTests(WebApplicationFactory<Program> factory)
        {
            _client = factory.CreateClient();
        }

        [Fact(DisplayName = "GET /api/v1/funcionarios deve retornar 200 OK")]
        public async Task GetFuncionarios_DeveRetornarStatus200()
        {
            // Faz requisição GET para o endpoint da API
            var response = await _client.GetAsync("/api/v1/funcionarios");

            // Verifica se retornou com sucesso (200 OK)
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
    }
}
