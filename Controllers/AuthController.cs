using Microsoft.AspNetCore.Mvc;
using MottuFlowApi.Services;
using Swashbuckle.AspNetCore.Annotations;

namespace MottuFlowApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly JwtService _jwtService;

        public AuthController(JwtService jwtService)
        {
            _jwtService = jwtService;
        }

        [HttpPost("login")]
        [SwaggerOperation(Summary = "Autentica um usuário e gera um token JWT")]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            // Login de exemplo
            if (request.Username == "admin" && request.Password == "123")
            {
                var token = _jwtService.GenerateToken(request.Username, "Admin");
                return Ok(new { token });
            }

            return Unauthorized(new { message = "Credenciais inválidas" });
        }
    }

    public class LoginRequest
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}
