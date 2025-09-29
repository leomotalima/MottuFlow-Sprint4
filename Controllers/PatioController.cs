using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MottuFlowApi.Data;
using MottuFlow.Models;
using MottuFlowApi.DTOs;
using MottuFlow.Hateoas;
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

        private void AddHateoasLinks(PatioResource resource, int id)
        {
            resource.AddLink(new Link { Href = Url.Link(nameof(GetPatio), new { id })!, Rel = "self", Method = "GET" });
            resource.AddLink(new Link { Href = Url.Link(nameof(UpdatePatio), new { id })!, Rel = "update", Method = "PUT" });
            resource.AddLink(new Link { Href = Url.Link(nameof(DeletePatio), new { id })!, Rel = "delete", Method = "DELETE" });
        }

        [HttpGet(Name = "GetPatios")]
        [SwaggerOperation(Summary = "Lista todos os pátios")]
        public async Task<IActionResult> GetPatios()
        {
            var patios = await _context.Patios
                .Select(p => new PatioResource
                {
                    Id = p.IdPatio,
                    Nome = p.Nome!,
                    Endereco = p.Endereco!,
                    CapacidadeMaxima = p.CapacidadeMaxima
                })
                .ToListAsync();

            patios.ForEach(p => AddHateoasLinks(p, p.Id));
            return Ok(patios);
        }

        [HttpGet("{id}", Name = "GetPatio")]
        [SwaggerOperation(Summary = "Retorna um pátio por ID")]
        public async Task<ActionResult<PatioResource>> GetPatio(int id)
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

            if (patio == null) return NotFound(new { Message = "Pátio não encontrado." });

            AddHateoasLinks(patio, patio.Id);
            return Ok(patio);
        }

        [HttpPost(Name = "CreatePatio")]
        [SwaggerOperation(Summary = "Cria um novo pátio")]
        public async Task<ActionResult<PatioResource>> CreatePatio([FromBody] PatioInputDTO input)
        {
            if (input == null) return BadRequest("Input não pode ser nulo.");

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

            return CreatedAtAction(nameof(GetPatio), new { id = patio.IdPatio }, resource);
        }

        [HttpPut("{id}", Name = "UpdatePatio")]
        [SwaggerOperation(Summary = "Atualiza um pátio")]
        public async Task<IActionResult> UpdatePatio(int id, [FromBody] PatioInputDTO input)
        {
            if (input == null) return BadRequest("Input não pode ser nulo.");

            var patio = await _context.Patios.FindAsync(id);
            if (patio == null) return NotFound(new { Message = "Pátio não encontrado." });

            patio.Nome = input.Nome;
            patio.Endereco = input.Endereco;
            patio.CapacidadeMaxima = input.CapacidadeMaxima;

            _context.Entry(patio).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}", Name = "DeletePatio")]
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
