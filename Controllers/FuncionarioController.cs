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
using MottuFlowApi.Utils;
using Swashbuckle.AspNetCore.Annotations;

namespace MottuFlowApi.Controllers.V1
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/funcionarios")]
    [Tags("Funcionários")]
    [Produces("application/json")]
    [Consumes("application/json")]
    [Authorize]
    public class FuncionarioController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _config;

        public FuncionarioController(AppDbContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        // ✅ LOGIN (gera token JWT)
        [AllowAnonymous]
        [HttpPost("login")]
        [SwaggerOperation(Summary = "Realiza login e gera token JWT", Description = "Autentica o funcionário e retorna um token JWT válido por 2h.")]
        [SwaggerResponse(StatusCodes.Status200OK, "Login realizado com sucesso.")]
        [SwaggerResponse(StatusCodes.Status401Unauthorized, "Credenciais inválidas.")]
        public async Task<IActionResult> Login([FromBody] LoginDTO login)
        {
            var funcionario = await _context.Funcionarios.FirstOrDefaultAsync(f => f.Email == login.Email);
            if (funcionario == null)
                return Unauthorized(ApiResponse<string>.Fail("Usuário não encontrado."));

            var senhaHash = HashSenha(login.Senha);
            if (funcionario.Senha != senhaHash)
                return Unauthorized(ApiResponse<string>.Fail("Senha incorreta."));

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

            var tokenGerado = new
            {
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                ExpiraEm = expiracao
            };

            return Ok(ApiResponse<object>.Ok(tokenGerado, "Login realizado com sucesso!"));
        }

        // 🔒 Hash seguro
        private string HashSenha(string senha)
        {
            using var sha256 = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(senha ?? string.Empty);
            var hash = sha256.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }

        // 🧩 GET - Lista funcionários
        [HttpGet]
        [SwaggerOperation(Summary = "Lista todos os funcionários", Description = "Retorna uma lista paginada de funcionários com filtros opcionais.")]
        [SwaggerResponse(StatusCodes.Status200OK, "Funcionários listados com sucesso.")]
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

            var meta = new
            {
                totalItems,
                page,
                pageSize,
                totalPages = Math.Ceiling((double)totalItems / pageSize)
            };

            return Ok(ApiResponse<object>.Ok(new { meta, funcionarios }, "Funcionários listados com sucesso."));
        }

        // 🧩 GET - Por ID
        [HttpGet("{id}")]
        [SwaggerOperation(Summary = "Obtém um funcionário específico", Description = "Retorna os dados detalhados de um funcionário.")]
        [SwaggerResponse(StatusCodes.Status200OK, "Funcionário encontrado.")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Funcionário não encontrado.")]
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
                return NotFound(ApiResponse<string>.Fail("Funcionário não encontrado."));

            return Ok(ApiResponse<FuncionarioOutputDTO>.Ok(funcionario, "Funcionário encontrado com sucesso."));
        }

        // 🧩 PUT - Atualiza
        [HttpPut("{id}")]
        [SwaggerOperation(Summary = "Atualiza um funcionário existente", Description = "Permite atualizar dados de um funcionário pelo ID.")]
        [SwaggerResponse(StatusCodes.Status200OK, "Funcionário atualizado com sucesso.")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Funcionário não encontrado.")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Erro na requisição.")]
        public async Task<IActionResult> UpdateFuncionario(int id, [FromBody] FuncionarioInputDTO input)
        {
            if (input == null)
                return BadRequest(ApiResponse<string>.Fail("Dados inválidos."));

            var funcionario = await _context.Funcionarios.FindAsync(id);
            if (funcionario == null)
                return NotFound(ApiResponse<string>.Fail("Funcionário não encontrado."));

            funcionario.Nome = input.Nome;
            funcionario.Cargo = input.Cargo;
            funcionario.Telefone = input.Telefone;
            funcionario.Email = input.Email;

            if (!string.IsNullOrEmpty(input.Senha))
                funcionario.Senha = HashSenha(input.Senha);

            _context.Entry(funcionario).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return Ok(ApiResponse<string>.Ok("Funcionário atualizado com sucesso."));
        }

        // 🧩 DELETE - Remove
        [HttpDelete("{id}")]
        [SwaggerOperation(Summary = "Remove um funcionário", Description = "Exclui o funcionário do sistema.")]
        [SwaggerResponse(StatusCodes.Status204NoContent, "Funcionário removido com sucesso.")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Funcionário não encontrado.")]
        public async Task<IActionResult> DeleteFuncionario(int id)
        {
            var funcionario = await _context.Funcionarios.FindAsync(id);
            if (funcionario == null)
                return NotFound(ApiResponse<string>.Fail("Funcionário não encontrado."));

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
