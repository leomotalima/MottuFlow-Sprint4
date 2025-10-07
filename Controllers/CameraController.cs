using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MottuFlowApi.Data;
using MottuFlow.Models;
using MottuFlowApi.DTOs;
using Swashbuckle.AspNetCore.Annotations;

namespace MottuFlowApi.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/cameras")]
    [Tags("Câmeras")]
    public class CameraController : ControllerBase
    {
        private readonly AppDbContext _context;
        public CameraController(AppDbContext context) => _context = context;

        // 🧩 GET - Todas as câmeras (com paginação)
        [HttpGet(Name = "GetCameras")]
        [SwaggerOperation(Summary = "Lista todas as câmeras com paginação")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetCameras(int page = 1, int pageSize = 10)
        {
            page = Math.Max(page, 1);
            pageSize = Math.Max(pageSize, 1);

            var totalItems = await _context.Cameras.CountAsync();

            var cameras = await _context.Cameras
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var data = cameras.Select(c => new CameraOutputDTO
            {
                IdCamera = c.IdCamera,
                StatusOperacional = c.StatusOperacional,
                LocalizacaoFisica = c.LocalizacaoFisica,
                IdPatio = c.IdPatio
            });

            var meta = new
            {
                totalItems,
                page,
                pageSize,
                totalPages = Math.Ceiling((double)totalItems / pageSize)
            };

            return Ok(new { success = true, meta, data });
        }

        // 🧩 GET - Câmera por ID
        [HttpGet("{id}", Name = "GetCamera")]
        [SwaggerOperation(Summary = "Retorna uma câmera pelo ID")]
        [ProducesResponseType(typeof(CameraOutputDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetCamera(int id)
        {
            var c = await _context.Cameras.FindAsync(id);
            if (c == null)
                return NotFound(new { success = false, message = "Câmera não encontrada." });

            var result = new CameraOutputDTO
            {
                IdCamera = c.IdCamera,
                StatusOperacional = c.StatusOperacional,
                LocalizacaoFisica = c.LocalizacaoFisica,
                IdPatio = c.IdPatio
            };

            return Ok(new { success = true, data = result });
        }

        // 🧩 POST - Criar câmera
        [HttpPost(Name = "CreateCamera")]
        [SwaggerOperation(Summary = "Cria uma nova câmera")]
        [ProducesResponseType(typeof(CameraOutputDTO), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateCamera([FromBody] CameraInputDTO input)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { success = false, message = "Dados inválidos.", errors = ModelState });

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

            return CreatedAtAction(nameof(GetCamera), new { id = camera.IdCamera }, new { success = true, data = result });
        }

        // 🧩 PUT - Atualizar câmera
        [HttpPut("{id}", Name = "UpdateCamera")]
        [SwaggerOperation(Summary = "Atualiza uma câmera existente")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateCamera(int id, [FromBody] CameraInputDTO input)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { success = false, message = "Dados inválidos.", errors = ModelState });

            var camera = await _context.Cameras.FindAsync(id);
            if (camera == null)
                return NotFound(new { success = false, message = "Câmera não encontrada." });

            camera.StatusOperacional = input.StatusOperacional;
            camera.LocalizacaoFisica = input.LocalizacaoFisica;
            camera.IdPatio = input.IdPatio;

            _context.Entry(camera).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // 🧩 DELETE - Remover câmera
        [HttpDelete("{id}", Name = "DeleteCamera")]
        [SwaggerOperation(Summary = "Remove uma câmera pelo ID")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteCamera(int id)
        {
            var c = await _context.Cameras.FindAsync(id);
            if (c == null)
                return NotFound(new { success = false, message = "Câmera não encontrada." });

            _context.Cameras.Remove(c);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
