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
    [Route("api/v{version:apiVersion}/motos")]
    [Tags("Motos")]
    public class MotoController : ControllerBase
    {
        private readonly AppDbContext _context;
        public MotoController(AppDbContext context) => _context = context;

        // 🔗 Adiciona links HATEOAS
        private void AddHateoasLinks(MotoResource resource, int id)
        {
            resource.AddLink(new Link { Href = Url.Link(nameof(GetMoto), new { id })!, Rel = "self", Method = "GET" });
            resource.AddLink(new Link { Href = Url.Link(nameof(UpdateMoto), new { id })!, Rel = "update", Method = "PUT" });
            resource.AddLink(new Link { Href = Url.Link(nameof(DeleteMoto), new { id })!, Rel = "delete", Method = "DELETE" });
        }

        // 🧩 GET - Listar todas as motos
        [HttpGet(Name = "GetMotos")]
        [SwaggerOperation(Summary = "Lista todas as motos com paginação e HATEOAS")]
        [ProducesResponseType(StatusCodes.Status200OK)]
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

            return Ok(new { success = true, meta, data = motos });
        }

        // 🧩 GET - Buscar moto por ID
        [HttpGet("{id}", Name = "GetMoto")]
        [SwaggerOperation(Summary = "Retorna uma moto por ID")]
        [ProducesResponseType(typeof(MotoResource), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetMoto(int id)
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

            if (moto == null)
                return NotFound(new { success = false, message = "Moto não encontrada." });

            AddHateoasLinks(moto, moto.Id);
            return Ok(new { success = true, data = moto });
        }

        // 🧩 POST - Criar nova moto
        [HttpPost(Name = "CreateMoto")]
        [SwaggerOperation(Summary = "Cria uma nova moto")]
        [ProducesResponseType(typeof(MotoResource), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateMoto([FromBody] MotoInputDTO input)
        {
            if (input == null)
                return BadRequest(new { success = false, message = "Input não pode ser nulo." });

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

            return CreatedAtAction(nameof(GetMoto), new { id = moto.IdMoto }, new { success = true, data = resource });
        }

        // 🧩 PUT - Atualizar moto existente
        [HttpPut("{id}", Name = "UpdateMoto")]
        [SwaggerOperation(Summary = "Atualiza uma moto existente")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateMoto(int id, [FromBody] MotoInputDTO input)
        {
            if (input == null)
                return BadRequest(new { success = false, message = "Input não pode ser nulo." });

            var moto = await _context.Motos.FindAsync(id);
            if (moto == null)
                return NotFound(new { success = false, message = "Moto não encontrada." });

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

        // 🧩 DELETE - Remover moto
        [HttpDelete("{id}", Name = "DeleteMoto")]
        [SwaggerOperation(Summary = "Remove uma moto pelo ID")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteMoto(int id)
        {
            var moto = await _context.Motos.FindAsync(id);
            if (moto == null)
                return NotFound(new { success = false, message = "Moto não encontrada." });

            _context.Motos.Remove(moto);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
