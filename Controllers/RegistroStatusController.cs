using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MottuFlowApi.Data;
using MottuFlow.Models;
using Swashbuckle.AspNetCore.Annotations;

namespace MottuFlowApi.Controllers
{
    [ApiController]
    [Route("api/registro-status")]
    public class RegistroStatusController : ControllerBase
    {
        private readonly AppDbContext _context;
        public RegistroStatusController(AppDbContext context) => _context = context;

        [HttpGet]
        [SwaggerOperation(Summary = "Lista todos os registros de status com paginação")]
        public async Task<ActionResult<IEnumerable<object>>> GetRegistroStatus(int page = 1, int pageSize = 10)
        {
            var registros = await _context.RegistroStatuses
                .Include(r => r.Moto)
                .Include(r => r.Funcionario)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var result = registros.Select(r => new
            {
                r.IdStatus,
                r.TipoStatus,
                r.Descricao,
                r.DataStatus,
                Moto = r.Moto != null ? new { r.Moto.IdMoto, r.Moto.Placa } : null,
                Funcionario = r.Funcionario != null ? new { r.Funcionario.IdFuncionario, r.Funcionario.Nome } : null,
                Links = new[]
                {
                    new { Rel = "self", Href = $"/api/registro-status/{r.IdStatus}" },
                    new { Rel = "update", Href = $"/api/registro-status/{r.IdStatus}" },
                    new { Rel = "delete", Href = $"/api/registro-status/{r.IdStatus}" }
                }
            });

            return Ok(result);
        }

        [HttpGet("{id}")]
        [SwaggerOperation(Summary = "Retorna um registro de status pelo ID")]
        public async Task<ActionResult<object>> GetRegistroStatus(int id)
        {
            var r = await _context.RegistroStatuses
                .Include(rs => rs.Moto)
                .Include(rs => rs.Funcionario)
                .FirstOrDefaultAsync(rs => rs.IdStatus == id);

            if (r == null) return NotFound(new { Message = "Registro não encontrado." });

            var result = new
            {
                r.IdStatus,
                r.TipoStatus,
                r.Descricao,
                r.DataStatus,
                Moto = r.Moto != null ? new { r.Moto.IdMoto, r.Moto.Placa } : null,
                Funcionario = r.Funcionario != null ? new { r.Funcionario.IdFuncionario, r.Funcionario.Nome } : null,
                Links = new[]
                {
                    new { Rel = "self", Href = $"/api/registro-status/{r.IdStatus}" },
                    new { Rel = "update", Href = $"/api/registro-status/{r.IdStatus}" },
                    new { Rel = "delete", Href = $"/api/registro-status/{r.IdStatus}" }
                }
            };

            return Ok(result);
        }

        [HttpPost]
        [SwaggerOperation(Summary = "Cria um novo registro de status")]
        public async Task<ActionResult<object>> CreateRegistroStatus([FromBody] RegistroStatus r)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            _context.RegistroStatuses.Add(r);
            await _context.SaveChangesAsync();

            var result = new
            {
                r.IdStatus,
                r.TipoStatus,
                r.Descricao,
                r.DataStatus,
                Moto = r.Moto != null ? new { r.Moto.IdMoto, r.Moto.Placa } : null,
                Funcionario = r.Funcionario != null ? new { r.Funcionario.IdFuncionario, r.Funcionario.Nome } : null,
                Links = new[]
                {
                    new { Rel = "self", Href = $"/api/registro-status/{r.IdStatus}" },
                    new { Rel = "update", Href = $"/api/registro-status/{r.IdStatus}" },
                    new { Rel = "delete", Href = $"/api/registro-status/{r.IdStatus}" }
                }
            };

            return CreatedAtAction(nameof(GetRegistroStatus), new { id = r.IdStatus }, result);
        }

        [HttpPut("{id}")]
        [SwaggerOperation(Summary = "Atualiza um registro de status")]
        public async Task<IActionResult> UpdateRegistroStatus(int id, [FromBody] RegistroStatus r)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            if (id != r.IdStatus) return BadRequest(new { Message = "ID inválido." });

            _context.Entry(r).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.RegistroStatuses.Any(x => x.IdStatus == id))
                    return NotFound(new { Message = "Registro não encontrado." });
                throw;
            }

            return NoContent();
        }

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
