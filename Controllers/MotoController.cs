using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MottuFlowApi.Data;
using MottuFlowApi.Models;
using System.Linq;

namespace MottuFlowApi.Controllers
{
    [ApiController]
    [Route("api/motos")]
    public class MotoController : ControllerBase
    {
        private readonly AppDbContext _context;
        public MotoController(AppDbContext context) => _context = context;

        [HttpGet]
        public async Task<ActionResult<IEnumerable<object>>> GetMotos(int page = 1, int pageSize = 10)
        {
            var motos = await _context.Motos
                .Include(m => m.Patio)
                .Include(m => m.Statuses)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var result = motos.Select(m => new
            {
                m.IdMoto,
                m.Placa,
                m.Modelo,
                Status = (m.Statuses ?? Enumerable.Empty<RegistroStatus>())
                            .OrderByDescending(s => s.DataStatus)
                            .FirstOrDefault()?.TipoStatus,
                Links = new[]
                {
                    new { Rel = "self", Href = $"/api/motos/{m.IdMoto}" },
                    new { Rel = "update", Href = $"/api/motos/{m.IdMoto}" },
                    new { Rel = "delete", Href = $"/api/motos/{m.IdMoto}" }
                }
            });

            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<object>> GetMoto(int id)
        {
            var m = await _context.Motos
                .Include(m => m.Patio)
                .Include(m => m.Statuses)
                .FirstOrDefaultAsync(m => m.IdMoto == id);

            if (m == null) return NotFound(new { Message = "Moto não encontrada." });

            var result = new
            {
                m.IdMoto,
                m.Placa,
                m.Modelo,
                Status = (m.Statuses ?? Enumerable.Empty<RegistroStatus>())
                            .OrderByDescending(s => s.DataStatus)
                            .FirstOrDefault()?.TipoStatus,
                Links = new[]
                {
                    new { Rel = "self", Href = $"/api/motos/{m.IdMoto}" },
                    new { Rel = "update", Href = $"/api/motos/{m.IdMoto}" },
                    new { Rel = "delete", Href = $"/api/motos/{m.IdMoto}" }
                }
            };

            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<object>> CreateMoto([FromBody] Moto m)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            _context.Motos.Add(m);
            await _context.SaveChangesAsync();

            var result = new
            {
                m.IdMoto,
                m.Placa,
                m.Modelo,
                Status = (m.Statuses ?? Enumerable.Empty<RegistroStatus>())
                            .OrderByDescending(s => s.DataStatus)
                            .FirstOrDefault()?.TipoStatus,
                Links = new[]
                {
                    new { Rel = "self", Href = $"/api/motos/{m.IdMoto}" },
                    new { Rel = "update", Href = $"/api/motos/{m.IdMoto}" },
                    new { Rel = "delete", Href = $"/api/motos/{m.IdMoto}" }
                }
            };

            return CreatedAtAction(nameof(GetMoto), new { id = m.IdMoto }, result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateMoto(int id, [FromBody] Moto m)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            if (id != m.IdMoto) return BadRequest(new { Message = "ID da moto inválido." });

            _context.Entry(m).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Motos.Any(x => x.IdMoto == id))
                    return NotFound(new { Message = "Moto não encontrada." });
                throw;
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMoto(int id)
        {
            var m = await _context.Motos.FindAsync(id);
            if (m == null) return NotFound(new { Message = "Moto não encontrada." });

            _context.Motos.Remove(m);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
