using EcoSENA.Api.Models;
using EcoSENA.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EcoSENA.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController(IAuthService authService) : ControllerBase
    {
        [HttpPost("register")]
        public async Task<ActionResult> Register(RegisterReqDto req)
        {
            var usuario = await authService.RegisterAsync(req);

            if(usuario == null)
            {
                return BadRequest("algo salio mal");
            }

            return Ok();
        }

        [HttpPost("login")]
        public async Task<ActionResult<LoginResDto?>> Login(LoginReqDto req)
        {
            var usuario = await authService.LoginAsync(req);
            
            if (usuario == null)
            {
                return BadRequest("Documento o contraseña incorrectos");
            }

            return Ok(usuario);
        }

        //ENDPOINTS DE PRUEBA, DESPUES SE ELIMINARAN
        [Authorize]
        [HttpPost("logged-user")]
        public ActionResult Logged()
        {
            return Ok("usuario loggeado");
        }

        [Authorize(Roles ="Administrador")]
        [HttpPost("logged-admin")]
        public ActionResult LoggedAdmin()
        {
            return Ok("admin loggeado");
        }
    }
}
