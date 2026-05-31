using BC = BCrypt.Net.BCrypt;
using EcoSENA.Api.Data;
using EcoSENA.Api.Entities;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using EcoSENA.Api.Entities.Sofia;
using EcoSENA.Api.Interfaces;
using EcoSENA.Api.Models.Auth;


namespace EcoSENA.Api.Services
{
    public class AuthService(EcosenaDbContext context, SofiaDbContext sofiaContext, IConfiguration configuration) : IAuthService
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

            var sofiaUsuario = await sofiaContext.SofiaUsuarios
                .Include(us => us.Matriculas)
                .ThenInclude(m => m.Ficha)
                .ThenInclude(f => f.ProgramaFormacion)
                .FirstOrDefaultAsync(us => us.Documento == req.Documento);

            if (sofiaUsuario == null)
            {
                return null;
            }

            string hashed = BC.HashPassword(req.Contraseña);

            var usuario = new Usuario
            {
                Documento = req.Documento,
                Nombre = req.Nombre,
                Correo = sofiaUsuario.EmailPersonal,
                Apellido = req.Apellido,
                ContraseñaHash = hashed,
                FechaNacimiento = sofiaUsuario.FechaNacimiento,
                Ficha = sofiaUsuario.Matriculas.Select(m => m.Ficha?.Numero).FirstOrDefault(),
                ProgramaFormacion = sofiaUsuario.Matriculas.Select(m => m.Ficha?.ProgramaFormacion.Nombre).FirstOrDefault(),
                Rol = validarRol(sofiaUsuario.EmailSena)
            };

            context.Add(usuario);
            await context.SaveChangesAsync();

            return usuario;
        }

        private RolUsuario validarRol(string correoSena)
        {
            if (string.IsNullOrEmpty(correoSena))
            {
                return RolUsuario.Aprendiz;
            } 

            if (correoSena.Split('@').Last() == "sena.edu.co")
            {
                return RolUsuario.Administrador;
            }

            return RolUsuario.Aprendiz;
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
