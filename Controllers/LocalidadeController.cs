using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MottuFlowApi.Data;
using MottuFlow.Models;
using MottuFlowApi.DTOs;
using Swashbuckle.AspNetCore.Annotations;

namespace MottuFlowApi.Controllers
{
    [ApiController]
    [Route("api/localidades")]
    public class LocalidadeController : ControllerBase
    {
        private readonly AppDbContext _context;
        public LocalidadeController(AppDbContext context) => _context = context;

        [HttpGet]
        [SwaggerOperation(Summary = "Lista todas as localidades")]
        public async Task<ActionResult<IEnumerable<LocalidadeOutputDTO>>> GetLocalidades()
        {
            var localidades = await _context.Localidades.ToListAsync();

            var result = localidades.Select(l => new LocalidadeOutputDTO
            {
                IdLocalidade = l.IdLocalidade,
                DataHora = l.DataHora,
                PontoReferencia = l.PontoReferencia,
                IdMoto = l.IdMoto,
                IdPatio = l.IdPatio,
                IdCamera = l.IdCamera
            });

            return Ok(result);
        }

        [HttpGet("{id}")]
        [SwaggerOperation(Summary = "Retorna uma localidade pelo ID")]
        public async Task<ActionResult<LocalidadeOutputDTO>> GetLocalidade(int id)
        {
            var l = await _context.Localidades.FindAsync(id);
            if (l == null) return NotFound(new { Message = "Localidade não encontrada." });

            var result = new LocalidadeOutputDTO
            {
                IdLocalidade = l.IdLocalidade,
                DataHora = l.DataHora,
                PontoReferencia = l.PontoReferencia,
                IdMoto = l.IdMoto,
                IdPatio = l.IdPatio,
                IdCamera = l.IdCamera
            };

            return Ok(result);
        }

        [HttpPost]
        [SwaggerOperation(Summary = "Cria uma nova localidade")]
        public async Task<ActionResult<LocalidadeOutputDTO>> CreateLocalidade([FromBody] LocalidadeInputDTO input)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var localidade = new Localidade
            {
                DataHora = input.DataHora,
                PontoReferencia = input.PontoReferencia,
                IdMoto = input.IdMoto,
                IdPatio = input.IdPatio,
                IdCamera = input.IdCamera
            };

            _context.Localidades.Add(localidade);
            await _context.SaveChangesAsync();

            var result = new LocalidadeOutputDTO
            {
                IdLocalidade = localidade.IdLocalidade,
                DataHora = localidade.DataHora,
                PontoReferencia = localidade.PontoReferencia,
                IdMoto = localidade.IdMoto,
                IdPatio = localidade.IdPatio,
                IdCamera = localidade.IdCamera
            };

            return CreatedAtAction(nameof(GetLocalidade), new { id = localidade.IdLocalidade }, result);
        }

        [HttpPut("{id}")]
        [SwaggerOperation(Summary = "Atualiza uma localidade")]
        public async Task<IActionResult> UpdateLocalidade(int id, [FromBody] LocalidadeInputDTO input)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var localidade = await _context.Localidades.FindAsync(id);
            if (localidade == null) return NotFound(new { Message = "Localidade não encontrada." });

            localidade.DataHora = input.DataHora;
            localidade.PontoReferencia = input.PontoReferencia;
            localidade.IdMoto = input.IdMoto;
            localidade.IdPatio = input.IdPatio;
            localidade.IdCamera = input.IdCamera;

            _context.Entry(localidade).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        [SwaggerOperation(Summary = "Deleta uma localidade")]
        public async Task<IActionResult> DeleteLocalidade(int id)
        {
            var l = await _context.Localidades.FindAsync(id);
            if (l == null) return NotFound(new { Message = "Localidade não encontrada." });

            _context.Localidades.Remove(l);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
