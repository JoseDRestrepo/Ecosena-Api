using EcoSENA.Api.Data;
using EcoSENA.Api.Entities;

using BC = BCrypt.Net.BCrypt;
using System.Security.Claims;
using EcoSENA.Api.Interfaces;
using EcoSENA.Api.Models.Profile;

namespace EcoSENA.Api.Services
{
    public class ProfileService(EcosenaDbContext context) : IProfileService
    {
        public async Task<ProfileResDto> GetProfileAsync(int? id)
        {
            var user = await context.Usuarios.FindAsync(id);

            if (user == null) 
            {
                return null;
            }

            return new ProfileResDto
            {
                FotoPerfil = user.FotoPerfil,
                Nombre = user.Nombre,
                Ficha = user.Ficha,
                Programa = user.ProgramaFormacion,
                Email = user.Correo,
                FechaNacimiento = (DateOnly)user.FechaNacimiento
            };
        }

        public async Task<Usuario> UpdateProfileAsync(int? id, EditProfileReqDto req)
        {
            var user = await context.Usuarios.FindAsync(id);

            if (user == null)
            {
                return null;
            }

            if (!string.IsNullOrEmpty(req.Email))
            {
                user.Correo = req.Email;
            }

            if (req.FechaNacimiento != null)
            {
                user.FechaNacimiento = (DateOnly)req.FechaNacimiento;
            }

            if (!string.IsNullOrEmpty(req.Contraseña) && !string.IsNullOrEmpty(req.ConfirmacionContraseña))
            {
                user.ContraseñaHash = BC.HashPassword(req.Contraseña);
            }

            await context.SaveChangesAsync();
            return user;
        }
    }
}
