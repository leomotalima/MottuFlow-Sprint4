using Microsoft.AspNetCore.Mvc;
using MottuFlowApi.Services;
using MottuFlowApi.DTOs; // 👈 IMPORTANTE: adiciona este using
using Swashbuckle.AspNetCore.Annotations;

namespace MottuFlowApi.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/auth")]
    [Tags("Autenticação")]
    [Produces("application/json")] // ✅ Força saída JSON no Swagger
    public class AuthController : ControllerBase
    {
        private readonly JwtService _jwtService;

        public AuthController(JwtService jwtService)
        {
            _jwtService = jwtService;
        }

        // 🧩 POST - Login
        [HttpPost("login")]
        [SwaggerOperation(
            Summary = "Autentica um usuário e gera um token JWT",
            Description = "Recebe credenciais (usuário e senha) e retorna um token JWT válido para uso na API.")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult Login([FromBody] AuthLoginInputDTO request) // 👈 usa o DTO criado na pasta DTOs
        {
            if (request == null)
                return BadRequest(new { success = false, message = "Requisição inválida. Verifique o corpo da requisição." });

            // 🔐 Exemplo fixo (em produção seria validado no banco)
            if (request.Username == "admin" && request.Password == "123")
            {
                var token = _jwtService.GenerateToken(request.Username, "Admin");
                return Ok(new
                {
                    success = true,
                    message = "Autenticação realizada com sucesso.",
                    data = new { token, role = "Admin" }
                });
            }

            return Unauthorized(new { success = false, message = "Credenciais inválidas." });
        }
    }
}
