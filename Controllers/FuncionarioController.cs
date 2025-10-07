using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MottuFlowApi.Data;
using MottuFlow.Models;
using MottuFlowApi.DTOs;
using MottuFlow.Hateoas;
using Swashbuckle.AspNetCore.Annotations;
using System.Security.Cryptography;
using System.Text;

namespace MottuFlowApi.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/funcionarios")]
    [Tags("Funcionários")]
    public class FuncionarioController : ControllerBase
    {
        private readonly AppDbContext _context;
        public FuncionarioController(AppDbContext context) => _context = context;

        // 🔒 Criação do hash da senha
        private string HashSenha(string senha)
        {
            using var sha256 = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(senha ?? string.Empty);
            var hash = sha256.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }

        // 🔗 Adiciona links HATEOAS
        private void AddHateoasLinks(FuncionarioResource resource, int id)
        {
            resource.AddLink(new Link { Href = Url.Link(nameof(GetFuncionario), new { id })!, Rel = "self", Method = "GET" });
            resource.AddLink(new Link { Href = Url.Link(nameof(UpdateFuncionario), new { id })!, Rel = "update", Method = "PUT" });
            resource.AddLink(new Link { Href = Url.Link(nameof(DeleteFuncionario), new { id })!, Rel = "delete", Method = "DELETE" });
        }

        // 🧩 GET (todos)
        [HttpGet(Name = "GetFuncionarios")]
        [SwaggerOperation(Summary = "Lista todos os funcionários com paginação e HATEOAS")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetFuncionarios(int page = 1, int pageSize = 10)
        {
            page = Math.Max(page, 1);
            pageSize = Math.Max(pageSize, 1);

            var totalItems = await _context.Funcionarios.CountAsync();

            var funcionarios = await _context.Funcionarios
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(f => new FuncionarioResource
                {
                    Id = f.IdFuncionario,
                    Nome = f.Nome!,
                    Cpf = f.CPF!,
                    Cargo = f.Cargo!,
                    Telefone = f.Telefone!,
                    Email = f.Email!
                })
                .ToListAsync();

            funcionarios.ForEach(f => AddHateoasLinks(f, f.Id));

            var meta = new
            {
                totalItems,
                page,
                pageSize,
                totalPages = Math.Ceiling((double)totalItems / pageSize)
            };

            return Ok(new { success = true, meta, data = funcionarios });
        }

        // 🧩 GET (por ID)
        [HttpGet("{id}", Name = "GetFuncionario")]
        [SwaggerOperation(Summary = "Retorna um funcionário por ID")]
        [ProducesResponseType(typeof(FuncionarioResource), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetFuncionario(int id)
        {
            var funcionario = await _context.Funcionarios
                .Where(f => f.IdFuncionario == id)
                .Select(f => new FuncionarioResource
                {
                    Id = f.IdFuncionario,
                    Nome = f.Nome!,
                    Cpf = f.CPF!,
                    Cargo = f.Cargo!,
                    Telefone = f.Telefone!,
                    Email = f.Email!
                })
                .FirstOrDefaultAsync();

            if (funcionario == null)
                return NotFound(new { success = false, message = "Funcionário não encontrado." });

            AddHateoasLinks(funcionario, funcionario.Id);
            return Ok(new { success = true, data = funcionario });
        }

        // 🧩 POST
        [HttpPost(Name = "CreateFuncionario")]
        [SwaggerOperation(Summary = "Cria um novo funcionário")]
        [ProducesResponseType(typeof(FuncionarioResource), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateFuncionario([FromBody] FuncionarioInputDTO input)
        {
            if (input == null)
                return BadRequest(new { success = false, message = "Input não pode ser nulo." });

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

            var resource = new FuncionarioResource
            {
                Id = funcionario.IdFuncionario,
                Nome = funcionario.Nome,
                Cpf = funcionario.CPF,
                Cargo = funcionario.Cargo,
                Telefone = funcionario.Telefone,
                Email = funcionario.Email
            };

            AddHateoasLinks(resource, funcionario.IdFuncionario);

            return CreatedAtAction(nameof(GetFuncionario), new { id = funcionario.IdFuncionario }, new { success = true, data = resource });
        }

        // 🧩 PUT
        [HttpPut("{id}", Name = "UpdateFuncionario")]
        [SwaggerOperation(Summary = "Atualiza um funcionário existente")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateFuncionario(int id, [FromBody] FuncionarioInputDTO input)
        {
            if (input == null)
                return BadRequest(new { success = false, message = "Input não pode ser nulo." });

            var funcionario = await _context.Funcionarios.FindAsync(id);
            if (funcionario == null)
                return NotFound(new { success = false, message = "Funcionário não encontrado." });

            funcionario.Nome = input.Nome;
            funcionario.CPF = input.Cpf;
            funcionario.Cargo = input.Cargo;
            funcionario.Telefone = input.Telefone;
            funcionario.Email = input.Email;

            if (!string.IsNullOrEmpty(input.Senha))
                funcionario.Senha = HashSenha(input.Senha);

            _context.Entry(funcionario).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // 🧩 DELETE
        [HttpDelete("{id}", Name = "DeleteFuncionario")]
        [SwaggerOperation(Summary = "Remove um funcionário pelo ID")]
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
}
