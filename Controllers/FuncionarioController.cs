using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MottuFlowApi.Data;
using MottuFlowApi.DTOs;
using MottuFlow.Models;
using MottuFlow.Hateoas;  // Importa o namespace HATEOAS
using Swashbuckle.AspNetCore.Annotations;
using System.Security.Cryptography;
using System.Text;

namespace MottuFlowApi.Controllers
{
    [ApiController]
    [Route("api/funcionarios")]
    [Tags("Funcionario")] // 🔹 Tag para ordenar no Swagger
    public class FuncionarioController : ControllerBase
    {
        private readonly AppDbContext _context;

        public FuncionarioController(AppDbContext context) => _context = context;

        // === Helpers ===

        private FuncionarioOutputDTO MapToOutputDTO(Funcionario f) => new FuncionarioOutputDTO
        {
            IdFuncionario = f.IdFuncionario,
            Nome = f.Nome ?? "Nome Padrão",
            Cpf = f.CPF ?? "CPF Padrão",
            Cargo = f.Cargo ?? "Cargo Padrão",
            Telefone = f.Telefone ?? "Telefone Padrão",
            Email = f.Email ?? "Email Padrão"
        };

        private void AddHateoasLinks(FuncionarioResource funcionarioResource, int? id)
        {
            if (funcionarioResource == null || id == null)
                throw new ArgumentNullException("FuncionarioResource ou id não podem ser nulos");

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

        // GET: api/funcionarios?page=1&pageSize=10
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

        // GET: api/funcionarios/{id}
        [HttpGet("{id}", Name = "GetFuncionario")]
        [SwaggerOperation(Summary = "Retorna funcionário por ID")]
        public async Task<ActionResult<FuncionarioResource>> GetFuncionario(int id)
        {
            var funcionario = await _context.Funcionarios
                .Include(f => f.RegistrosStatus)
                .FirstOrDefaultAsync(f => f.IdFuncionario == id);

            if (funcionario == null) 
                return NotFound(new { Message = "Funcionário não encontrado." });

            var funcionarioResource = new FuncionarioResource
            {
                Id = funcionario.IdFuncionario,
                Nome = funcionario.Nome ?? "Nome Padrão",
                Cpf = funcionario.CPF ?? "CPF Padrão",
                Cargo = funcionario.Cargo ?? "Cargo Padrão",
                Telefone = funcionario.Telefone ?? "Telefone Padrão",
                Email = funcionario.Email ?? "Email Padrão"
            };

            AddHateoasLinks(funcionarioResource, funcionario.IdFuncionario);

            return Ok(funcionarioResource);
        }

        // POST: api/funcionarios
        [HttpPost]
        [SwaggerOperation(Summary = "Cria um novo funcionário")]
        public async Task<ActionResult<FuncionarioResource>> CreateFuncionario([FromBody] FuncionarioInputDTO input)
        {
            if (!ModelState.IsValid) 
                return BadRequest(ModelState);

            var funcionario = new Funcionario
            {
                Nome = input.Nome ?? "Nome Padrão",
                CPF = input.Cpf ?? "CPF Padrão",
                Cargo = input.Cargo ?? "Cargo Padrão",
                Telefone = input.Telefone ?? "Telefone Padrão",
                Email = input.Email ?? "Email Padrão",
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

        // PUT: api/funcionarios/{id}
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

        // DELETE: api/funcionarios/{id}
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
