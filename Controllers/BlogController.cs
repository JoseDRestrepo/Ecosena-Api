using EcoSENA.Api.Entities;
using EcoSENA.Api.Interfaces;
using EcoSENA.Api.Models.Blog;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using System.Security.Claims;

namespace EcoSENA.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableRateLimiting("general")]
    public class BlogController(IBlogService service) : ControllerBase
    {
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<List<BlogListResDto>>> GetEntradas([FromQuery] string? titulo = null)
        {
            var entradas = await service.GetEntradasAsync(titulo);

            return Ok(entradas);
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<EntradaResDto>> GetEntradaById(int id)
        {
            var entrada = await service.GetEntradaAsync(id);

            if (entrada == null)
            {
                return NotFound();
            }

            return Ok(entrada);
        }

        [HttpPost]
        [Authorize(Roles = "Administrador")]
        [EnableRateLimiting("uploads")]
        public async Task<ActionResult<EntradaResDto>> PostEntrada(PostEntradaReqDto req)
        {
            int id = GetUserIdFromToken();
            var entrada = await service.PostEntradaAsync(req, id);
            if (entrada == null)
            {
                return BadRequest();
            }

            return CreatedAtAction(nameof(GetEntradaById), new { id = entrada.Id }, entrada);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Administrador")]
        public async Task<ActionResult> UpdateEntrada(int id, EditEntradaReqDto req)
        {
            var resultado = await service.UpdateEntradaAsync(id, req);

            if (resultado == false)
            {
                return NotFound();
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles ="Administrador")]
        public async Task<ActionResult> DeleteEntrada(int id)
        {
            var resultado = await service.DeleteEntradaAsync(id);
            if (resultado == false)
            {
                return NotFound();
            }

            return NoContent();
        }

        private int GetUserIdFromToken()
        {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(claim) || !int.TryParse(claim, out var id))
            {
                return 0;
            }
            return id;
        }
    }
}
