using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MottuFlowApi.Data;
using MottuFlowApi.Models;
using MottuFlowApi.DTOs;
using MottuFlow.Hateoas;
using Swashbuckle.AspNetCore.Annotations;

namespace MottuFlowApi.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/patios")]
    [Tags("Pátios")]
    [Produces("application/json")] // ✅ Garante que o Swagger exiba JSON
    public class PatioController : ControllerBase
    {
        private readonly AppDbContext _context;
        public PatioController(AppDbContext context) => _context = context;

        // 🔗 Adiciona links HATEOAS
        private void AddHateoasLinks(PatioResource resource, int id)
        {
            resource.AddLink(new Link { Href = Url.Link(nameof(GetPatio), new { id })!, Rel = "self", Method = "GET" });
            resource.AddLink(new Link { Href = Url.Link(nameof(UpdatePatio), new { id })!, Rel = "update", Method = "PUT" });
            resource.AddLink(new Link { Href = Url.Link(nameof(DeletePatio), new { id })!, Rel = "delete", Method = "DELETE" });
        }

        // 🧩 GET - Todos os pátios
        [HttpGet(Name = "GetPatios")]
        [SwaggerOperation(Summary = "Lista todos os pátios com paginação e links HATEOAS")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetPatios(int page = 1, int pageSize = 10)
        {
            page = Math.Max(page, 1);
            pageSize = Math.Max(pageSize, 1);

            var totalItems = await _context.Patios.CountAsync();

            var patios = await _context.Patios
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(p => new PatioResource
                {
                    Id = p.IdPatio,
                    Nome = p.Nome!,
                    Endereco = p.Endereco!,
                    CapacidadeMaxima = p.CapacidadeMaxima
                })
                .ToListAsync();

            if (!patios.Any())
                return Ok(new { success = true, message = "Nenhum pátio encontrado.", data = new List<PatioResource>() });

            patios.ForEach(p => AddHateoasLinks(p, p.Id));

            var meta = new
            {
                totalItems,
                page,
                pageSize,
                totalPages = Math.Ceiling((double)totalItems / pageSize)
            };

            return Ok(new { success = true, meta, data = patios });
        }

        // 🧩 GET - Por ID
        [HttpGet("{id}", Name = "GetPatio")]
        [SwaggerOperation(Summary = "Retorna os dados de um pátio específico pelo ID")]
        [ProducesResponseType(typeof(PatioResource), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetPatio(int id)
        {
            var patio = await _context.Patios
                .Where(p => p.IdPatio == id)
                .Select(p => new PatioResource
                {
                    Id = p.IdPatio,
                    Nome = p.Nome!,
                    Endereco = p.Endereco!,
                    CapacidadeMaxima = p.CapacidadeMaxima
                })
                .FirstOrDefaultAsync();

            if (patio == null)
                return NotFound(new { success = false, message = "Pátio não encontrado." });

            AddHateoasLinks(patio, patio.Id);
            return Ok(new { success = true, data = patio });
        }

        // 🧩 POST - Criar novo pátio
        [HttpPost(Name = "CreatePatio")]
        [SwaggerOperation(Summary = "Cria um novo pátio no sistema")]
        [ProducesResponseType(typeof(PatioResource), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreatePatio([FromBody] PatioInputDTO input)
        {
            if (input == null)
                return BadRequest(new { success = false, message = "Input não pode ser nulo." });

            var patio = new Patio
            {
                Nome = input.Nome,
                Endereco = input.Endereco,
                CapacidadeMaxima = input.CapacidadeMaxima
            };

            _context.Patios.Add(patio);
            await _context.SaveChangesAsync();

            var resource = new PatioResource
            {
                Id = patio.IdPatio,
                Nome = patio.Nome,
                Endereco = patio.Endereco,
                CapacidadeMaxima = patio.CapacidadeMaxima
            };

            AddHateoasLinks(resource, patio.IdPatio);

            return CreatedAtAction(nameof(GetPatio), new { id = patio.IdPatio },
                new { success = true, message = "Pátio criado com sucesso.", data = resource });
        }

        // 🧩 PUT - Atualizar pátio
        [HttpPut("{id}", Name = "UpdatePatio")]
        [SwaggerOperation(Summary = "Atualiza um pátio existente")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdatePatio(int id, [FromBody] PatioInputDTO input)
        {
            if (input == null)
                return BadRequest(new { success = false, message = "Input não pode ser nulo." });

            var patio = await _context.Patios.FindAsync(id);
            if (patio == null)
                return NotFound(new { success = false, message = "Pátio não encontrado." });

            patio.Nome = input.Nome;
            patio.Endereco = input.Endereco;
            patio.CapacidadeMaxima = input.CapacidadeMaxima;

            _context.Entry(patio).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            var updated = new PatioResource
            {
                Id = patio.IdPatio,
                Nome = patio.Nome,
                Endereco = patio.Endereco,
                CapacidadeMaxima = patio.CapacidadeMaxima
            };

            AddHateoasLinks(updated, patio.IdPatio);

            return Ok(new { success = true, message = "Pátio atualizado com sucesso.", data = updated });
        }

        // 🧩 DELETE - Remover pátio
        [HttpDelete("{id}", Name = "DeletePatio")]
        [SwaggerOperation(Summary = "Remove um pátio do sistema pelo ID")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeletePatio(int id)
        {
            var patio = await _context.Patios.FindAsync(id);
            if (patio == null)
                return NotFound(new { success = false, message = "Pátio não encontrado." });

            _context.Patios.Remove(patio);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
