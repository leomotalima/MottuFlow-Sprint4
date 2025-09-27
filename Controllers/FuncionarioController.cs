using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MottuFlowApi.Data;
using MottuFlow.Models;
<<<<<<< HEAD
=======
using MottuFlow.DTOs;
using MottuFlowApi.DTOs;
>>>>>>> 5974efe (Atualiza controllers e DTOs: Patio e ArucoTag, ajusta HATEOAS e Swagger)
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

<<<<<<< HEAD
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
=======
        private void AddHateoasLinks(FuncionarioResource resource, int id)
        {
            resource.AddLink(new Link { Href = Url.Link(nameof(GetFuncionario), new { id }), Rel = "self", Method = "GET" });
            resource.AddLink(new Link { Href = Url.Link(nameof(UpdateFuncionario), new { id }), Rel = "update", Method = "PUT" });
            resource.AddLink(new Link { Href = Url.Link(nameof(DeleteFuncionario), new { id }), Rel = "delete", Method = "DELETE" });
        }

        private string HashSenha(string senha)
>>>>>>> 5974efe (Atualiza controllers e DTOs: Patio e ArucoTag, ajusta HATEOAS e Swagger)
        {
            using var sha256 = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(senha ?? string.Empty);
            var hash = sha256.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }

        [HttpGet(Name = "GetFuncionarios")]
        [SwaggerOperation(Summary = "Lista todos os funcionários com paginação e HATEOAS")]
        public async Task<IActionResult> GetFuncionarios(int page = 1, int pageSize = 10)
        {
            page = page < 1 ? 1 : page;
            pageSize = pageSize < 1 ? 10 : pageSize;

            var totalItems = await _context.Funcionarios.CountAsync();

            var funcionarios = await _context.Funcionarios
                .Include(f => f.RegistrosStatus)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var data = funcionarios.Select(f =>
            {
                var resource = new FuncionarioResource
                {
                    Id = f.IdFuncionario,
                    Nome = f.Nome ?? string.Empty,
                    Cpf = f.CPF ?? string.Empty,
                    Cargo = f.Cargo ?? string.Empty,
                    Telefone = f.Telefone ?? string.Empty,
                    Email = f.Email ?? string.Empty
                };
                AddHateoasLinks(resource, f.IdFuncionario);
                return resource;
            }).ToList();

            var meta = new
            {
                totalItems,
                page,
                pageSize,
                totalPages = Math.Ceiling((double)totalItems / pageSize)
            };

            return Ok(new { meta, data });
        }

        [HttpGet("{id}", Name = "GetFuncionario")]
        [SwaggerOperation(Summary = "Retorna funcionário por ID")]
        public async Task<ActionResult<FuncionarioResource>> GetFuncionario(int id)
        {
            var f = await _context.Funcionarios
                .Include(f => f.RegistrosStatus)
<<<<<<< HEAD
                .FirstOrDefaultAsync(f => f.IdFuncionario == id)
                ?? throw new KeyNotFoundException("Funcionário não encontrado.");
=======
                .FirstOrDefaultAsync(f => f.IdFuncionario == id);

            if (f == null) return NotFound(new { Message = "Funcionário não encontrado." });
>>>>>>> 5974efe (Atualiza controllers e DTOs: Patio e ArucoTag, ajusta HATEOAS e Swagger)

            var resource = new FuncionarioResource
            {
<<<<<<< HEAD
                Id = funcionario.IdFuncionario,
                Nome = funcionario.Nome!,
                Cpf = funcionario.CPF!,
                Cargo = funcionario.Cargo!,
                Telefone = funcionario.Telefone!,
                Email = funcionario.Email!
=======
                Id = f.IdFuncionario,
                Nome = f.Nome ?? string.Empty,
                Cpf = f.CPF ?? string.Empty,
                Cargo = f.Cargo ?? string.Empty,
                Telefone = f.Telefone ?? string.Empty,
                Email = f.Email ?? string.Empty
>>>>>>> 5974efe (Atualiza controllers e DTOs: Patio e ArucoTag, ajusta HATEOAS e Swagger)
            };
            AddHateoasLinks(resource, f.IdFuncionario);

<<<<<<< HEAD
            AddHateoasLinks(funcionarioResource, funcionario.IdFuncionario);
            return Ok(funcionarioResource);
        }

        [HttpPost]
        [SwaggerOperation(Summary = "Cria um novo funcionário")]
        public async Task<ActionResult<FuncionarioResource>> CreateFuncionario([FromBody] FuncionarioInputDTO input)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
