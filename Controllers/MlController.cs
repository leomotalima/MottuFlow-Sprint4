using Microsoft.AspNetCore.Mvc;
using MottuFlowApi.Services;
using Swashbuckle.AspNetCore.Annotations;

namespace MottuFlowApi.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/ml")]
    [Tags("Machine Learning")]
    [Produces("application/json")] // ✅ Garante exibição JSON no Swagger
    public class MlController : ControllerBase
    {
        private readonly MotoMlService _mlService;

        public MlController()
        {
            _mlService = new MotoMlService();
        }

        // 🧠 POST - Predição de manutenção
        [HttpPost("predicao")]
        [SwaggerOperation(
            Summary = "Prediz se a moto precisa de manutenção com base na quilometragem e tempo de uso",
            Description = "Usa um modelo de Machine Learning para prever a necessidade de manutenção preventiva de uma motocicleta.")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult Prever([FromBody] MotoInput input)
        {
            if (input == null)
                return BadRequest(new { success = false, message = "Entrada inválida. O corpo da requisição não pode ser nulo." });

            if (input.Quilometragem <= 0 || input.TempoUsoMeses <= 0)
                return BadRequest(new { success = false, message = "Os valores de quilometragem e tempo de uso devem ser maiores que zero." });

            var resultado = _mlService.Prever(input.Quilometragem, input.TempoUsoMeses);

            return Ok(new
            {
                success = true,
                message = "Predição de manutenção realizada com sucesso.",
                data = new
                {
                    input.Quilometragem,
                    input.TempoUsoMeses,
                    resultado.Predicao,
                    resultado.Probabilidade
                }
            });
        }

        // DTO para entrada do modelo
        public class MotoInput
        {
            public float Quilometragem { get; set; }
            public float TempoUsoMeses { get; set; }
        }
    }
}
