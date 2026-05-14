using BC = BCrypt.Net.BCrypt;
using EcoSENA.Api.Data;
using EcoSENA.Api.Entities;
using EcoSENA.Api.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.IdentityModel.Tokens.Jwt;


namespace EcoSENA.Api.Services
{
    public class AuthService(EcosenaDbContext context, IConfiguration configuration) : IAuthService
    {
        public async Task<LoginResDto?> LoginAsync(LoginReqDto req)
        {
            var usuario = await context.Usuarios.FirstOrDefaultAsync(u => u.Documento == req.Documento);

            if (usuario == null)
            {
                return null;
            }

            if(!BC.Verify(req.Contraseña, usuario.ContraseñaHash))
            {
                return null;
            }

            return new LoginResDto { JWT = GenerateToken(usuario) };
        }

        public async Task<Usuario?> RegisterAsync(RegisterReqDto req)
        {
            if(await context.Usuarios.AnyAsync(u => u.Documento == req.Documento))
            {
                return null;
            }

            if(string.IsNullOrEmpty(req.Contraseña) || string.IsNullOrEmpty(req.ConfirmacionContraseña))
            {
                return null;
            }

            if(req.Contraseña != req.ConfirmacionContraseña)
            {
                return null;
            }

            string hashed = BC.HashPassword(req.Contraseña);

            var usuario = new Usuario
            {
                Documento = req.Documento,
                Nombre = req.Nombre,
                Correo = string.Empty,
                Apellido = req.Apellido,
                ContraseñaHash = hashed,
                Rol = RolUsuario.Aprendiz
            };

            context.Add(usuario);
            await context.SaveChangesAsync();

            return usuario;
        }

        private string GenerateToken(Usuario usuario)
        {
            var claims = new List<Claim> {
                new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
                new Claim(ClaimTypes.Name, $"{usuario.Nombre} {usuario.Apellido}"),
                new Claim(ClaimTypes.Role, usuario.Rol.ToString())
            };

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(configuration.GetValue<string>("JwtConfig:Key")!));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512);

            var tokenDesc = new JwtSecurityToken(
                issuer: configuration.GetValue<string>("JwtConfig:Issuer"),
                audience: configuration.GetValue<string>("JwtConfig:Audience"),
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(configuration.GetValue<int>("JwtConfig:TokenValidityMins")),
                signingCredentials: creds
                );

            return new JwtSecurityTokenHandler().WriteToken(tokenDesc);
        }
    }
}
