using Microsoft.AspNetCore.Mvc;
using MottuFlowApi.Services;
using MottuFlowApi.DTOs;
using MottuFlowApi.Utils;
using Swashbuckle.AspNetCore.Annotations;

namespace MottuFlowApi.Controllers.V1
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/auth")]
    [Tags("Autenticação")]
    [Produces("application/json")]
    [Consumes("application/json")]
    public class AuthController : ControllerBase
    {
        private readonly JwtService _jwtService;

        public AuthController(JwtService jwtService)
        {
            _jwtService = jwtService;
        }

        // POST - Login
        [HttpPost("login")]
        [SwaggerOperation(
            Summary = "Autentica um usuário e gera um token JWT",
            Description = "Recebe credenciais (usuário e senha) e retorna um token JWT válido para acesso aos endpoints protegidos da API.")]
        [SwaggerResponse(StatusCodes.Status200OK, "Autenticação realizada com sucesso")]
        [SwaggerResponse(StatusCodes.Status401Unauthorized, "Credenciais inválidas")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Erro de validação nos dados de entrada")]
        public IActionResult Login([FromBody] AuthLoginInputDTO request)
        {
            if (request == null)
                return BadRequest(ApiResponse<string>.Fail("Requisição inválida. Verifique o corpo da requisição."));

            if (string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Password))
                return BadRequest(ApiResponse<string>.Fail("Usuário e senha são obrigatórios."));

            // Exemplo fixo — substituir por validação real no banco
            if (request.Username == "admin" && request.Password == "123")
            {
                var token = _jwtService.GenerateToken(request.Username, "Admin");

                var data = new
                {
                    token,
                    role = "Admin",
                    expiresIn = "2h"
                };

                return Ok(ApiResponse<object>.Ok(data, "Autenticação realizada com sucesso."));
            }

            return Unauthorized(ApiResponse<string>.Fail("Credenciais inválidas. Usuário ou senha incorretos."));
        }
    }
}
