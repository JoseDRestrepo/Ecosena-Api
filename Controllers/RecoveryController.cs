using EcoSENA.Api.Interfaces;
using EcoSENA.Api.Models.Recover;
using EcoSENA.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EcoSENA.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    [EnableCors("AllowRecovery")]
    public class RecoveryController(IRecuperacionService service) : ControllerBase
    {
        [HttpPost("solicitar")]
        public async Task<IActionResult> Solicitar(GetResetCodeReq req)
        {
            await service.SolicitarRecuperacionAsync(req.Documento, req.Email);

            return Ok("Si los datos son correctos, recibirá un código en su correo");
        }

        [HttpPost("reestablecer")]
        public async Task<IActionResult> Restablecer(ResetPasswordReq req)
        {
            if (req.NuevaContraseña != req.ConfirmacionContraseña)
            {
                return BadRequest("Las contraseñas no coinsiden");
            }

            var exito = await service.RestablecerContrasenaAsync(req.Codigo, req.NuevaContraseña);

            if (!exito)
            {
                return BadRequest("Código inválido o expirado.");
            }

            return Ok("Contraseña actualizada correctamente.");
        }
    }
}
