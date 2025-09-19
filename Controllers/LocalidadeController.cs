using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MottuFlowApi.Data;
using MottuFlowApi.Models;
using Swashbuckle.AspNetCore.Annotations;

namespace MottuFlowApi.Controllers
{
    [ApiController]
    [Route("api/localidades")]
    public class LocalidadeController : ControllerBase
    {
        private readonly AppDbContext _context;
        public LocalidadeController(AppDbContext context) => _context = context;

        [HttpGet]
        [SwaggerOperation(Summary = "Lista todas as localidades com paginação")]
        public async Task<ActionResult<IEnumerable<object>>> GetLocalidades(int page = 1, int pageSize = 10)
        {
            var locais = await _context.Localidades
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var result = locais.Select(l => new
            {
                l.IdLocalidade,
                l.DataHora,
                l.PontoReferencia,
                l.IdMoto,
                l.IdPatio,
                l.IdCamera,
                Links = new[]
                {
                    new { Rel = "self", Href = $"/api/localidades/{l.IdLocalidade}" },
                    new { Rel = "update", Href = $"/api/localidades/{l.IdLocalidade}" },
                    new { Rel = "delete", Href = $"/api/localidades/{l.IdLocalidade}" }
                }
            });

            return Ok(result);
        }

        [HttpGet("{id}")]
        [SwaggerOperation(Summary = "Retorna uma localidade pelo ID")]
        public async Task<ActionResult<object>> GetLocalidade(int id)
        {
            var l = await _context.Localidades.FindAsync(id);
            if (l == null) return NotFound(new { Message = "Localidade não encontrada." });

            var result = new
            {
                l.IdLocalidade,
                l.DataHora,
                l.PontoReferencia,
                l.IdMoto,
                l.IdPatio,
                l.IdCamera,
                Links = new[]
                {
                    new { Rel = "self", Href = $"/api/localidades/{l.IdLocalidade}" },
                    new { Rel = "update", Href = $"/api/localidades/{l.IdLocalidade}" },
                    new { Rel = "delete", Href = $"/api/localidades/{l.IdLocalidade}" }
                }
            };

            return Ok(result);
        }

        [HttpPost]
        [SwaggerOperation(Summary = "Cria uma nova localidade")]
        public async Task<ActionResult<object>> CreateLocalidade([FromBody] Localidade l)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            _context.Localidades.Add(l);
            await _context.SaveChangesAsync();

            var result = new
            {
                l.IdLocalidade,
                l.DataHora,
                l.PontoReferencia,
                l.IdMoto,
                l.IdPatio,
                l.IdCamera,
                Links = new[]
                {
                    new { Rel = "self", Href = $"/api/localidades/{l.IdLocalidade}" },
                    new { Rel = "update", Href = $"/api/localidades/{l.IdLocalidade}" },
                    new { Rel = "delete", Href = $"/api/localidades/{l.IdLocalidade}" }
                }
            };

            return CreatedAtAction(nameof(GetLocalidade), new { id = l.IdLocalidade }, result);
        }

        [HttpPut("{id}")]
        [SwaggerOperation(Summary = "Atualiza uma localidade")]
        public async Task<IActionResult> UpdateLocalidade(int id, [FromBody] Localidade l)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            if (id != l.IdLocalidade) return BadRequest(new { Message = "ID inválido." });

            _context.Entry(l).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Localidades.Any(x => x.IdLocalidade == id))
                    return NotFound(new { Message = "Localidade não encontrada." });
                throw;
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        [SwaggerOperation(Summary = "Deleta uma localidade")]
        public async Task<IActionResult> DeleteLocalidade(int id)
        {
            var l = await _context.Localidades.FindAsync(id);
            if (l == null) return NotFound(new { Message = "Localidade não encontrada." });

            _context.Localidades.Remove(l);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
