using Microsoft.AspNetCore.Mvc;
using MottuFlowApi.Models;
using MottuFlowApi.Services;
using Swashbuckle.AspNetCore.Annotations;

namespace MottuFlowApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MotoController : ControllerBase
    {
        private readonly IMotoService _motoService;

        public MotoController(IMotoService motoService)
        {
            _motoService = motoService;
        }

        // ----------------------------
        // GET com paginação
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
                        new { rel = "self", href = Url.Action(nameof(GetMotoById), new { id = m.Id }) },
                        new { rel = "update", href = Url.Action(nameof(UpdateMoto), new { id = m.Id }) },
                        new { rel = "delete", href = Url.Action(nameof(DeleteMoto), new { id = m.Id }) }
                    }
                })
            };

            return Ok(response);
        }

        // ----------------------------
        // GET por Id
        // ----------------------------
        [HttpGet("{id}")]
        [SwaggerOperation(Summary = "Busca moto por ID", Description = "Retorna uma moto específica pelo seu ID.")]
        [SwaggerResponse(200, "Moto encontrada", typeof(Moto))]
        [SwaggerResponse(404, "Moto não encontrada")]
        public async Task<ActionResult<Moto>> GetMotoById(int id)
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
                    new { rel = "self", href = Url.Action(nameof(GetMotoById), new { id = moto.Id }) },
                    new { rel = "update", href = Url.Action(nameof(UpdateMoto), new { id = moto.Id }) },
                    new { rel = "delete", href = Url.Action(nameof(DeleteMoto), new { id = moto.Id }) }
                }
            });
        }

        // ----------------------------
        // POST
        // ----------------------------
        [HttpPost]
        [SwaggerOperation(Summary = "Cria uma nova moto", Description = "Adiciona uma nova moto no sistema.")]
        [SwaggerResponse(201, "Moto criada com sucesso", typeof(Moto))]
        [SwaggerResponse(400, "Dados inválidos")]
        public async Task<ActionResult<Moto>> CreateMoto(Moto moto)
        {
            var created = await _motoService.CreateAsync(moto);

            return CreatedAtAction(nameof(GetMotoById), new { id = created.Id }, created);
        }

        // ----------------------------
        // PUT
        // ----------------------------
        [HttpPut("{id}")]
        [SwaggerOperation(Summary = "Atualiza moto", Description = "Atualiza os dados de uma moto existente.")]
        [SwaggerResponse(200, "Moto atualizada com sucesso", typeof(Moto))]
        [SwaggerResponse(404, "Moto não encontrada")]
        public async Task<ActionResult<Moto>> UpdateMoto(int id, Moto moto)
        {
            if (id != moto.Id) return BadRequest("ID da URL não corresponde ao ID da moto.");

            var updated = await _motoService.UpdateAsync(moto);
            if (updated == null) return NotFound();

            return Ok(updated);
        }

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

            return NoContent();
        }
    }
}
