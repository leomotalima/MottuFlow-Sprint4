using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MottuFlowApi.Data;
using MottuFlowApi.DTOs;
using MottuFlow.Models;
using Swashbuckle.AspNetCore.Annotations;

namespace MottuFlowApi.Controllers
{
    [ApiController]
    [Route("api/patios")]
    public class PatioController : ControllerBase
    {
        private readonly AppDbContext _context;
        public PatioController(AppDbContext context) => _context = context;

        // GET /api/patios?page=1&pageSize=10
        [HttpGet]
        [SwaggerOperation(Summary = "Lista todos os pátios com paginação")]
        public async Task<ActionResult<IEnumerable<PatioOutputDTO>>> GetPatios(int page = 1, int pageSize = 10)
        {
            var patios = await _context.Patios
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var result = patios.Select(p => new PatioOutputDTO
            {
                IdPatio = p.IdPatio,
                Nome = p.Nome,
                Endereco = p.Endereco,
                CapacidadeMaxima = p.CapacidadeMaxima
            });

            return Ok(result);
        }

        // GET /api/patios/{id}
        [HttpGet("{id}")]
        [SwaggerOperation(Summary = "Retorna pátio por ID")]
        public async Task<ActionResult<PatioOutputDTO>> GetPatio(int id)
        {
            var p = await _context.Patios.FindAsync(id);
            if (p == null) return NotFound(new { Message = "Pátio não encontrado." });

            var result = new PatioOutputDTO
            {
                IdPatio = p.IdPatio,
                Nome = p.Nome,
                Endereco = p.Endereco,
                CapacidadeMaxima = p.CapacidadeMaxima
            };

            return Ok(result);
        }

        // POST /api/patios
        [HttpPost]
        [SwaggerOperation(Summary = "Cria um novo pátio")]
        public async Task<ActionResult<PatioOutputDTO>> CreatePatio([FromBody] PatioInputDTO patioInput)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var patio = new Patio
            {
                Nome = patioInput.Nome,
                Endereco = patioInput.Endereco,
                CapacidadeMaxima = patioInput.CapacidadeMaxima
            };

            _context.Patios.Add(patio);
            await _context.SaveChangesAsync();

            var result = new PatioOutputDTO
            {
                IdPatio = patio.IdPatio,
                Nome = patio.Nome,
                Endereco = patio.Endereco,
                CapacidadeMaxima = patio.CapacidadeMaxima
            };

            return CreatedAtAction(nameof(GetPatio), new { id = patio.IdPatio }, result);
        }

        // PUT /api/patios/{id}
        [HttpPut("{id}")]
        [SwaggerOperation(Summary = "Atualiza um pátio")]
        public async Task<IActionResult> UpdatePatio(int id, [FromBody] PatioInputDTO patioInput)
        {
            var patio = await _context.Patios.FindAsync(id);
            if (patio == null) return NotFound(new { Message = "Pátio não encontrado." });

            patio.Nome = patioInput.Nome;
            patio.Endereco = patioInput.Endereco;
            patio.CapacidadeMaxima = patioInput.CapacidadeMaxima;

            _context.Entry(patio).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE /api/patios/{id}
        [HttpDelete("{id}")]
        [SwaggerOperation(Summary = "Deleta um pátio")]
        public async Task<IActionResult> DeletePatio(int id)
        {
            var patio = await _context.Patios.FindAsync(id);
            if (patio == null) return NotFound(new { Message = "Pátio não encontrado." });

            _context.Patios.Remove(patio);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
