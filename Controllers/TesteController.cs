using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace MottuFlowApi.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/health")]
    [Tags("Health Check")]
    [Produces("application/json")] // ✅ Garante resposta JSON formatada
    public class HealthController : ControllerBase
    {
        /// <summary>
        /// Endpoint de verificação da saúde da API.
        /// </summary>
        [HttpGet("ping")]
        [SwaggerOperation(
            Summary = "Verifica o status de funcionamento da API",
            Description = "Retorna informações sobre o estado atual da aplicação, ambiente e timestamp UTC.")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult Ping()
        {
            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Desconhecido";

            var data = new
            {
                status = "Healthy",
                version = "1.0.0",
                timestamp = DateTime.UtcNow,
                environment
            };

            return Ok(new
            {
                success = true,
                message = "API rodando com sucesso 🚀",
                data
            });
        }
    }
}
