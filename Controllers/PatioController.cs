using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MottuFlowApi.Data;
using MottuFlow.Models;
using MottuFlow.Hateoas;
using MottuFlowApi.DTOs;
using Swashbuckle.AspNetCore.Annotations;

namespace MottuFlowApi.Controllers
{
    [ApiController]
    [Route("api/patios")]
    [Tags("Patio")]
    public class PatioController : ControllerBase
    {
        private readonly AppDbContext _context;
        public PatioController(AppDbContext context) => _context = context;

        private void AddHateoasLinks(PatioResource patioResource, int id)
        {
            patioResource.AddLink(new Link { Href = Url.Link(nameof(GetPatio), new { id }), Rel = "self", Method = "GET" });
            patioResource.AddLink(new Link { Href = Url.Link(nameof(UpdatePatio), new { id }), Rel = "update", Method = "PUT" });
            patioResource.AddLink(new Link { Href = Url.Link(nameof(DeletePatio), new { id }), Rel = "delete", Method = "DELETE" });
        }

        [HttpGet(Name = "GetPatios")]
        [SwaggerOperation(Summary = "Lista todos os pátios com paginação e HATEOAS")]
        public async Task<IActionResult> GetPatios(int page = 1, int pageSize = 10)
        {
            page = page < 1 ? 1 : page;
            pageSize = pageSize < 1 ? 10 : pageSize;

            var totalItems = await _context.Patios.CountAsync();

            var patios = await _context.Patios
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var data = patios.Select(p =>
            {
                var resource = new PatioResource
                {
                    Id = p.IdPatio,
                    Nome = p.Nome ?? string.Empty,
                    Endereco = p.Endereco ?? string.Empty,
                    CapacidadeMaxima = p.CapacidadeMaxima
                };
                AddHateoasLinks(resource, p.IdPatio);
                return resource;
            }).ToList();

            var meta = new
            {
                totalItems,
                page,
                pageSize,
                totalPages = Math.Ceiling((double)totalItems / pageSize)
            };

            return Ok(new { meta, data });
        }

        [HttpGet("{id}", Name = "GetPatio")]
        [SwaggerOperation(Summary = "Retorna pátio por ID")]
        public async Task<ActionResult<PatioResource>> GetPatio(int id)
        {
            var p = await _context.Patios.FindAsync(id);
            if (p == null) return NotFound(new { Message = "Pátio não encontrado." });

            var resource = new PatioResource
            {
                Id = p.IdPatio,
                Nome = p.Nome ?? string.Empty,
                Endereco = p.Endereco ?? string.Empty,
                CapacidadeMaxima = p.CapacidadeMaxima
            };
            AddHateoasLinks(resource, p.IdPatio);

            return Ok(resource);
        }

        [HttpPost(Name = "CreatePatio")]
        [SwaggerOperation(Summary = "Cria um novo pátio")]
        public async Task<ActionResult<PatioResource>> CreatePatio([FromBody] PatioInputDTO input)
        {
            var p = new Patio
            {
                Nome = input.Nome,
                Endereco = input.Endereco,
                CapacidadeMaxima = input.CapacidadeMaxima
            };

            _context.Patios.Add(p);
            await _context.SaveChangesAsync();

            var resource = new PatioResource
            {
                Id = p.IdPatio,
                Nome = p.Nome,
                Endereco = p.Endereco,
                CapacidadeMaxima = p.CapacidadeMaxima
            };
            AddHateoasLinks(resource, p.IdPatio);

            return CreatedAtAction(nameof(GetPatio), new { id = p.IdPatio }, resource);
        }

        [HttpPut("{id}", Name = "UpdatePatio")]
        [SwaggerOperation(Summary = "Atualiza um pátio")]
        public async Task<IActionResult> UpdatePatio(int id, [FromBody] PatioInputDTO input)
        {
            var p = await _context.Patios.FindAsync(id);
            if (p == null) return NotFound(new { Message = "Pátio não encontrado." });

            p.Nome = input.Nome ?? p.Nome;
            p.Endereco = input.Endereco ?? p.Endereco;
            p.CapacidadeMaxima = input.CapacidadeMaxima;

            _context.Entry(p).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}", Name = "DeletePatio")]
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