=======
            return Ok(resource);
        }

        [HttpPost(Name = "CreateFuncionario")]
        [SwaggerOperation(Summary = "Cria um novo funcionário")]
        public async Task<ActionResult<FuncionarioResource>> CreateFuncionario([FromBody] FuncionarioInputDTO input)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
>>>>>>> 5974efe (Atualiza controllers e DTOs: Patio e ArucoTag, ajusta HATEOAS e Swagger)

            // Campos obrigatórios devem ser preenchidos
            if (input.Nome is null || input.Cpf is null || input.Cargo is null || input.Telefone is null || input.Email is null || input.Senha is null)
                return BadRequest("Todos os campos são obrigatórios.");

            var funcionario = new Funcionario
            {
<<<<<<< HEAD
                Nome = input.Nome,
                CPF = input.Cpf,
                Cargo = input.Cargo,
                Telefone = input.Telefone,
                Email = input.Email,
=======
                Nome = input.Nome ?? string.Empty,
                CPF = input.Cpf ?? string.Empty,
                Cargo = input.Cargo ?? string.Empty,
                Telefone = input.Telefone ?? string.Empty,
                Email = input.Email ?? string.Empty,
>>>>>>> 5974efe (Atualiza controllers e DTOs: Patio e ArucoTag, ajusta HATEOAS e Swagger)
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

<<<<<<< HEAD
            AddHateoasLinks(funcionarioResource, funcionario.IdFuncionario);
            return CreatedAtAction(nameof(GetFuncionario), new { id = funcionario.IdFuncionario }, funcionarioResource);
=======
            return CreatedAtAction(nameof(GetFuncionario), new { id = funcionario.IdFuncionario }, resource);
>>>>>>> 5974efe (Atualiza controllers e DTOs: Patio e ArucoTag, ajusta HATEOAS e Swagger)
        }

        [HttpPut("{id}", Name = "UpdateFuncionario")]
        [SwaggerOperation(Summary = "Atualiza um funcionário")]
        public async Task<IActionResult> UpdateFuncionario(int id, [FromBody] FuncionarioInputDTO input)
        {
<<<<<<< HEAD
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
=======
            var f = await _context.Funcionarios.FindAsync(id);
            if (f == null) return NotFound(new { Message = "Funcionário não encontrado." });

            f.Nome = input.Nome ?? f.Nome;
            f.CPF = input.Cpf ?? f.CPF;
            f.Cargo = input.Cargo ?? f.Cargo;
            f.Telefone = input.Telefone ?? f.Telefone;
            f.Email = input.Email ?? f.Email;
            f.Senha = HashSenha(input.Senha);

            _context.Entry(f).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
>>>>>>> 5974efe (Atualiza controllers e DTOs: Patio e ArucoTag, ajusta HATEOAS e Swagger)
        }

        [HttpDelete("{id}", Name = "DeleteFuncionario")]
        [SwaggerOperation(Summary = "Deleta um funcionário")]
        public async Task<IActionResult> DeleteFuncionario(int id)
        {
<<<<<<< HEAD
            var funcionario = await _context.Funcionarios.FindAsync(id);
            if (funcionario == null)
                return NotFound(new { Message = "Funcionário não encontrado." });
=======
            var f = await _context.Funcionarios.FindAsync(id);
            if (f == null) return NotFound(new { Message = "Funcionário não encontrado." });
>>>>>>> 5974efe (Atualiza controllers e DTOs: Patio e ArucoTag, ajusta HATEOAS e Swagger)

            _context.Funcionarios.Remove(f);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
