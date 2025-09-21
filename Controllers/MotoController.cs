using Microsoft.AspNetCore.Mvc;
<<<<<<< HEAD
using MottuFlowApi.Models;
using MottuFlowApi.Services;
using Swashbuckle.AspNetCore.Annotations;
using Swashbuckle.AspNetCore.Filters;
=======
using Microsoft.EntityFrameworkCore;
using MottuFlowApi.Data;
using MottuFlow.Models;
using MottuFlowApi.DTOs;
using Swashbuckle.AspNetCore.Annotations;
>>>>>>> bb95fe1 (Adiciona model, DTO e controller de RegistroStatus)

namespace MottuFlowApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MotoController : ControllerBase
    {
        private readonly IMotoService _motoService;

<<<<<<< HEAD
        public MotoController(IMotoService motoService)
        {
            _motoService = motoService;
        }

        // ----------------------------
        // GET com paginação + HATEOAS
        // ----------------------------
        /// <summary>
        /// Lista todas as motos com suporte a paginação.
        /// </summary>
        /// <param name="page">Número da página (mínimo = 1)</param>
        /// <param name="size">Quantidade de registros por página</param>
        [HttpGet]
        [SwaggerOperation(Summary = "Lista motos", Description = "Retorna as motos com suporte a paginação e links HATEOAS.")]
        [SwaggerResponse(200, "Lista de motos retornada com sucesso")]
        public async Task<ActionResult> GetMotos([FromQuery] int page = 1, [FromQuery] int size = 10)
        {
            if (page <= 0 || size <= 0)
                return BadRequest("Parâmetros de paginação inválidos.");

            var result = await _motoService.GetPagedAsync(page, size);

            var response = new
            {
                result.Page,
                result.Size,
                result.TotalCount,
                Data = result.Items.Select(m => new
                {
                    m.Id,
                    m.Placa,
                    m.Marca,
                    m.Modelo,
                    Links = new[]
                    {
                        new LinkDto("self", Url.Action(nameof(GetMotoById), new { id = m.Id }), "GET"),
                        new LinkDto("update", Url.Action(nameof(UpdateMoto), new { id = m.Id }), "PUT"),
                        new LinkDto("delete", Url.Action(nameof(DeleteMoto), new { id = m.Id }), "DELETE")
                    }
                })
            };

            return Ok(response);
        }

        // ----------------------------
        // GET por ID
        // ----------------------------
        [HttpGet("{id}")]
        [SwaggerOperation(Summary = "Busca moto por ID", Description = "Retorna uma moto específica pelo seu ID.")]
        [SwaggerResponse(200, "Moto encontrada", typeof(Moto))]
        [SwaggerResponse(404, "Moto não encontrada")]
        public async Task<ActionResult> GetMotoById(int id)
        {
            var moto = await _motoService.GetByIdAsync(id);
            if (moto == null) return NotFound();

            return Ok(new
            {
                moto.Id,
                moto.Placa,
                moto.Marca,
                moto.Modelo,
                Links = new[]
                {
                    new LinkDto("self", Url.Action(nameof(GetMotoById), new { id = moto.Id }), "GET"),
                    new LinkDto("update", Url.Action(nameof(UpdateMoto), new { id = moto.Id }), "PUT"),
                    new LinkDto("delete", Url.Action(nameof(DeleteMoto), new { id = moto.Id }), "DELETE")
                }
            });
        }

        // ----------------------------
        // POST com exemplo Swagger
        // ----------------------------
        [HttpPost]
        [SwaggerOperation(Summary = "Cria uma nova moto", Description = "Adiciona uma nova moto no sistema.")]
        [SwaggerResponse(201, "Moto criada com sucesso", typeof(Moto))]
        [SwaggerResponse(400, "Dados inválidos")]
        [SwaggerRequestExample(typeof(Moto), typeof(MotoRequestExample))]
        public async Task<ActionResult<Moto>> CreateMoto([FromBody] Moto moto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var created = await _motoService.CreateAsync(moto);
            return CreatedAtAction(nameof(GetMotoById), new { id = created.Id }, created);
        }

        // ----------------------------
        // PUT
        // ----------------------------
        [HttpPut("{id}")]
        [SwaggerOperation(Summary = "Atualiza moto", Description = "Atualiza os dados de uma moto existente.")]
        [SwaggerResponse(200, "Moto atualizada com sucesso", typeof(Moto))]
        [SwaggerResponse(400, "ID inválido ou dados inválidos")]
        [SwaggerResponse(404, "Moto não encontrada")]
        [SwaggerRequestExample(typeof(Moto), typeof(MotoRequestExample))]
        public async Task<ActionResult<Moto>> UpdateMoto(int id, [FromBody] Moto moto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            if (id != moto.Id) return BadRequest("ID da URL não corresponde ao ID da moto.");

            var updated = await _motoService.UpdateAsync(moto);
            if (updated == null) return NotFound();
=======
        // GET: api/motos?page=1&pageSize=10
        [HttpGet]
        [SwaggerOperation(Summary = "Lista todas as motos com paginação")]
        public async Task<ActionResult<IEnumerable<MotoOutputDTO>>> GetMotos(int page = 1, int pageSize = 10)
        {
            var motos = await _context.Motos
                .Include(m => m.RegistrosStatus) // Inclui registros de status
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

            if (m == null) return NotFound(new { Message = "Moto não encontrada." });

            return Ok(MapToOutputDTO(m));
        }

        // POST: api/motos
        [HttpPost]
        [SwaggerOperation(Summary = "Cria uma nova moto")]
        public async Task<ActionResult<MotoOutputDTO>> CreateMoto([FromBody] MotoInputDTO input)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

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

            return CreatedAtAction(nameof(GetMoto), new { id = moto.IdMoto }, MapToOutputDTO(moto));
        }

        // PUT: api/motos/5
        [HttpPut("{id}")]
        [SwaggerOperation(Summary = "Atualiza uma moto")]
        public async Task<IActionResult> UpdateMoto(int id, [FromBody] MotoInputDTO input)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

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
>>>>>>> bb95fe1 (Adiciona model, DTO e controller de RegistroStatus)

            return Ok(updated);
        }

