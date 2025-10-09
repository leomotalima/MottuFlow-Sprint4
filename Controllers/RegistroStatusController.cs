using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MottuFlow.DTOs;
using MottuFlowApi.Models;
using MottuFlowApi.Data;
using Swashbuckle.AspNetCore.Annotations;

namespace MottuFlowApi.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/registro-status")]
    [Tags("Registros de Status")]
    [Produces("application/json")] // ✅ Força resposta JSON no Swagger
    public class RegistroStatusController : ControllerBase
    {
        private readonly AppDbContext _context;
        public RegistroStatusController(AppDbContext context) => _context = context;

        // 🧩 GET - Lista com paginação
        [HttpGet(Name = "GetRegistroStatus")]
        [SwaggerOperation(Summary = "Lista todos os registros de status com paginação")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetRegistroStatus(int page = 1, int pageSize = 10)
        {
            page = Math.Max(page, 1);
            pageSize = Math.Max(pageSize, 1);

            var totalItems = await _context.RegistroStatuses.CountAsync();

            var registros = await _context.RegistroStatuses
                .OrderByDescending(r => r.DataStatus)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            if (!registros.Any())
                return Ok(new { success = true, message = "Nenhum registro de status encontrado.", data = new List<StatusDTO>() });

            var data = registros.Select(r => new StatusDTO
            {
                TipoStatus = r.TipoStatus,
                Descricao = r.Descricao ?? string.Empty,
                DataStatus = r.DataStatus,
                IdMoto = r.IdMoto,
                IdFuncionario = r.IdFuncionario
            }).ToList();

            var meta = new
            {
                totalItems,
                page,
                pageSize,
                totalPages = Math.Ceiling((double)totalItems / pageSize)
            };

            return Ok(new { success = true, meta, data });
        }

        // 🧩 GET - Por ID
        [HttpGet("{id}", Name = "GetRegistroStatusById")]
        [SwaggerOperation(Summary = "Retorna os dados de um registro de status específico pelo ID")]
        [ProducesResponseType(typeof(StatusDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetRegistroStatusById(int id)
        {
            var r = await _context.RegistroStatuses.FindAsync(id);
            if (r == null)
                return NotFound(new { success = false, message = "Registro de status não encontrado." });

            var dto = new StatusDTO
            {
                TipoStatus = r.TipoStatus,
                Descricao = r.Descricao ?? string.Empty,
                DataStatus = r.DataStatus,
                IdMoto = r.IdMoto,
                IdFuncionario = r.IdFuncionario
            };

            return Ok(new { success = true, data = dto });
        }

        // 🧩 POST - Criar novo registro
        [HttpPost(Name = "CreateRegistroStatus")]
        [SwaggerOperation(Summary = "Cria um novo registro de status no sistema")]
        [ProducesResponseType(typeof(StatusDTO), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateRegistroStatus([FromBody] StatusDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { success = false, message = "Dados inválidos.", errors = ModelState });

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

            var result = new StatusDTO
            {
                TipoStatus = r.TipoStatus,
                Descricao = r.Descricao,
                DataStatus = r.DataStatus,
                IdMoto = r.IdMoto,
                IdFuncionario = r.IdFuncionario
            };

            return CreatedAtAction(nameof(GetRegistroStatusById), new { id = r.IdStatus },
                new { success = true, message = "Registro de status criado com sucesso.", data = result });
        }

        // 🧩 PUT - Atualizar registro existente
        [HttpPut("{id}", Name = "UpdateRegistroStatus")]
        [SwaggerOperation(Summary = "Atualiza um registro de status existente pelo ID")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateRegistroStatus(int id, [FromBody] StatusDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { success = false, message = "Dados inválidos.", errors = ModelState });

            var r = await _context.RegistroStatuses.FindAsync(id);
            if (r == null)
                return NotFound(new { success = false, message = "Registro de status não encontrado." });

            r.TipoStatus = dto.TipoStatus;
            r.Descricao = dto.Descricao;
            r.DataStatus = dto.DataStatus;
            r.IdMoto = dto.IdMoto;
            r.IdFuncionario = dto.IdFuncionario;

            _context.Entry(r).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            var updated = new StatusDTO
            {
                TipoStatus = r.TipoStatus,
                Descricao = r.Descricao,
                DataStatus = r.DataStatus,
                IdMoto = r.IdMoto,
                IdFuncionario = r.IdFuncionario
            };

            return Ok(new { success = true, message = "Registro de status atualizado com sucesso.", data = updated });
        }

        // 🧩 DELETE - Remover registro
        [HttpDelete("{id}", Name = "DeleteRegistroStatus")]
        [SwaggerOperation(Summary = "Remove um registro de status existente pelo ID")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteRegistroStatus(int id)
        {
            var r = await _context.RegistroStatuses.FindAsync(id);
            if (r == null)
                return NotFound(new { success = false, message = "Registro de status não encontrado." });

            _context.RegistroStatuses.Remove(r);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
