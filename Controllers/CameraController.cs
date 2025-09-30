using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MottuFlowApi.Data;
using MottuFlow.Models;
using MottuFlowApi.DTOs;
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
        public async Task<ActionResult<IEnumerable<CameraOutputDTO>>> GetCameras(int page = 1, int pageSize = 10)
        {
            var cameras = await _context.Cameras
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var result = cameras.Select(c => new CameraOutputDTO
            {
                IdCamera = c.IdCamera,
                StatusOperacional = c.StatusOperacional,
                LocalizacaoFisica = c.LocalizacaoFisica,
                IdPatio = c.IdPatio
            });

            return Ok(result);
        }

        [HttpGet("{id}")]
        [SwaggerOperation(Summary = "Retorna uma câmera pelo ID")]
        public async Task<ActionResult<CameraOutputDTO>> GetCamera(int id)
        {
            var c = await _context.Cameras.FindAsync(id);
            if (c == null) return NotFound(new { Message = "Câmera não encontrada." });

            var result = new CameraOutputDTO
            {
                IdCamera = c.IdCamera,
                StatusOperacional = c.StatusOperacional,
                LocalizacaoFisica = c.LocalizacaoFisica,
                IdPatio = c.IdPatio
            };

            return Ok(result);
        }

        [HttpPost]
        [SwaggerOperation(Summary = "Cria uma nova câmera")]
        public async Task<ActionResult<CameraOutputDTO>> CreateCamera([FromBody] CameraInputDTO input)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var camera = new Camera
            {
                StatusOperacional = input.StatusOperacional,
                LocalizacaoFisica = input.LocalizacaoFisica,
                IdPatio = input.IdPatio
            };

            _context.Cameras.Add(camera);
            await _context.SaveChangesAsync();

            var result = new CameraOutputDTO
            {
                IdCamera = camera.IdCamera,
                StatusOperacional = camera.StatusOperacional,
                LocalizacaoFisica = camera.LocalizacaoFisica,
                IdPatio = camera.IdPatio
            };

            return CreatedAtAction(nameof(GetCamera), new { id = camera.IdCamera }, result);
        }

        [HttpPut("{id}")]
        [SwaggerOperation(Summary = "Atualiza uma câmera")]
        public async Task<IActionResult> UpdateCamera(int id, [FromBody] CameraInputDTO input)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var camera = await _context.Cameras.FindAsync(id);
            if (camera == null) return NotFound(new { Message = "Câmera não encontrada." });

            camera.StatusOperacional = input.StatusOperacional;
            camera.LocalizacaoFisica = input.LocalizacaoFisica;
            camera.IdPatio = input.IdPatio;

            _context.Entry(camera).State = EntityState.Modified;
            await _context.SaveChangesAsync();

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