<<<<<<< HEAD
        // ----------------------------
        // DELETE
        // ----------------------------
        [HttpDelete("{id}")]
        [SwaggerOperation(Summary = "Exclui moto", Description = "Remove uma moto do sistema.")]
        [SwaggerResponse(204, "Moto excluída com sucesso")]
        [SwaggerResponse(404, "Moto não encontrada")]
        public async Task<IActionResult> DeleteMoto(int id)
        {
            var success = await _motoService.DeleteAsync(id);
            if (!success) return NotFound();

=======
        // DELETE: api/motos/5
        [HttpDelete("{id}")]
        [SwaggerOperation(Summary = "Deleta uma moto")]
        public async Task<IActionResult> DeleteMoto(int id)
        {
            var moto = await _context.Motos.FindAsync(id);
            if (moto == null) return NotFound(new { Message = "Moto não encontrada." });

            _context.Motos.Remove(moto);
            await _context.SaveChangesAsync();
>>>>>>> bb95fe1 (Adiciona model, DTO e controller de RegistroStatus)
            return NoContent();
        }

        // === Helpers ===
        private MotoOutputDTO MapToOutputDTO(Moto m)
        {
            return new MotoOutputDTO
            {
                IdMoto = m.IdMoto,
                Placa = m.Placa,
                Modelo = m.Modelo,
                Fabricante = m.Fabricante,
                Ano = m.Ano,
                IdPatio = m.IdPatio,
                LocalizacaoAtual = m.LocalizacaoAtual,
                Statuses = (m.RegistrosStatus?.Select(rs => new StatusDTO
                {
                    IdStatus = rs.IdStatus,
                    TipoStatus = rs.TipoStatus,
                    Descricao = rs.Descricao,
                    DataStatus = rs.DataStatus,
                    IdFuncionario = rs.IdFuncionario
                }).ToList()) ?? new List<StatusDTO>()
            };
        }
    }

    // ----------------------------
    // DTO para HATEOAS
    // ----------------------------
    public class LinkDto
    {
        public string Rel { get; set; }
        public string Href { get; set; }
        public string Method { get; set; }

        public LinkDto(string rel, string href, string method)
        {
            Rel = rel;
            Href = href;
            Method = method;
        }
    }

    // ----------------------------
    // Exemplo de request para Swagger
    // ----------------------------
    public class MotoRequestExample : IExamplesProvider<Moto>
    {
        public Moto GetExamples()
        {
            return new Moto
            {
                Placa = "ABC1234",
                Marca = "Honda",
                Modelo = "CG 160"
            };
        }
    }
}
