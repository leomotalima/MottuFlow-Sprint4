using Microsoft.AspNetCore.Mvc;
using MottuFlowApi.Services;
using Swashbuckle.AspNetCore.Annotations;

namespace MottuFlowApi.Controllers
{
    [ApiController]
    [Route("api/ml")]
    public class MlController : ControllerBase
    {
        private readonly MotoMlService _mlService;

        public MlController()
        {
            _mlService = new MotoMlService();
        }

        [HttpPost("predicao")]
        [SwaggerOperation(Summary = "Prediz se a moto precisa de manutenção com base na quilometragem e tempo de uso")]
        public IActionResult Prever([FromBody] MotoInput input)
        {
            var resultado = _mlService.Prever(input.Quilometragem, input.TempoUsoMeses);
            return Ok(new
            {
                input.Quilometragem,
                input.TempoUsoMeses,
                resultado.Predicao,
                resultado.Probabilidade
            });
        }

        public class MotoInput
        {
            public float Quilometragem { get; set; }
            public float TempoUsoMeses { get; set; }
        }
    }
}
