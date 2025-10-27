using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MottuFlowApi.Services;
using MottuFlowApi.Utils;
using Swashbuckle.AspNetCore.Annotations;

namespace MottuFlowApi.Controllers.V1
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/ml")]
    [Tags("Machine Learning")]
    [Produces("application/json")]
    [Consumes("application/json")]
    [AllowAnonymous] // 🔓 ML pode ser público para demonstração
    public class MlController : ControllerBase
    {
        private readonly MotoMlService _mlService;

        public MlController()
        {
            _mlService = new MotoMlService();
        }

        // POST - Predição de manutenção
        [HttpPost("predicao")]
        [SwaggerOperation(
            Summary = "Prediz necessidade de manutenção da moto",
            Description = "Usa um modelo de Machine Learning (ML.NET) para prever se uma moto precisa de manutenção preventiva com base na quilometragem e no tempo de uso.")]
        [SwaggerResponse(StatusCodes.Status200OK, "Predição realizada com sucesso")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Entrada inválida")]
        public IActionResult Prever([FromBody] MotoInput input)
        {
            if (input == null)
                return BadRequest(ApiResponse<string>.Fail("Entrada inválida. O corpo da requisição não pode ser nulo."));

            if (input.Quilometragem <= 0 || input.TempoUsoMeses <= 0)
                return BadRequest(ApiResponse<string>.Fail("Os valores de quilometragem e tempo de uso devem ser maiores que zero."));

            var resultado = _mlService.Prever(input.Quilometragem, input.TempoUsoMeses);

            var data = new
            {
                input.Quilometragem,
                input.TempoUsoMeses,
                resultado.Predicao,
                resultado.Probabilidade
            };

            return Ok(ApiResponse<object>.Ok(data, "Predição de manutenção realizada com sucesso."));
        }

        // DTO de entrada
        public class MotoInput
        {
            public float Quilometragem { get; set; }
            public float TempoUsoMeses { get; set; }
        }
    }
}
