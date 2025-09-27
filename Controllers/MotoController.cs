using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MottuFlowApi.Data;
using MottuFlow.Models;

using MottuFlow.DTOs;
using MottuFlow.Hateoas;
using Swashbuckle.AspNetCore.Annotations;

namespace MottuFlowApi.Controllers
{
    [ApiController]
    [Route("api/motos")]
    [Tags("Moto")]
    public class MotoController : ControllerBase
    {
        private readonly AppDbContext _context;

        public MotoController(AppDbContext context) => _context = context;

        private void AddHateoasLinks(MotoResource resource, int id)
        {
            resource.AddLink(new Link { Href = Url.Link(nameof(GetMoto), new { id }), Rel = "self", Method = "GET" });
            resource.AddLink(new Link { Href = Url.Link(nameof(UpdateMoto), new { id }), Rel = "update", Method = "PUT" });
            resource.AddLink(new Link { Href = Url.Link(nameof(DeleteMoto), new { id }), Rel = "delete", Method = "DELETE" });
        }

        [HttpGet(Name = "GetMotos")]
        [SwaggerOperation(Summary = "Lista todas as motos com paginação")]
        public async Task<IActionResult> GetMotos(int page = 1, int pageSize = 10)
        {
            page = page < 1 ? 1 : page;
            pageSize = pageSize < 1 ? 10 : pageSize;

            var totalItems = await _context.Motos.CountAsync();

            var motos = await _context.Motos
                .Include(m => m.Patio)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var data = motos.Select(m =>
            {
                var resource = new MotoResource
                {
                    Id = m.IdMoto,
                    Placa = m.Placa ?? string.Empty,
                    Modelo = m.Modelo ?? string.Empty,
                    Fabricante = m.Fabricante ?? string.Empty,
                    Ano = m.Ano,
                    IdPatio = m.IdPatio
                };
                AddHateoasLinks(resource, m.IdMoto);
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

        [HttpGet("{id}", Name = "GetMoto")]
        [SwaggerOperation(Summary = "Retorna moto por ID")]
        public async Task<IActionResult> GetMoto(int id)
        {
            var m = await _context.Motos.FindAsync(id);
            if (m == null) return NotFound(new { Message = "Moto não encontrada." });

            var resource = new MotoResource
            {
                Id = m.IdMoto,
                Placa = m.Placa ?? string.Empty,
                Modelo = m.Modelo ?? string.Empty,
                Fabricante = m.Fabricante ?? string.Empty,
                Ano = m.Ano,
                IdPatio = m.IdPatio
            };

            AddHateoasLinks(resource, m.IdMoto);

            return Ok(resource);
        }

        [HttpPost(Name = "CreateMoto")]
        [SwaggerOperation(Summary = "Cria uma nova moto")]
        public async Task<IActionResult> CreateMoto([FromBody] MotoInputDTO input)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var moto = new Moto
            {
                Placa = input.Placa ?? string.Empty,
                Modelo = input.Modelo ?? string.Empty,
                Fabricante = input.Fabricante ?? string.Empty,
                Ano = input.Ano,
                IdPatio = input.IdPatio
            };

            _context.Motos.Add(moto);
            await _context.SaveChangesAsync();

            var resource = new MotoResource
            {
                Id = moto.IdMoto,
                Placa = moto.Placa,
                Modelo = moto.Modelo,
                Fabricante = moto.Fabricante,
                Ano = moto.Ano,
                IdPatio = moto.IdPatio
            };

            AddHateoasLinks(resource, moto.IdMoto);

            return CreatedAtAction(nameof(GetMoto), new { id = moto.IdMoto }, resource);
        }

        [HttpPut("{id}", Name = "UpdateMoto")]
        [SwaggerOperation(Summary = "Atualiza uma moto")]
        public async Task<IActionResult> UpdateMoto(int id, [FromBody] MotoInputDTO input)
        {
            var m = await _context.Motos.FindAsync(id);
            if (m == null) return NotFound(new { Message = "Moto não encontrada." });

            m.Placa = input.Placa ?? m.Placa;
            m.Modelo = input.Modelo ?? m.Modelo;
            m.Fabricante = input.Fabricante ?? m.Fabricante;
            m.Ano = input.Ano != 0 ? input.Ano : m.Ano;
            m.IdPatio = input.IdPatio != 0 ? input.IdPatio : m.IdPatio;

            _context.Entry(m).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}", Name = "DeleteMoto")]
        [SwaggerOperation(Summary = "Deleta uma moto")]
        public async Task<IActionResult> DeleteMoto(int id)
        {
            var m = await _context.Motos.FindAsync(id);
            if (m == null) return NotFound(new { Message = "Moto não encontrada." });

            _context.Motos.Remove(m);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
