using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using MottuFlowApi.Data;
using MottuFlowApi.Models;
using MottuFlowApi.DTOs;
using MottuFlow.Hateoas;
using Swashbuckle.AspNetCore.Annotations;

namespace MottuFlowApi.Controllers.V1
{
    [ApiController]
    [ApiVersion("1.0")]
    [ApiExplorerSettings(GroupName = "v1")]
    [Route("api/v{version:apiVersion}/funcionarios")]
    [Tags("Funcionários")]
    [Produces("application/json")]
    [Consumes("application/json")]
    [Authorize] // 🔒 Protege toda a controller com JWT
    public class FuncionarioController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _config;

        public FuncionarioController(AppDbContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        // 🔐 LOGIN - Gera token JWT
        [AllowAnonymous]
        [HttpPost("login")]
        [SwaggerOperation(Summary = "Realiza login e gera o token JWT (v1)")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Login([FromBody] LoginDTO login)
        {
            var funcionario = await _context.Funcionarios
                .FirstOrDefaultAsync(f => f.Email == login.Email);

            if (funcionario == null)
                return Unauthorized(new { success = false, message = "Usuário não encontrado." });

            var senhaHash = HashSenha(login.Senha);
            if (funcionario.Senha != senhaHash)
                return Unauthorized(new { success = false, message = "Senha incorreta." });

            var claims = new[]
            {
                new Claim(ClaimTypes.Name, funcionario.Nome),
                new Claim(ClaimTypes.Email, funcionario.Email),
                new Claim(ClaimTypes.Role, funcionario.Cargo)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expiracao = DateTime.UtcNow.AddHours(2);

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: expiracao,
                signingCredentials: creds
            );

            return Ok(new
            {
                success = true,
                message = "Login realizado com sucesso!",
                token = new JwtSecurityTokenHandler().WriteToken(token),
                expiracao
            });
        }

        // 🔒 Hash de senha
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
            resource.AddLink(new Link { Href = Url.Link(nameof(GetFuncionario), new { id })!, Rel = "self", Method = "GET" });
            resource.AddLink(new Link { Href = Url.Link(nameof(UpdateFuncionario), new { id })!, Rel = "update", Method = "PUT" });
            resource.AddLink(new Link { Href = Url.Link(nameof(DeleteFuncionario), new { id })!, Rel = "delete", Method = "DELETE" });
        }

        // 🧩 GET (com filtros e ordenação)
        [HttpGet(Name = "GetFuncionarios")]
        [MapToApiVersion("1.0")]
        [SwaggerOperation(Summary = "Lista funcionários com filtro opcional por nome/cargo e ordenação (v1)")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetFuncionarios(string? nome = null, string? cargo = null, string? orderBy = "nome", int page = 1, int pageSize = 10)
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
                    DataCadastro = DateTime.Now.AddDays(-f.IdFuncionario)
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

            return Ok(new { success = true, version = "1.0", meta, data = funcionarios });
        }

        // 🧩 GET (por ID)
        [HttpGet("{id}", Name = "GetFuncionario")]
        [MapToApiVersion("1.0")]
        [SwaggerOperation(Summary = "Retorna um funcionário específico (v1)")]
        [ProducesResponseType(typeof(FuncionarioOutputDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetFuncionario(int id)
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
            return Ok(new { success = true, version = "1.0", data = funcionario });
        }

        // 🧩 PUT (Atualiza funcionário)
        [HttpPut("{id}", Name = "UpdateFuncionario")]
        [MapToApiVersion("1.0")]
        [SwaggerOperation(Summary = "Atualiza dados de um funcionário (v1)")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateFuncionario(int id, [FromBody] FuncionarioInputDTO input)
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
            return Ok(new { success = true, message = "Funcionário atualizado com sucesso.", data = updated });
        }

        // 🧩 DELETE
        [HttpDelete("{id}", Name = "DeleteFuncionario")]
        [MapToApiVersion("1.0")]
        [SwaggerOperation(Summary = "Remove um funcionário (v1)")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteFuncionario(int id)
        {
            var funcionario = await _context.Funcionarios.FindAsync(id);
            if (funcionario == null)
                return NotFound(new { success = false, message = "Funcionário não encontrado." });

            _context.Funcionarios.Remove(funcionario);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }

    // DTO auxiliar para login
    public class LoginDTO
    {
        public string Email { get; set; } = string.Empty;
        public string Senha { get; set; } = string.Empty;
    }
}
