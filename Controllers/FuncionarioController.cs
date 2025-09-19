using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MottuFlowApi.Data;
using MottuFlowApi.Models;
using MottuFlowApi.DTOs;  // <-- DTOs
using Swashbuckle.AspNetCore.Annotations;
using System.Security.Cryptography;
using System.Text;

namespace MottuFlowApi.Controllers
{
    [ApiController]
    [Route("api/funcionarios")]
    public class FuncionarioController : ControllerBase
    {
        private readonly AppDbContext _context;
        public FuncionarioController(AppDbContext context) => _context = context;

        // GET: api/funcionarios?page=1&pageSize=10
        [HttpGet]
        [SwaggerOperation(Summary = "Lista todos os funcionários com paginação")]
        public async Task<ActionResult<IEnumerable<FuncionarioOutputDTO>>> GetFuncionarios(int page = 1, int pageSize = 10)
        {
            var funcionarios = await _context.Funcionarios
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var result = funcionarios.Select(f => MapToOutputDTO(f));
            return Ok(result);
        }

        // GET: api/funcionarios/5
        [HttpGet("{id}")]
        [SwaggerOperation(Summary = "Retorna funcionário por ID")]
        public async Task<ActionResult<FuncionarioOutputDTO>> GetFuncionario(int id)
        {
            var f = await _context.Funcionarios.FindAsync(id);
            if (f == null) return NotFound(new { Message = "Funcionário não encontrado." });

            return Ok(MapToOutputDTO(f));
        }

        // POST: api/funcionarios
        [HttpPost]
        [SwaggerOperation(Summary = "Cria um novo funcionário")]
        public async Task<ActionResult<FuncionarioOutputDTO>> CreateFuncionario([FromBody] FuncionarioInputDTO input)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var funcionario = new Funcionario
            {
                Nome = input.Nome,
                Cpf = input.Cpf,
                Cargo = input.Cargo,
                Telefone = input.Telefone,
                Email = input.Email,
                Senha = HashSenha(input.Senha) // <-- senha com hash
            };

            _context.Funcionarios.Add(funcionario);
            await _context.SaveChangesAsync();

            var output = MapToOutputDTO(funcionario);
            return CreatedAtAction(nameof(GetFuncionario), new { id = funcionario.IdFuncionario }, output);
        }

        // PUT: api/funcionarios/5
        [HttpPut("{id}")]
        [SwaggerOperation(Summary = "Atualiza um funcionário")]
        public async Task<IActionResult> UpdateFuncionario(int id, [FromBody] FuncionarioInputDTO input)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var funcionario = await _context.Funcionarios.FindAsync(id);
            if (funcionario == null) return NotFound(new { Message = "Funcionário não encontrado." });

            funcionario.Nome = input.Nome;
            funcionario.Cpf = input.Cpf;
            funcionario.Cargo = input.Cargo;
            funcionario.Telefone = input.Telefone;
            funcionario.Email = input.Email;
            funcionario.Senha = HashSenha(input.Senha); // Atualiza hash da senha

            _context.Entry(funcionario).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/funcionarios/5
        [HttpDelete("{id}")]
        [SwaggerOperation(Summary = "Deleta um funcionário")]
        public async Task<IActionResult> DeleteFuncionario(int id)
        {
            var funcionario = await _context.Funcionarios.FindAsync(id);
            if (funcionario == null) return NotFound(new { Message = "Funcionário não encontrado." });

            _context.Funcionarios.Remove(funcionario);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        // === Helpers ===

        // Mapeia Funcionario para FuncionarioOutputDTO
        private FuncionarioOutputDTO MapToOutputDTO(Funcionario f)
        {
            return new FuncionarioOutputDTO
            {
                IdFuncionario = f.IdFuncionario,
                Nome = f.Nome,
                Cpf = f.Cpf,
                Cargo = f.Cargo,
                Telefone = f.Telefone,
                Email = f.Email
            };
        }

        // Gera hash da senha
        private string HashSenha(string senha)
        {
            using var sha256 = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(senha);
            var hash = sha256.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }
    }
}
