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
    [Route("api/arucotags")]
    [Tags("ArucoTag")]
    public class ArucoTagController : ControllerBase
    {
        private readonly AppDbContext _context;
        public ArucoTagController(AppDbContext context) => _context = context;

        private void AddHateoasLinks(ArucoTagResource resource, int id)
        {
            resource.AddLink(new Link { Href = Url.Link(nameof(GetArucoTag), new { id })!, Rel = "self", Method = "GET" });
            resource.AddLink(new Link { Href = Url.Link(nameof(UpdateArucoTag), new { id })!, Rel = "update", Method = "PUT" });
            resource.AddLink(new Link { Href = Url.Link(nameof(DeleteArucoTag), new { id })!, Rel = "delete", Method = "DELETE" });
        }

        [HttpGet(Name = "GetArucoTags")]
        [SwaggerOperation(Summary = "Lista todas as ArucoTags")]
        public async Task<IActionResult> GetArucoTags()
        {
            var tags = await _context.ArucoTags
                .Select(t => new ArucoTagResource
                {
                    Id = t.IdTag,
                    Codigo = t.Codigo!,
                    Status = t.Status!,
                    IdMoto = t.IdMoto
                })
                .ToListAsync();

            tags.ForEach(t => AddHateoasLinks(t, t.Id));
            return Ok(tags);
        }

        [HttpGet("{id}", Name = "GetArucoTag")]
        [SwaggerOperation(Summary = "Retorna uma ArucoTag por ID")]
        public async Task<ActionResult<ArucoTagResource>> GetArucoTag(int id)
        {
            var tag = await _context.ArucoTags
                .Where(t => t.IdTag == id)
                .Select(t => new ArucoTagResource
                {
                    Id = t.IdTag,
                    Codigo = t.Codigo!,
                    Status = t.Status!,
                    IdMoto = t.IdMoto
                })
                .FirstOrDefaultAsync();

            if (tag == null) return NotFound(new { Message = "Tag não encontrada." });
            AddHateoasLinks(tag, tag.Id);
            return Ok(tag);
        }

        [HttpPost(Name = "CreateArucoTag")]
        [SwaggerOperation(Summary = "Cria uma nova ArucoTag")]
        public async Task<ActionResult<ArucoTagResource>> CreateArucoTag([FromBody] ArucoTagInputDTO input)
        {
            if (input == null) return BadRequest("Input não pode ser nulo.");

            var tag = new ArucoTag
            {
                Codigo = input.Codigo,
                Status = input.Status,
                IdMoto = input.IdMoto
            };

            _context.ArucoTags.Add(tag);
            await _context.SaveChangesAsync();

            var resource = new ArucoTagResource
            {
                Id = tag.IdTag,
                Codigo = tag.Codigo,
                Status = tag.Status,
                IdMoto = tag.IdMoto
            };
            AddHateoasLinks(resource, tag.IdTag);

            return CreatedAtAction(nameof(GetArucoTag), new { id = tag.IdTag }, resource);
        }

        [HttpPut("{id}", Name = "UpdateArucoTag")]
        [SwaggerOperation(Summary = "Atualiza uma ArucoTag")]
        public async Task<IActionResult> UpdateArucoTag(int id, [FromBody] ArucoTagInputDTO input)
        {
            if (input == null) return BadRequest("Input não pode ser nulo.");

            var tag = await _context.ArucoTags.FindAsync(id);
            if (tag == null) return NotFound(new { Message = "Tag não encontrada." });

            tag.Codigo = input.Codigo;
            tag.Status = input.Status;
            tag.IdMoto = input.IdMoto;

            _context.Entry(tag).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}", Name = "DeleteArucoTag")]
        [SwaggerOperation(Summary = "Deleta uma ArucoTag")]
        public async Task<IActionResult> DeleteArucoTag(int id)
        {
            var tag = await _context.ArucoTags.FindAsync(id);
            if (tag == null) return NotFound(new { Message = "Tag não encontrada." });

            _context.ArucoTags.Remove(tag);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
