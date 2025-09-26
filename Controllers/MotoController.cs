using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MottuFlowApi.Data;
using MottuFlow.Models;
using MottuFlow.DTOs;
using Swashbuckle.AspNetCore.Annotations;

namespace MottuFlowApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MotoController : ControllerBase
    {
        private readonly AppDbContext _context;

        public MotoController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/motos?page=1&pageSize=10
        [HttpGet]
        [SwaggerOperation(Summary = "Lista todas as motos com paginação")]
        public async Task<ActionResult<IEnumerable<MotoOutputDTO>>> GetMotos(int page = 1, int pageSize = 10)
        {
            var motos = await _context.Motos
                .Include(m => m.RegistrosStatus)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var result = motos.Select(m => MapToOutputDTO(m));
            return Ok(result);
        }

        // GET: api/motos/5
        [HttpGet("{id}")]
        [SwaggerOperation(Summary = "Retorna moto por ID")]
        public async Task<ActionResult<MotoOutputDTO>> GetMoto(int id)
        {
            var m = await _context.Motos
                .Include(m => m.RegistrosStatus)
                .FirstOrDefaultAsync(m => m.IdMoto == id);

            if (m == null) 
                return NotFound(new { Message = "Moto não encontrada." });

            return Ok(MapToOutputDTO(m));
        }

        // POST: api/motos
        [HttpPost]
        [SwaggerOperation(Summary = "Cria uma nova moto")]
        public async Task<ActionResult<MotoOutputDTO>> CreateMoto([FromBody] MotoInputDTO input)
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

            return CreatedAtAction(nameof(GetMoto), new { id = moto.IdMoto }, MapToOutputDTO(moto));
        }

        // PUT: api/motos/5
        [HttpPut("{id}")]
        [SwaggerOperation(Summary = "Atualiza uma moto")]
        public async Task<IActionResult> UpdateMoto(int id, [FromBody] MotoInputDTO input)
        {
            if (!ModelState.IsValid) 
                return BadRequest(ModelState);

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

            return Ok(MapToOutputDTO(moto));
        }

        // DELETE: api/motos/5
        [HttpDelete("{id}")]
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

        // === Helpers ===
        private MotoOutputDTO MapToOutputDTO(Moto m)
        {
            return new MotoOutputDTO
            {
                IdMoto = m.IdMoto,
                Placa = m.Placa ?? "Placa padrão",
                Modelo = m.Modelo ?? "Modelo padrão",
                Fabricante = m.Fabricante ?? "Fabricante padrão",
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
