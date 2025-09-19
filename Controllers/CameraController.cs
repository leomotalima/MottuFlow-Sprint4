using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MottuFlowApi.Data;
using MottuFlowApi.Models;
using Swashbuckle.AspNetCore.Annotations;

namespace MottuFlowApi.Controllers
{
    [ApiController]
    [Route("api/cameras")]
    public class CameraController : ControllerBase
    {
        private readonly AppDbContext _context;
        public CameraController(AppDbContext context) => _context = context;

        [HttpGet]
        [SwaggerOperation(Summary = "Lista todas as câmeras com paginação")]
        public async Task<ActionResult<IEnumerable<object>>> GetCameras(int page = 1, int pageSize = 10)
        {
            var cameras = await _context.Cameras
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var result = cameras.Select(c => new
            {
                c.IdCamera,
                c.StatusOperacional,
                c.LocalizacaoFisica,
                Links = new object[]
                {
                    new { Rel = "self", Href = $"/api/cameras/{c.IdCamera}" },
                    new { Rel = "update", Href = $"/api/cameras/{c.IdCamera}" },
                    new { Rel = "delete", Href = $"/api/cameras/{c.IdCamera}" }
                }
            });

            return Ok(result);
        }

        [HttpGet("{id}")]
        [SwaggerOperation(Summary = "Retorna uma câmera pelo ID")]
        public async Task<ActionResult<object>> GetCamera(int id)
        {
            var c = await _context.Cameras.FindAsync(id);
            if (c == null) return NotFound(new { Message = "Câmera não encontrada." });

            var result = new
            {
                c.IdCamera,
                c.StatusOperacional,
                c.LocalizacaoFisica,
                Links = new object[]
                {
                    new { Rel = "self", Href = $"/api/cameras/{c.IdCamera}" },
                    new { Rel = "update", Href = $"/api/cameras/{c.IdCamera}" },
                    new { Rel = "delete", Href = $"/api/cameras/{c.IdCamera}" }
                }
            };

            return Ok(result);
        }

        [HttpPost]
        [SwaggerOperation(Summary = "Cria uma nova câmera")]
        public async Task<ActionResult<object>> CreateCamera([FromBody] Camera c)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            _context.Cameras.Add(c);
            await _context.SaveChangesAsync();

            var result = new
            {
                c.IdCamera,
                c.StatusOperacional,
                c.LocalizacaoFisica,
                Links = new object[]
                {
                    new { Rel = "self", Href = $"/api/cameras/{c.IdCamera}" },
                    new { Rel = "update", Href = $"/api/cameras/{c.IdCamera}" },
                    new { Rel = "delete", Href = $"/api/cameras/{c.IdCamera}" }
                }
            };

            return CreatedAtAction(nameof(GetCamera), new { id = c.IdCamera }, result);
        }

        [HttpPut("{id}")]
        [SwaggerOperation(Summary = "Atualiza uma câmera")]
        public async Task<IActionResult> UpdateCamera(int id, [FromBody] Camera c)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            if (id != c.IdCamera) return BadRequest(new { Message = "ID inválido." });

            _context.Entry(c).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Cameras.Any(x => x.IdCamera == id))
                    return NotFound(new { Message = "Câmera não encontrada." });
                throw;
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        [SwaggerOperation(Summary = "Deleta uma câmera")]
        public async Task<IActionResult> DeleteCamera(int id)
        {
            var c = await _context.Cameras.FindAsync(id);
            if (c == null) return NotFound(new { Message = "Câmera não encontrada." });

            _context.Cameras.Remove(c);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
