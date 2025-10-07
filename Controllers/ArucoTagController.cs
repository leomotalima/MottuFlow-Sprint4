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
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/arucotags")]
    [Tags("ArucoTags")]
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

        // 🧩 GET - Lista todas as ArucoTags
        [HttpGet(Name = "GetArucoTags")]
        [SwaggerOperation(Summary = "Lista todas as ArucoTags registradas")]
        [ProducesResponseType(StatusCodes.Status200OK)]
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

            return Ok(new { success = true, data = tags });
        }

        // 🧩 GET - Retorna uma ArucoTag por ID
        [HttpGet("{id}", Name = "GetArucoTag")]
        [SwaggerOperation(Summary = "Retorna uma ArucoTag específica pelo ID")]
        [ProducesResponseType(typeof(ArucoTagResource), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetArucoTag(int id)
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

            if (tag == null)
                return NotFound(new { success = false, message = "ArucoTag não encontrada." });

            AddHateoasLinks(tag, tag.Id);
            return Ok(new { success = true, data = tag });
        }

        // 🧩 POST - Cria uma nova ArucoTag
        [HttpPost(Name = "CreateArucoTag")]
        [SwaggerOperation(Summary = "Cria uma nova ArucoTag")]
        [ProducesResponseType(typeof(ArucoTagResource), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateArucoTag([FromBody] ArucoTagInputDTO input)
        {
            if (input == null)
                return BadRequest(new { success = false, message = "Input não pode ser nulo." });

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

            return CreatedAtAction(nameof(GetArucoTag), new { id = tag.IdTag }, new { success = true, data = resource });
        }

        // 🧩 PUT - Atualiza uma ArucoTag
        [HttpPut("{id}", Name = "UpdateArucoTag")]
        [SwaggerOperation(Summary = "Atualiza uma ArucoTag existente")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateArucoTag(int id, [FromBody] ArucoTagInputDTO input)
        {
            if (input == null)
                return BadRequest(new { success = false, message = "Input não pode ser nulo." });

            var tag = await _context.ArucoTags.FindAsync(id);
            if (tag == null)
                return NotFound(new { success = false, message = "ArucoTag não encontrada." });

            tag.Codigo = input.Codigo;
            tag.Status = input.Status;
            tag.IdMoto = input.IdMoto;

            _context.Entry(tag).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // 🧩 DELETE - Remove uma ArucoTag
        [HttpDelete("{id}", Name = "DeleteArucoTag")]
        [SwaggerOperation(Summary = "Remove uma ArucoTag pelo ID")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteArucoTag(int id)
        {
            var tag = await _context.ArucoTags.FindAsync(id);
            if (tag == null)
                return NotFound(new { success = false, message = "ArucoTag não encontrada." });

            _context.ArucoTags.Remove(tag);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
