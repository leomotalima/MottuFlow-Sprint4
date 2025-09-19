using Microsoft.AspNetCore.Mvc;

namespace MottuFlowApi.Controllers
{
    [ApiController]
    [Route("api/teste")]
    public class TesteController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get()
        {
            var result = new
            {
                mensagem = "API MottuFlow funcionando!",
                Links = new[]
                {
                    new { Rel = "self", Href = "/api/teste" },
                    new { Rel = "motos", Href = "/api/motos" },
                    new { Rel = "funcionarios", Href = "/api/funcionarios" }
                },
                status = "online",
                timestamp = DateTime.UtcNow
            };

            return Ok(result);
        }
    }
}
