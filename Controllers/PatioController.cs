using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MottuFlowApi.Data;
using MottuFlowApi.Models;
using Swashbuckle.AspNetCore.Annotations;

namespace MottuFlowApi.Controllers
{
    [ApiController]
    [Route("api/patios")]
    public class PatioController : ControllerBase
    {
        private readonly AppDbContext _context;
        public PatioController(AppDbContext context) => _context = context;

        [HttpGet]
        [SwaggerOperation(Summary = "Lista todos os pátios com paginação")]
        public async Task<ActionResult<IEnumerable<object>>> GetPatios(int page = 1, int pageSize = 10)
        {
            var patios = await _context.Patios
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var result = patios.Select(p => new
            {
                p.IdPatio,
                p.Nome,
                p.Endereco, // corrigido
                Links = new[]
                {
                    new { Rel = "self", Href = $"/api/patios/{p.IdPatio}" },
                    new { Rel = "update", Href = $"/api/patios/{p.IdPatio}" },
                    new { Rel = "delete", Href = $"/api/patios/{p.IdPatio}" }
                }
            });

            return Ok(result);
        }

        [HttpGet("{id}")]
        [SwaggerOperation(Summary = "Retorna pátio por ID")]
        public async Task<ActionResult<object>> GetPatio(int id)
        {
            var p = await _context.Patios.FindAsync(id);
            if (p == null) return NotFound(new { Message = "Pátio não encontrado." });

            var result = new
            {
                p.IdPatio,
                p.Nome,
                p.Endereco, // corrigido
                Links = new[]
                {
                    new { Rel = "self", Href = $"/api/patios/{p.IdPatio}" },
                    new { Rel = "update", Href = $"/api/patios/{p.IdPatio}" },
                    new { Rel = "delete", Href = $"/api/patios/{p.IdPatio}" }
                }
            };

            return Ok(result);
        }

        [HttpPost]
        [SwaggerOperation(Summary = "Cria um novo pátio")]
        public async Task<ActionResult<object>> CreatePatio([FromBody] Patio p)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            _context.Patios.Add(p);
            await _context.SaveChangesAsync();

            var result = new
            {
                p.IdPatio,
                p.Nome,
                p.Endereco, // corrigido
                Links = new[]
                {
                    new { Rel = "self", Href = $"/api/patios/{p.IdPatio}" },
                    new { Rel = "update", Href = $"/api/patios/{p.IdPatio}" },
                    new { Rel = "delete", Href = $"/api/patios/{p.IdPatio}" }
                }
            };

            return CreatedAtAction(nameof(GetPatio), new { id = p.IdPatio }, result);
        }

        [HttpPut("{id}")]
        [SwaggerOperation(Summary = "Atualiza um pátio")]
        public async Task<IActionResult> UpdatePatio(int id, [FromBody] Patio p)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            if (id != p.IdPatio) return BadRequest(new { Message = "ID inválido." });

            _context.Entry(p).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Patios.Any(x => x.IdPatio == id))
                    return NotFound(new { Message = "Pátio não encontrado." });
                throw;
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        [SwaggerOperation(Summary = "Deleta um pátio")]
        public async Task<IActionResult> DeletePatio(int id)
        {
            var p = await _context.Patios.FindAsync(id);
            if (p == null) return NotFound(new { Message = "Pátio não encontrado." });

            _context.Patios.Remove(p);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
