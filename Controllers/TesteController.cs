using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;

namespace MottuFlowApi.Controllers
{
    [ApiController]
    [Route("api/teste")]
    public class TesteController : ControllerBase
    {
        // Simulando dados de exemplo
        private readonly List<FuncionarioDto> funcionarios = new()
        {
            new FuncionarioDto { Id = 1, Nome = "João" },
            new FuncionarioDto { Id = 2, Nome = "Maria" }
        };

        // GET api/teste/ids
        [HttpGet("ids")]
        public ActionResult<List<string>> GetIdsComoString()
        {
            // Convertendo int → string para evitar CS0029
            var idsComoString = funcionarios
                .Select(f => f.Id.ToString())
                .ToList();

            return Ok(idsComoString);
        }

        // GET api/teste/nomes
        [HttpGet("nomes")]
        public ActionResult<List<string>> GetNomes()
        {
            // Tratando possível null com ?? ""
            var nomes = funcionarios
                .Select(f => f.Nome ?? string.Empty)
                .ToList();

            return Ok(nomes);
        }
    }

    // DTO simples
    public class FuncionarioDto
    {
        public int Id { get; set; }
        public string? Nome { get; set; } // string? para evitar warning CS8601
    }
}
