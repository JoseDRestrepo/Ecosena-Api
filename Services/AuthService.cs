using BC = BCrypt.Net.BCrypt;
using EcoSENA.Api.Data;
using EcoSENA.Api.Entities;
using EcoSENA.Api.Models;
using Microsoft.EntityFrameworkCore;


namespace EcoSENA.Api.Services
{
    public class AuthService(EcosenaDbContext context, IConfiguration configuration) : IAuthService
    {
        public async Task<LoginResDto?> LoginAsync(LoginReqDto req)
        {
            var user = await context.Usuarios.FirstOrDefaultAsync(u => u.Documento == req.Documento);

            if (user == null)
            {
                return null;
            }

            if(!BC.Verify(req.Contraseña, user.ContraseñaHash))
            {
                return null;
            }

            return new LoginResDto { JWT = "Autenticated" };
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
    }
}
