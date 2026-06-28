using EcoSENA.Api.Data;
using EcoSENA.Api.Entities;
using EcoSENA.Api.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;
using BC = BCrypt.Net.BCrypt;

namespace EcoSENA.Api.Services
{
    public class RecuperacionService(EcosenaDbContext context, IEmailService emailService, IPasswordHasher<Usuario> hasher) : IRecuperacionService
    {
        public async Task<bool> RestablecerContrasenaAsync(string codigo, string nuevaContrasena)
        {
            var codigoHashed = Convert.ToBase64String(SHA256.HashData(Encoding.UTF8.GetBytes(codigo)));

            var registro = await context.TokensRecuperacion
            .Include(t => t.Usuario)
            .FirstOrDefaultAsync(t => t.CodigoHash == codigoHashed
                                   && !t.Usado
                                   && t.ExpiraEn > DateTime.UtcNow);

            if (registro?.Usuario is null)
            {
                return false;
            }

            registro.Usuario.ContraseñaHash = BC.HashPassword(nuevaContrasena);

            registro.Usado = true;

            await context.SaveChangesAsync();
            return true;
        }

        public async Task SolicitarRecuperacionAsync(string documento, string email)
        {
            var usuario = await context.Usuarios.FirstOrDefaultAsync(u=> u.Documento == documento && u.Correo == email);
            if (usuario == null)
            {
                return;
            }

            await context.TokensRecuperacion
                .Where(t => t.UsuarioId == usuario.Id && !t.Usado)
                .ExecuteUpdateAsync(t => t.SetProperty(x => x.Usado, true));

            var codigo = Random.Shared.Next(100000, 999999).ToString();
            var codigoHashed = Convert.ToBase64String(SHA256.HashData(Encoding.UTF8.GetBytes(codigo)));

            context.TokensRecuperacion.Add(new TokenRecuperacion
            {
                UsuarioId = usuario.Id,
                CodigoHash = codigoHashed,
                ExpiraEn = DateTime.UtcNow.AddMinutes(15),
                Usado = false
            });

            await context.SaveChangesAsync();

            await emailService.EnviarCodigoRecuperacionAsync(usuario.Correo, codigo);
        }
    }
}
