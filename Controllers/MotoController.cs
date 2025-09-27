using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MottuFlowApi.Data;
using MottuFlow.Models;
using MottuFlow.DTOs;
using MottuFlow.Hateoas;
using Swashbuckle.AspNetCore.Annotations;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MottuFlowApi.Controllers
{
    [ApiController]
    [Route("api/motos")]
    [Tags("Moto")]
    public class MotoController : ControllerBase
    {
        private readonly AppDbContext _context;

        public MotoController(AppDbContext context) => _context = context;

        // ===============================
        // GET: api/motos?page=1&pageSize=10
        // ===============================
        [HttpGet(Name = "GetMotos")]
        [SwaggerOperation(Summary = "Lista todas as motos com paginação e HATEOAS")]
        public async Task<IActionResult> GetMotos(int page = 1, int pageSize = 10)
        {
            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 10;

            var totalItems = await _context.Motos.CountAsync();

            var motos = await _context.Motos
                .Include(m => m.RegistrosStatus)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var data = motos.Select(m =>
            {
                var resource = MapMotoToResource(m);
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

        // ===============================
        // GET: api/motos/{id}
        // ===============================
        [HttpGet("{id}", Name = "GetMoto")]
        [SwaggerOperation(Summary = "Retorna moto por ID")]
        public async Task<ActionResult<MotoResource>> GetMoto(int id)
        {
            var m = await _context.Motos
                .Include(m => m.RegistrosStatus)
                .FirstOrDefaultAsync(m => m.IdMoto == id);

            if (m == null) 
                return NotFound(new { Message = "Moto não encontrada." });

            var resource = MapMotoToResource(m);
            AddHateoasLinks(resource, m.IdMoto);

            return Ok(resource);
        }

        // ===============================
        // POST: api/motos
        // ===============================
        [HttpPost(Name = "CreateMoto")]
        [SwaggerOperation(Summary = "Cria uma nova moto")]
        public async Task<ActionResult<MotoResource>> CreateMoto([FromBody] MotoInputDTO input)
        {
            if (!ModelState.IsValid) 
                return BadRequest(ModelState);

            var moto = new Moto
            {
                Placa = input.Placa ?? "Placa padrão",
                Modelo = input.Modelo ?? "Modelo padrão",
                Fabricante = input.Fabricante ?? "Fabricante padrão",
                Ano = input.Ano,
                IdPatio = input.IdPatio,
                LocalizacaoAtual = input.LocalizacaoAtual ?? "Localização padrão"
            };

            _context.Motos.Add(moto);
            await _context.SaveChangesAsync();

            var resource = MapMotoToResource(moto);
            AddHateoasLinks(resource, moto.IdMoto);

            return CreatedAtAction(nameof(GetMoto), new { id = moto.IdMoto }, resource);
        }

        // ===============================
        // PUT: api/motos/{id}
        // ===============================
        [HttpPut("{id}", Name = "UpdateMoto")]
        [SwaggerOperation(Summary = "Atualiza uma moto")]
        public async Task<IActionResult> UpdateMoto(int id, [FromBody] MotoInputDTO input)
        {
<<<<<<< HEAD
            if (!ModelState.IsValid) 
                return BadRequest(ModelState);

=======
>>>>>>> 5974efe (Atualiza controllers e DTOs: Patio e ArucoTag, ajusta HATEOAS e Swagger)
            var moto = await _context.Motos.FindAsync(id);
            if (moto == null) 
                return NotFound(new { Message = "Moto não encontrada." });

            moto.Placa = input.Placa ?? moto.Placa;
            moto.Modelo = input.Modelo ?? moto.Modelo;
            moto.Fabricante = input.Fabricante ?? moto.Fabricante;
            moto.Ano = input.Ano;
            moto.IdPatio = input.IdPatio;
            moto.LocalizacaoAtual = input.LocalizacaoAtual ?? moto.LocalizacaoAtual;

            _context.Entry(moto).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // ===============================
        // DELETE: api/motos/{id}
        // ===============================
        [HttpDelete("{id}", Name = "DeleteMoto")]
        [SwaggerOperation(Summary = "Deleta uma moto")]
        public async Task<IActionResult> DeleteMoto(int id)
        {
            var moto = await _context.Motos.FindAsync(id);
            if (moto == null) 
                return NotFound(new { Message = "Moto não encontrada." });

            _context.Motos.Remove(moto);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // ===============================
        // Métodos auxiliares
        // ===============================
        private void AddHateoasLinks(MotoResource motoResource, int id)
        {
            motoResource.AddLink(new Link { Href = Url.Link("GetMoto", new { id })!, Rel = "self", Method = "GET" });
            motoResource.AddLink(new Link { Href = Url.Link("UpdateMoto", new { id })!, Rel = "update", Method = "PUT" });
            motoResource.AddLink(new Link { Href = Url.Link("DeleteMoto", new { id })!, Rel = "delete", Method = "DELETE" });
        }

        private MotoResource MapMotoToResource(Moto m)
        {
            return new MotoResource
            {
<<<<<<< HEAD
                IdMoto = m.IdMoto,
                Placa = m.Placa ?? "Placa padrão",
                Modelo = m.Modelo ?? "Modelo padrão",
                Fabricante = m.Fabricante ?? "Fabricante padrão",
=======
                Id = m.IdMoto,
                Placa = m.Placa,
                Modelo = m.Modelo,
                Fabricante = m.Fabricante,
>>>>>>> 5974efe (Atualiza controllers e DTOs: Patio e ArucoTag, ajusta HATEOAS e Swagger)
                Ano = m.Ano,
                IdPatio = m.IdPatio,
                LocalizacaoAtual = m.LocalizacaoAtual ?? "Localização padrão",
                Statuses = (m.RegistrosStatus?.Select(rs => new StatusDTO
                {
                    IdStatus = rs.IdStatus,
                    TipoStatus = rs.TipoStatus ?? "Tipo padrão",
                    Descricao = rs.Descricao ?? "Descrição padrão",
                    DataStatus = rs.DataStatus,
                    IdFuncionario = rs.IdFuncionario
                }).ToList()) ?? new List<StatusDTO>()
            };
        }
    }
}
