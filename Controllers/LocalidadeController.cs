using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MottuFlowApi.Data;
using MottuFlowApi.Models;
using MottuFlowApi.DTOs;
using Swashbuckle.AspNetCore.Annotations;

namespace MottuFlowApi.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/localidades")]
    [Tags("Localidades")]
    [Produces("application/json")] // ✅ Garante saída JSON no Swagger
    public class LocalidadeController : ControllerBase
    {
        private readonly AppDbContext _context;
        public LocalidadeController(AppDbContext context) => _context = context;

        // 🧩 GET - Lista todas as localidades
        [HttpGet(Name = "GetLocalidades")]
        [SwaggerOperation(Summary = "Lista todas as localidades registradas no sistema")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetLocalidades()
        {
            var localidades = await _context.Localidades
                .Select(l => new LocalidadeOutputDTO
                {
                    IdLocalidade = l.IdLocalidade,
                    DataHora = l.DataHora,
                    PontoReferencia = l.PontoReferencia,
                    IdMoto = l.IdMoto,
                    IdPatio = l.IdPatio,
                    IdCamera = l.IdCamera
                })
                .ToListAsync();

            if (!localidades.Any())
                return Ok(new { success = true, message = "Nenhuma localidade cadastrada.", data = new List<LocalidadeOutputDTO>() });

            return Ok(new { success = true, data = localidades });
        }

        // 🧩 GET - Localidade por ID
        [HttpGet("{id}", Name = "GetLocalidade")]
        [SwaggerOperation(Summary = "Retorna os dados de uma localidade específica pelo ID")]
        [ProducesResponseType(typeof(LocalidadeOutputDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetLocalidade(int id)
        {
            var l = await _context.Localidades.FindAsync(id);
            if (l == null)
                return NotFound(new { success = false, message = "Localidade não encontrada." });

            var result = new LocalidadeOutputDTO
            {
                IdLocalidade = l.IdLocalidade,
                DataHora = l.DataHora,
                PontoReferencia = l.PontoReferencia,
                IdMoto = l.IdMoto,
                IdPatio = l.IdPatio,
                IdCamera = l.IdCamera
            };

            return Ok(new { success = true, data = result });
        }

        // 🧩 POST - Cria uma nova localidade
        [HttpPost(Name = "CreateLocalidade")]
        [SwaggerOperation(Summary = "Cria uma nova localidade no sistema")]
        [ProducesResponseType(typeof(LocalidadeOutputDTO), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateLocalidade([FromBody] LocalidadeInputDTO input)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { success = false, message = "Dados inválidos.", errors = ModelState });

            var localidade = new Localidade
            {
                DataHora = input.DataHora,
                PontoReferencia = input.PontoReferencia,
                IdMoto = input.IdMoto,
                IdPatio = input.IdPatio,
                IdCamera = input.IdCamera
            };

            _context.Localidades.Add(localidade);
            await _context.SaveChangesAsync();

            var result = new LocalidadeOutputDTO
            {
                IdLocalidade = localidade.IdLocalidade,
                DataHora = localidade.DataHora,
                PontoReferencia = localidade.PontoReferencia,
                IdMoto = localidade.IdMoto,
                IdPatio = localidade.IdPatio,
                IdCamera = localidade.IdCamera
            };

            return CreatedAtAction(nameof(GetLocalidade), new { id = localidade.IdLocalidade },
                new { success = true, message = "Localidade criada com sucesso.", data = result });
        }

        // 🧩 PUT - Atualiza uma localidade existente
        [HttpPut("{id}", Name = "UpdateLocalidade")]
        [SwaggerOperation(Summary = "Atualiza uma localidade existente pelo ID")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateLocalidade(int id, [FromBody] LocalidadeInputDTO input)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { success = false, message = "Dados inválidos.", errors = ModelState });

            var localidade = await _context.Localidades.FindAsync(id);
            if (localidade == null)
                return NotFound(new { success = false, message = "Localidade não encontrada." });

            localidade.DataHora = input.DataHora;
            localidade.PontoReferencia = input.PontoReferencia;
            localidade.IdMoto = input.IdMoto;
            localidade.IdPatio = input.IdPatio;
            localidade.IdCamera = input.IdCamera;

            _context.Entry(localidade).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            var updated = new LocalidadeOutputDTO
            {
                IdLocalidade = localidade.IdLocalidade,
                DataHora = localidade.DataHora,
                PontoReferencia = localidade.PontoReferencia,
                IdMoto = localidade.IdMoto,
                IdPatio = localidade.IdPatio,
                IdCamera = localidade.IdCamera
            };

            return Ok(new { success = true, message = "Localidade atualizada com sucesso.", data = updated });
        }

        // 🧩 DELETE - Remove uma localidade
        [HttpDelete("{id}", Name = "DeleteLocalidade")]
        [SwaggerOperation(Summary = "Remove uma localidade do sistema pelo ID")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteLocalidade(int id)
        {
            var l = await _context.Localidades.FindAsync(id);
            if (l == null)
                return NotFound(new { success = false, message = "Localidade não encontrada." });

            _context.Localidades.Remove(l);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
