using EcoSENA.Api.Interfaces;
using EcoSENA.Api.Models.Profile;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using System.Security.Claims;

namespace EcoSENA.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    [EnableRateLimiting("general")]
    public class ProfileController(IProfileService profileService) : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<ProfileResDto>> GetProfile()
        {
            var id = GetUserIdFromToken();

            var res = await profileService.GetProfileAsync(id);

            if (res == null)
            {
                return NotFound();
            }

            return Ok(res);
        }

        [HttpPut]
        [EnableRateLimiting("uploads")]
        public async Task<ActionResult> UpdateProfile(EditProfileReqDto req)
        {
            var id = GetUserIdFromToken();

            if ((!string.IsNullOrEmpty(req.Contraseña) && string.IsNullOrEmpty(req.ConfirmacionContraseña))
                || (string.IsNullOrEmpty(req.Contraseña) && !string.IsNullOrEmpty(req.ConfirmacionContraseña)))
            {
                return BadRequest("Debe ingresar la contraseña más la confirmación");
            } else if (req.Contraseña != req.ConfirmacionContraseña)
            {
                return BadRequest("Las contraseñas deben coincidir");
            }

            var usuario = await profileService.UpdateProfileAsync(id, req);

            if (usuario == null)
            {
                return NotFound();
            }

            return NoContent();
        }

        private int? GetUserIdFromToken()
        {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(claim) || !int.TryParse(claim, out var id))
            {
                return null; 
            }
            return id;
        }
    }
}
