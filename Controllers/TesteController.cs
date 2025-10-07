using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace MottuFlowApi.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/health")]
    [Tags("Health Check")]
    public class HealthController : ControllerBase
    {
        /// <summary>
        /// Verifica se a API está em execução.
        /// </summary>
        [HttpGet("ping")]
        [SwaggerOperation(Summary = "Verifica o status de funcionamento da API")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult Ping()
        {
            return Ok(new
            {
                success = true,
                message = "API rodando com sucesso 🚀",
                timestamp = DateTime.UtcNow,
                environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Desconhecido"
            });
        }
    }
}
