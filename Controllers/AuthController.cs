using EcoSENA.Api.Interfaces;
using EcoSENA.Api.Models.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace EcoSENA.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableRateLimiting("auth")]
    public class AuthController(IAuthService authService) : ControllerBase
    {
        [HttpPost("register")]
        public async Task<ActionResult> Register(RegisterReqDto req)
        {
            if (string.IsNullOrEmpty(req.Contraseña) || string.IsNullOrEmpty(req.ConfirmacionContraseña))
            {
                return BadRequest("Debe ingresar una contraseña");
            }

            if (req.Contraseña != req.ConfirmacionContraseña)
            {
                return BadRequest("Las contraseñas no coinciden");
            }

            var usuario = await authService.RegisterAsync(req);

            if (usuario == null)
            {
                return BadRequest("Usuario ya registrado o inexistente en SofiaPlus");
            }

            return StatusCode(StatusCodes.Status201Created, new { message = "Usuario registrado correctamente" });
        }

        [HttpPost("login")]
        public async Task<ActionResult<LoginResDto?>> Login(LoginReqDto req)
        {
            var result = await authService.LoginAsync(req);

            if (result == null)
            {
                // mantener mensaje genérico para no filtrar existencia de usuarios
                return BadRequest("Documento o contraseña incorrectos");
            }

            if (result.IsLocked)
            {
                return StatusCode(423, new { message = result.Message ?? "Cuenta bloqueada", lockedUntil = result.LockedUntil });
            }

            if (string.IsNullOrEmpty(result.JWT))
            {
                // credenciales erróneas pero no bloqueado
                return BadRequest(new { message = result.Message ?? "Documento o contraseña incorrectos", remainingAttempts = result.RemainingAttempts });
            }

            return Ok(result);
        }
    }
}