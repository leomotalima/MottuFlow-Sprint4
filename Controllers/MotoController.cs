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
    [Route("api/motos")]
    [Tags("Moto")]
    public class MotoController : ControllerBase
    {
        private readonly AppDbContext _context;
        public MotoController(AppDbContext context) => _context = context;

        private void AddHateoasLinks(MotoResource resource, int id)
        {
            resource.AddLink(new Link { Href = Url.Link(nameof(GetMoto), new { id })!, Rel = "self", Method = "GET" });
            resource.AddLink(new Link { Href = Url.Link(nameof(UpdateMoto), new { id })!, Rel = "update", Method = "PUT" });
            resource.AddLink(new Link { Href = Url.Link(nameof(DeleteMoto), new { id })!, Rel = "delete", Method = "DELETE" });
        }

        [HttpGet(Name = "GetMotos")]
        [SwaggerOperation(Summary = "Lista todas as motos com paginação e HATEOAS")]
        public async Task<IActionResult> GetMotos(int page = 1, int pageSize = 10)
        {
            page = Math.Max(page, 1);
            pageSize = Math.Max(pageSize, 1);

            var totalItems = await _context.Motos.CountAsync();

            var motos = await _context.Motos
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(m => new MotoResource
                {
                    Id = m.IdMoto,
                    Placa = m.Placa!,
                    Modelo = m.Modelo!,
                    Fabricante = m.Fabricante!,
                    Ano = m.Ano,
                    IdPatio = m.IdPatio,
                    LocalizacaoAtual = m.LocalizacaoAtual!
                })
                .ToListAsync();

            motos.ForEach(m => AddHateoasLinks(m, m.Id));

            var meta = new
            {
                totalItems,
                page,
                pageSize,
                totalPages = Math.Ceiling((double)totalItems / pageSize)
            };

            return Ok(new { meta, data = motos });
        }

        [HttpGet("{id}", Name = "GetMoto")]
        [SwaggerOperation(Summary = "Retorna uma moto por ID")]
        public async Task<ActionResult<MotoResource>> GetMoto(int id)
        {
            var moto = await _context.Motos
                .Where(m => m.IdMoto == id)
                .Select(m => new MotoResource
                {
                    Id = m.IdMoto,
                    Placa = m.Placa!,
                    Modelo = m.Modelo!,
                    Fabricante = m.Fabricante!,
                    Ano = m.Ano,
                    IdPatio = m.IdPatio,
                    LocalizacaoAtual = m.LocalizacaoAtual!
                })
                .FirstOrDefaultAsync();

            if (moto == null) return NotFound(new { Message = "Moto não encontrada." });

            AddHateoasLinks(moto, moto.Id);
            return Ok(moto);
        }

        [HttpPost(Name = "CreateMoto")]
        [SwaggerOperation(Summary = "Cria uma nova moto")]
        public async Task<ActionResult<MotoResource>> CreateMoto([FromBody] MotoInputDTO input)
        {
            if (input == null) return BadRequest("Input não pode ser nulo.");

            var moto = new Moto
            {
                Placa = input.Placa,
                Modelo = input.Modelo,
                Fabricante = input.Fabricante,
                Ano = input.Ano,
                IdPatio = input.IdPatio,
                LocalizacaoAtual = input.LocalizacaoAtual
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
                IdPatio = moto.IdPatio,
                LocalizacaoAtual = moto.LocalizacaoAtual
            };
            AddHateoasLinks(resource, moto.IdMoto);

            return CreatedAtAction(nameof(GetMoto), new { id = moto.IdMoto }, resource);
        }

        [HttpPut("{id}", Name = "UpdateMoto")]
        [SwaggerOperation(Summary = "Atualiza uma moto")]
        public async Task<IActionResult> UpdateMoto(int id, [FromBody] MotoInputDTO input)
        {
            if (input == null) return BadRequest("Input não pode ser nulo.");

            var moto = await _context.Motos.FindAsync(id);
            if (moto == null) return NotFound(new { Message = "Moto não encontrada." });

            moto.Placa = input.Placa;
            moto.Modelo = input.Modelo;
            moto.Fabricante = input.Fabricante;
            moto.Ano = input.Ano;
            moto.IdPatio = input.IdPatio;
            moto.LocalizacaoAtual = input.LocalizacaoAtual;

            _context.Entry(moto).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}", Name = "DeleteMoto")]
        [SwaggerOperation(Summary = "Deleta uma moto")]
        public async Task<IActionResult> DeleteMoto(int id)
        {
            var moto = await _context.Motos.FindAsync(id);
            if (moto == null) return NotFound(new { Message = "Moto não encontrada." });

            _context.Motos.Remove(moto);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
