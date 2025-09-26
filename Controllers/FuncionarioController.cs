using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MottuFlowApi.Data;
using MottuFlowApi.DTOs;
using MottuFlow.Models;
using MottuFlow.Hateoas;
using Swashbuckle.AspNetCore.Annotations;
using System.Security.Cryptography;
using System.Text;

namespace MottuFlowApi.Controllers
{
    [ApiController]
    [Route("api/funcionarios")]
    [Tags("Funcionario")]
    public class FuncionarioController : ControllerBase
    {
        private readonly AppDbContext _context;

        public FuncionarioController(AppDbContext context) => _context = context;

        // === Helpers ===
        private FuncionarioOutputDTO MapToOutputDTO(Funcionario f) => new FuncionarioOutputDTO
        {
            IdFuncionario = f.IdFuncionario,
            Nome = f.Nome!,
            Cpf = f.CPF!,
            Cargo = f.Cargo!,
            Telefone = f.Telefone!,
            Email = f.Email!
        };

        private void AddHateoasLinks(FuncionarioResource funcionarioResource, int id)
        {
            funcionarioResource.AddLink(new Link
            {
                Href = Url.Link("GetFuncionario", new { id }),
                Rel = "self",
                Method = "GET"
            });

            funcionarioResource.AddLink(new Link
            {
                Href = Url.Link("UpdateFuncionario", new { id }),
                Rel = "update",
                Method = "PUT"
            });

            funcionarioResource.AddLink(new Link
            {
                Href = Url.Link("DeleteFuncionario", new { id }),
                Rel = "delete",
                Method = "DELETE"
            });
        }

        // === Endpoints ===

        [HttpGet]
        [SwaggerOperation(Summary = "Lista todos os funcionários com paginação")]
        public async Task<ActionResult<IEnumerable<FuncionarioOutputDTO>>> GetFuncionarios(int page = 1, int pageSize = 10)
        {
            var funcionarios = await _context.Funcionarios
                .Include(f => f.RegistrosStatus)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return Ok(funcionarios.Select(f => MapToOutputDTO(f)));
        }

        [HttpGet("{id}", Name = "GetFuncionario")]
        [SwaggerOperation(Summary = "Retorna funcionário por ID")]
        public async Task<ActionResult<FuncionarioResource>> GetFuncionario(int id)
        {
            var funcionario = await _context.Funcionarios
                .Include(f => f.RegistrosStatus)
                .FirstOrDefaultAsync(f => f.IdFuncionario == id)
                ?? throw new KeyNotFoundException("Funcionário não encontrado.");

            var funcionarioResource = new FuncionarioResource
            {
                Id = funcionario.IdFuncionario,
                Nome = funcionario.Nome!,
                Cpf = funcionario.CPF!,
                Cargo = funcionario.Cargo!,
                Telefone = funcionario.Telefone!,
                Email = funcionario.Email!
            };

            AddHateoasLinks(funcionarioResource, funcionario.IdFuncionario);
            return Ok(funcionarioResource);
        }

        [HttpPost]
        [SwaggerOperation(Summary = "Cria um novo funcionário")]
        public async Task<ActionResult<FuncionarioResource>> CreateFuncionario([FromBody] FuncionarioInputDTO input)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Campos obrigatórios devem ser preenchidos
            if (input.Nome is null || input.Cpf is null || input.Cargo is null || input.Telefone is null || input.Email is null || input.Senha is null)
                return BadRequest("Todos os campos são obrigatórios.");

            var funcionario = new Funcionario
            {
                Nome = input.Nome,
                CPF = input.Cpf,
                Cargo = input.Cargo,
                Telefone = input.Telefone,
                Email = input.Email,
                Senha = HashSenha(input.Senha)
            };

            _context.Funcionarios.Add(funcionario);
            await _context.SaveChangesAsync();

            var funcionarioResource = new FuncionarioResource
            {
                Id = funcionario.IdFuncionario,
                Nome = funcionario.Nome,
                Cpf = funcionario.CPF,
                Cargo = funcionario.Cargo,
                Telefone = funcionario.Telefone,
                Email = funcionario.Email
            };

            AddHateoasLinks(funcionarioResource, funcionario.IdFuncionario);
            return CreatedAtAction(nameof(GetFuncionario), new { id = funcionario.IdFuncionario }, funcionarioResource);
        }

        [HttpPut("{id}", Name = "UpdateFuncionario")]
        [SwaggerOperation(Summary = "Atualiza um funcionário")]
        public async Task<IActionResult> UpdateFuncionario(int id, [FromBody] FuncionarioInputDTO input)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var funcionario = await _context.Funcionarios.FindAsync(id);
            if (funcionario == null)
                return NotFound(new { Message = "Funcionário não encontrado." });

            funcionario.Nome = input.Nome ?? funcionario.Nome;
            funcionario.CPF = input.Cpf ?? funcionario.CPF;
            funcionario.Cargo = input.Cargo ?? funcionario.Cargo;
            funcionario.Telefone = input.Telefone ?? funcionario.Telefone;
            funcionario.Email = input.Email ?? funcionario.Email;

            if (input.Senha != null)
                funcionario.Senha = HashSenha(input.Senha);

            _context.Entry(funcionario).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            var funcionarioResource = new FuncionarioResource
            {
                Id = funcionario.IdFuncionario,
                Nome = funcionario.Nome,
                Cpf = funcionario.CPF,
                Cargo = funcionario.Cargo,
                Telefone = funcionario.Telefone,
                Email = funcionario.Email
            };

            AddHateoasLinks(funcionarioResource, funcionario.IdFuncionario);
            return Ok(funcionarioResource);
        }

        [HttpDelete("{id}", Name = "DeleteFuncionario")]
        [SwaggerOperation(Summary = "Deleta um funcionário")]
        public async Task<IActionResult> DeleteFuncionario(int id)
        {
            var funcionario = await _context.Funcionarios.FindAsync(id);
            if (funcionario == null)
                return NotFound(new { Message = "Funcionário não encontrado." });

            _context.Funcionarios.Remove(funcionario);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        // === Helpers ===
        private string HashSenha(string senha)
        {
            using var sha256 = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(senha);
            var hash = sha256.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }
    }
}
