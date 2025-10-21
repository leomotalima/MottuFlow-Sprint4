using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MottuFlowApi.Data;
using MottuFlowApi.Models;
using MottuFlowApi.DTOs;
using MottuFlow.Hateoas;
using Swashbuckle.AspNetCore.Annotations;
using System.Security.Cryptography;
using System.Text;

namespace MottuFlowApi.Controllers.V2
{
    [ApiController]
    [ApiVersion("2.0")]
    [ApiExplorerSettings(GroupName = "v2")]
    [Route("api/v{version:apiVersion}/funcionarios")]
    [Tags("Funcionários - v2")]
    [Produces("application/json")]
    [Consumes("application/json")]
    public class FuncionarioControllerV2 : ControllerBase
    {
        private readonly AppDbContext _context;
        public FuncionarioControllerV2(AppDbContext context) => _context = context;

        // 🔒 Hash de senha (mantém a mesma lógica da v1)
        private string HashSenha(string senha)
        {
            using var sha256 = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(senha ?? string.Empty);
            var hash = sha256.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }

        // 🔗 HATEOAS Links
        private void AddHateoasLinks(FuncionarioOutputDTO resource, int id)
        {
            resource.AddLink(new Link { Href = Url.Link(nameof(GetFuncionarioV2), new { id })!, Rel = "self", Method = "GET" });
            resource.AddLink(new Link { Href = Url.Link(nameof(UpdateFuncionarioV2), new { id })!, Rel = "update", Method = "PUT" });
            resource.AddLink(new Link { Href = Url.Link(nameof(DeleteFuncionarioV2), new { id })!, Rel = "delete", Method = "DELETE" });
        }

        // 🧩 GET (com filtros e ordenação)
        [HttpGet(Name = "GetFuncionariosV2")]
        [MapToApiVersion("2.0")]
        [SwaggerOperation(Summary = "Lista funcionários com filtro opcional por nome/cargo e ordenação (v2)")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetFuncionariosV2(string? nome = null, string? cargo = null, string? orderBy = "nome", int page = 1, int pageSize = 10)
        {
            var query = _context.Funcionarios.AsQueryable();

            if (!string.IsNullOrEmpty(nome))
                query = query.Where(f => EF.Functions.Like(f.Nome.ToLower(), $"%{nome.ToLower()}%"));

            if (!string.IsNullOrEmpty(cargo))
                query = query.Where(f => EF.Functions.Like(f.Cargo.ToLower(), $"%{cargo.ToLower()}%"));

            query = orderBy?.ToLower() switch
            {
                "cargo" => query.OrderBy(f => f.Cargo),
                "email" => query.OrderBy(f => f.Email),
                _ => query.OrderBy(f => f.Nome)
            };

            var totalItems = await query.CountAsync();

            var funcionarios = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(f => new FuncionarioOutputDTO
                {
                    Id = f.IdFuncionario,
                    Nome = f.Nome!,
                    Cpf = f.CPF!,
                    Cargo = f.Cargo!,
                    Telefone = f.Telefone!,
                    Email = f.Email!,
                    DataCadastro = DateTime.Now.AddDays(-f.IdFuncionario) // novo campo simulado
                })
                .ToListAsync();

            if (!funcionarios.Any())
                return Ok(new { success = true, message = "Nenhum funcionário encontrado.", data = new List<FuncionarioOutputDTO>() });

            funcionarios.ForEach(f => AddHateoasLinks(f, f.Id));

            var meta = new
            {
                totalItems,
                page,
                pageSize,
                totalPages = Math.Ceiling((double)totalItems / pageSize)
            };

            return Ok(new { success = true, version = "2.0", meta, data = funcionarios });
        }

        // 🧩 GET (por ID)
        [HttpGet("{id}", Name = "GetFuncionarioV2")]
        [MapToApiVersion("2.0")]
        [SwaggerOperation(Summary = "Retorna um funcionário específico (v2)")]
        [ProducesResponseType(typeof(FuncionarioOutputDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetFuncionarioV2(int id)
        {
            var funcionario = await _context.Funcionarios
                .Where(f => f.IdFuncionario == id)
                .Select(f => new FuncionarioOutputDTO
                {
                    Id = f.IdFuncionario,
                    Nome = f.Nome!,
                    Cpf = f.CPF!,
                    Cargo = f.Cargo!,
                    Telefone = f.Telefone!,
                    Email = f.Email!,
                    DataCadastro = DateTime.Now.AddDays(-f.IdFuncionario)
                })
                .FirstOrDefaultAsync();

            if (funcionario == null)
                return NotFound(new { success = false, message = "Funcionário não encontrado." });

            AddHateoasLinks(funcionario, funcionario.Id);
            return Ok(new { success = true, version = "2.0", data = funcionario });
        }

        // 🧩 PUT (Atualiza funcionário)
        [HttpPut("{id}", Name = "UpdateFuncionarioV2")]
        [MapToApiVersion("2.0")]
        [SwaggerOperation(Summary = "Atualiza dados de um funcionário (v2)")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateFuncionarioV2(int id, [FromBody] FuncionarioInputDTO input)
        {
            var funcionario = await _context.Funcionarios.FindAsync(id);
            if (funcionario == null)
                return NotFound(new { success = false, message = "Funcionário não encontrado." });

            funcionario.Nome = input.Nome;
            funcionario.Cargo = input.Cargo;
            funcionario.Telefone = input.Telefone;
            funcionario.Email = input.Email;

            if (!string.IsNullOrEmpty(input.Senha))
                funcionario.Senha = HashSenha(input.Senha);

            _context.Entry(funcionario).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            var updated = new FuncionarioOutputDTO
            {
                Id = funcionario.IdFuncionario,
                Nome = funcionario.Nome,
                Cpf = funcionario.CPF,
                Cargo = funcionario.Cargo,
                Telefone = funcionario.Telefone,
                Email = funcionario.Email,
                DataCadastro = DateTime.Now.AddDays(-funcionario.IdFuncionario)
            };

            AddHateoasLinks(updated, funcionario.IdFuncionario);
            return Ok(new { success = true, message = "Funcionário atualizado com sucesso (v2).", data = updated });
        }

        // 🧩 DELETE
        [HttpDelete("{id}", Name = "DeleteFuncionarioV2")]
        [MapToApiVersion("2.0")]
        [SwaggerOperation(Summary = "Remove um funcionário (v2)")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteFuncionarioV2(int id)
        {
            var funcionario = await _context.Funcionarios.FindAsync(id);
            if (funcionario == null)
                return NotFound(new { success = false, message = "Funcionário não encontrado." });

            _context.Funcionarios.Remove(funcionario);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
