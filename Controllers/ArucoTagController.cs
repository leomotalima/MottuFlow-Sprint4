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
    [Route("api/aruco-tags")]
    [Tags("ArucoTag")]
    public class ArucoTagController : ControllerBase
    {
        private readonly AppDbContext _context;
        public ArucoTagController(AppDbContext context) => _context = context;

        private void AddHateoasLinks(ArucoTagResource resource, int id)
        {
            resource.AddLink(new Link { Href = Url.Link(nameof(GetArucoTag), new { id }), Rel = "self", Method = "GET" });
            resource.AddLink(new Link { Href = Url.Link(nameof(UpdateArucoTag), new { id }), Rel = "update", Method = "PUT" });
            resource.AddLink(new Link { Href = Url.Link(nameof(DeleteArucoTag), new { id }), Rel = "delete", Method = "DELETE" });
        }

        [HttpGet(Name = "GetArucoTags")]
        [SwaggerOperation(Summary = "Lista todas as ArucoTags com paginação e HATEOAS")]
        public async Task<IActionResult> GetArucoTags(int page = 1, int pageSize = 10)
        {
            page = Math.Max(1, page);
            pageSize = Math.Max(1, pageSize);

            var totalItems = await _context.ArucoTags.CountAsync();

            var tags = await _context.ArucoTags
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var data = tags.Select(t =>
            {
                var resource = new ArucoTagResource
                {
                    Id = t.IdTag,
                    Codigo = t.Codigo,
                    Status = t.Status,
                    IdMoto = t.IdMoto
                };
                AddHateoasLinks(resource, t.IdTag);
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

        [HttpGet("{id}", Name = "GetArucoTag")]
        [SwaggerOperation(Summary = "Retorna uma ArucoTag por ID")]
        public async Task<ActionResult<ArucoTagResource>> GetArucoTag(int id)
        {
            var t = await _context.ArucoTags.FindAsync(id);
            if (t == null) return NotFound(new { Message = "ArucoTag não encontrada." });

            var resource = new ArucoTagResource
            {
                Id = t.IdTag,
                Codigo = t.Codigo,
                Status = t.Status,
                IdMoto = t.IdMoto
            };
            AddHateoasLinks(resource, t.IdTag);

            return Ok(resource);
        }

        [HttpPost(Name = "CreateArucoTag")]
        [SwaggerOperation(Summary = "Cria uma nova ArucoTag")]
        public async Task<ActionResult<ArucoTagResource>> CreateArucoTag([FromBody] ArucoTagInputDTO input)
        {
            var t = new ArucoTag
            {
                Codigo = input.Codigo,
                Status = input.Status,
                IdMoto = input.IdMoto
            };

            _context.ArucoTags.Add(t);
            await _context.SaveChangesAsync();

            var resource = new ArucoTagResource
            {
                Id = t.IdTag,
                Codigo = t.Codigo,
                Status = t.Status,
                IdMoto = t.IdMoto
            };
            AddHateoasLinks(resource, t.IdTag);

            return CreatedAtAction(nameof(GetArucoTag), new { id = t.IdTag }, resource);
        }

        [HttpPut("{id}", Name = "UpdateArucoTag")]
        [SwaggerOperation(Summary = "Atualiza uma ArucoTag existente")]
        public async Task<IActionResult> UpdateArucoTag(int id, [FromBody] ArucoTagInputDTO input)
        {
            var t = await _context.ArucoTags.FindAsync(id);
            if (t == null) return NotFound(new { Message = "ArucoTag não encontrada." });

            t.Codigo = input.Codigo;
            t.Status = input.Status;
            t.IdMoto = input.IdMoto;

            _context.Entry(t).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}", Name = "DeleteArucoTag")]
        [SwaggerOperation(Summary = "Deleta uma ArucoTag existente")]
        public async Task<IActionResult> DeleteArucoTag(int id)
        {
            var t = await _context.ArucoTags.FindAsync(id);
            if (t == null) return NotFound(new { Message = "ArucoTag não encontrada." });

            _context.ArucoTags.Remove(t);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
