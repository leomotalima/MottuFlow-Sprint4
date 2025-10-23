using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MottuFlowApi.Data;
using MottuFlowApi.Models;
using MottuFlowApi.DTOs;
using MottuFlowApi.Utils;
using Swashbuckle.AspNetCore.Annotations;

namespace MottuFlowApi.Controllers.V1
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/localidades")]
    [Tags("Localidades")]
    [Produces("application/json")]
    [Consumes("application/json")]
    [Authorize] // 🔒 exige JWT para escrita
    public class LocalidadeController : ControllerBase
    {
        private readonly AppDbContext _context;
        public LocalidadeController(AppDbContext context) => _context = context;

        // 🧩 GET - Lista todas as localidades (público)
        [AllowAnonymous]
        [HttpGet(Name = "GetLocalidades")]
        [SwaggerOperation(
            Summary = "Lista todas as localidades registradas",
            Description = "Retorna todas as localidades cadastradas no sistema, com os respectivos vínculos a motos, pátios e câmeras.")]
        [SwaggerResponse(StatusCodes.Status200OK, "Localidades listadas com sucesso")]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Erro interno no servidor")]
        public async Task<IActionResult> GetLocalidades()
        {
            var localidades = await _context.Localidades
                .Select(l => new LocalidadeOutputDTO
                {
                    IdLocalidade = l.IdLocalidade,
                    DataHora = l.DataHora,
                    PontoReferencia = l.PontoReferencia,
                    IdMoto = l.IdMoto,
                    IdPatio = l.IdPatio,
                    IdCamera = l.IdCamera
                })
                .ToListAsync();

            if (!localidades.Any())
                return Ok(ApiResponse<object>.Ok(new { totalItems = 0, data = new List<LocalidadeOutputDTO>() },
                    "Nenhuma localidade cadastrada."));

            return Ok(ApiResponse<IEnumerable<LocalidadeOutputDTO>>.Ok(localidades, "Localidades listadas com sucesso."));
        }

        // 🧩 GET - Localidade por ID (público)
        [AllowAnonymous]
        [HttpGet("{id}", Name = "GetLocalidade")]
        [SwaggerOperation(
            Summary = "Obtém os dados de uma localidade",
            Description = "Retorna os dados de uma localidade específica pelo seu ID.")]
        [SwaggerResponse(StatusCodes.Status200OK, "Localidade encontrada com sucesso")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Localidade não encontrada")]
        public async Task<IActionResult> GetLocalidade(int id)
        {
            var l = await _context.Localidades.FindAsync(id);
            if (l == null)
                return NotFound(ApiResponse<string>.Fail("Localidade não encontrada."));

            var result = new LocalidadeOutputDTO
            {
                IdLocalidade = l.IdLocalidade,
                DataHora = l.DataHora,
                PontoReferencia = l.PontoReferencia,
                IdMoto = l.IdMoto,
                IdPatio = l.IdPatio,
                IdCamera = l.IdCamera
            };

            return Ok(ApiResponse<LocalidadeOutputDTO>.Ok(result, "Localidade encontrada com sucesso."));
        }

        // 🧩 POST - Cria uma nova localidade
        [HttpPost(Name = "CreateLocalidade")]
        [SwaggerOperation(
            Summary = "Cria uma nova localidade",
            Description = "Registra uma nova localidade no sistema com seus vínculos a moto, pátio e câmera.")]
        [SwaggerResponse(StatusCodes.Status201Created, "Localidade criada com sucesso")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Erro de validação nos dados")]
        public async Task<IActionResult> CreateLocalidade([FromBody] LocalidadeInputDTO input)
        {
            if (!ModelState.IsValid)
                return BadRequest(ApiResponse<string>.Fail("Dados inválidos. Verifique os campos obrigatórios."));

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

            return CreatedAtAction(nameof(GetLocalidade), new { id = localidade.IdLocalidade },
                ApiResponse<LocalidadeOutputDTO>.Ok(result, "Localidade criada com sucesso."));
        }

        // 🧩 PUT - Atualiza uma localidade existente
        [HttpPut("{id}", Name = "UpdateLocalidade")]
        [SwaggerOperation(
            Summary = "Atualiza uma localidade existente",
            Description = "Atualiza os dados de uma localidade registrada no sistema.")]
        [SwaggerResponse(StatusCodes.Status200OK, "Localidade atualizada com sucesso")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Localidade não encontrada")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Erro de validação nos dados")]
        public async Task<IActionResult> UpdateLocalidade(int id, [FromBody] LocalidadeInputDTO input)
        {
            if (!ModelState.IsValid)
                return BadRequest(ApiResponse<string>.Fail("Dados inválidos. Verifique os campos obrigatórios."));

            var localidade = await _context.Localidades.FindAsync(id);
            if (localidade == null)
                return NotFound(ApiResponse<string>.Fail("Localidade não encontrada."));

            localidade.DataHora = input.DataHora;
            localidade.PontoReferencia = input.PontoReferencia;
            localidade.IdMoto = input.IdMoto;
            localidade.IdPatio = input.IdPatio;
            localidade.IdCamera = input.IdCamera;

            _context.Entry(localidade).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            var updated = new LocalidadeOutputDTO
            {
                IdLocalidade = localidade.IdLocalidade,
                DataHora = localidade.DataHora,
                PontoReferencia = localidade.PontoReferencia,
                IdMoto = localidade.IdMoto,
                IdPatio = localidade.IdPatio,
                IdCamera = localidade.IdCamera
            };

            return Ok(ApiResponse<LocalidadeOutputDTO>.Ok(updated, "Localidade atualizada com sucesso."));
        }

        // 🧩 DELETE - Remove uma localidade
        [HttpDelete("{id}", Name = "DeleteLocalidade")]
        [SwaggerOperation(
            Summary = "Remove uma localidade",
            Description = "Exclui uma localidade específica do sistema pelo seu ID.")]
        [SwaggerResponse(StatusCodes.Status204NoContent, "Localidade removida com sucesso")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Localidade não encontrada")]
        public async Task<IActionResult> DeleteLocalidade(int id)
        {
            var l = await _context.Localidades.FindAsync(id);
            if (l == null)
                return NotFound(ApiResponse<string>.Fail("Localidade não encontrada."));

            _context.Localidades.Remove(l);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
