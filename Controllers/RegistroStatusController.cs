using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MottuFlow.DTOs;
using MottuFlow.Models;
using MottuFlowApi.Data;
using Swashbuckle.AspNetCore.Annotations;

namespace MottuFlowApi.Controllers
{
    [ApiController]
    [Route("api/registro-status")]
    public class RegistroStatusController : ControllerBase
    {
        private readonly AppDbContext _context;
        public RegistroStatusController(AppDbContext context) => _context = context;

        // GET com paginação
        [HttpGet]
        [SwaggerOperation(Summary = "Lista todos os registros de status com paginação")]
        public async Task<ActionResult<IEnumerable<StatusDTO>>> GetRegistroStatus(int page = 1, int pageSize = 10)
        {
            var registros = await _context.RegistroStatuses
                .OrderBy(r => r.DataStatus)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var registrosDTO = registros.Select(r => new StatusDTO
            {
                TipoStatus = r.TipoStatus,
                Descricao = r.Descricao ?? string.Empty,
                DataStatus = r.DataStatus,
                IdMoto = r.IdMoto,
                IdFuncionario = r.IdFuncionario
            }).ToList();

            return Ok(registrosDTO);
        }

        // GET por ID
        [HttpGet("{id}")]
        [SwaggerOperation(Summary = "Retorna um registro de status pelo ID")]
        public async Task<ActionResult<StatusDTO>> GetRegistroStatus(int id)
        {
            var r = await _context.RegistroStatuses.FindAsync(id);
            if (r == null) return NotFound(new { Message = "Registro não encontrado." });

            var dto = new StatusDTO
            {
                TipoStatus = r.TipoStatus,
                Descricao = r.Descricao ?? string.Empty,
                DataStatus = r.DataStatus,
                IdMoto = r.IdMoto,
                IdFuncionario = r.IdFuncionario
            };

            return Ok(dto);
        }

        // POST
        [HttpPost]
        [SwaggerOperation(Summary = "Cria um novo registro de status")]
        public async Task<ActionResult<StatusDTO>> CreateRegistroStatus([FromBody] StatusDTO dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var r = new RegistroStatus
            {
                TipoStatus = dto.TipoStatus,
                Descricao = dto.Descricao,
                DataStatus = dto.DataStatus,
                IdMoto = dto.IdMoto,
                IdFuncionario = dto.IdFuncionario
            };

            _context.RegistroStatuses.Add(r);
            await _context.SaveChangesAsync();

            // Retorna o DTO minimalista
            return CreatedAtAction(nameof(GetRegistroStatus), new { id = r.IdStatus }, dto);
        }

        // PUT
        [HttpPut("{id}")]
        [SwaggerOperation(Summary = "Atualiza um registro de status")]
        public async Task<IActionResult> UpdateRegistroStatus(int id, [FromBody] StatusDTO dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var r = await _context.RegistroStatuses.FindAsync(id);
            if (r == null) return NotFound(new { Message = "Registro não encontrado." });

            r.TipoStatus = dto.TipoStatus;
            r.Descricao = dto.Descricao;
            r.DataStatus = dto.DataStatus;
            r.IdMoto = dto.IdMoto;
            r.IdFuncionario = dto.IdFuncionario;

            _context.Entry(r).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE
        [HttpDelete("{id}")]
        [SwaggerOperation(Summary = "Deleta um registro de status")]
        public async Task<IActionResult> DeleteRegistroStatus(int id)
        {
            var r = await _context.RegistroStatuses.FindAsync(id);
            if (r == null) return NotFound(new { Message = "Registro não encontrado." });

            _context.RegistroStatuses.Remove(r);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
