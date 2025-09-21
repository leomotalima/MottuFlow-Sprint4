using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MottuFlowApi.Data;
using MottuFlow.Models;
using Swashbuckle.AspNetCore.Annotations;

namespace MottuFlowApi.Controllers
{
    [ApiController]
    [Route("api/aruco-tags")]
    public class ArucoTagController : ControllerBase
    {
        private readonly AppDbContext _context;
        public ArucoTagController(AppDbContext context) => _context = context;

        [HttpGet]
        [SwaggerOperation(Summary = "Lista todas as ArucoTags com paginação")]
        public async Task<ActionResult<IEnumerable<object>>> GetArucoTags(int page = 1, int pageSize = 10)
        {
            var tags = await _context.ArucoTags
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var result = tags.Select(t => new
            {
                t.IdTag,
                t.Codigo,
                t.Status,
                t.IdMoto,
                Links = new[]
                {
                    new { Rel = "self", Href = $"/api/aruco-tags/{t.IdTag}" },
                    new { Rel = "update", Href = $"/api/aruco-tags/{t.IdTag}" },
                    new { Rel = "delete", Href = $"/api/aruco-tags/{t.IdTag}" }
                }
            });

            return Ok(result);
        }

        [HttpGet("{id}")]
        [SwaggerOperation(Summary = "Retorna ArucoTag por ID")]
        public async Task<ActionResult<object>> GetArucoTag(int id)
        {
            var t = await _context.ArucoTags.FindAsync(id);
            if (t == null) return NotFound(new { Message = "ArucoTag não encontrada." });

            var result = new
            {
                t.IdTag,
                t.Codigo,
                t.Status,
                t.IdMoto,
                Links = new[]
                {
                    new { Rel = "self", Href = $"/api/aruco-tags/{t.IdTag}" },
                    new { Rel = "update", Href = $"/api/aruco-tags/{t.IdTag}" },
                    new { Rel = "delete", Href = $"/api/aruco-tags/{t.IdTag}" }
                }
            };

            return Ok(result);
        }

        [HttpPost]
        [SwaggerOperation(Summary = "Cria uma nova ArucoTag")]
        public async Task<ActionResult<object>> CreateArucoTag([FromBody] ArucoTag t)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            _context.ArucoTags.Add(t);
            await _context.SaveChangesAsync();

            var result = new
            {
                t.IdTag,
                t.Codigo,
                t.Status,
                t.IdMoto,
                Links = new[]
                {
                    new { Rel = "self", Href = $"/api/aruco-tags/{t.IdTag}" },
                    new { Rel = "update", Href = $"/api/aruco-tags/{t.IdTag}" },
                    new { Rel = "delete", Href = $"/api/aruco-tags/{t.IdTag}" }
                }
            };

            return CreatedAtAction(nameof(GetArucoTag), new { id = t.IdTag }, result);
        }

        [HttpPut("{id}")]
        [SwaggerOperation(Summary = "Atualiza uma ArucoTag")]
        public async Task<IActionResult> UpdateArucoTag(int id, [FromBody] ArucoTag t)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            if (id != t.IdTag) return BadRequest(new { Message = "ID inválido." });

            _context.Entry(t).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.ArucoTags.Any(x => x.IdTag == id))
                    return NotFound(new { Message = "ArucoTag não encontrada." });
                throw;
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        [SwaggerOperation(Summary = "Deleta uma ArucoTag")]
        public async Task<IActionResult> DeleteArucoTag(int id)
        {
            var t = await _context.ArucoTags.FindAsync(id);
            if (t == null) return NotFound(new { Message = "ArucoTag não encontrada." });

            _context.ArucoTags.Remove(t);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
