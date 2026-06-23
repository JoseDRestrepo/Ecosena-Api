using DotnetBadWordDetector;
using EcoSENA.Api.Interfaces;
using System.Text.Json;

namespace EcoSENA.Api.Services
{
    public class CensorshipService : ICensorshipService
    {
        private readonly ProfanityDetector detector;
        private readonly List<string> colombianismos;

        public CensorshipService(IWebHostEnvironment env)
        {
            detector = new ProfanityDetector(allLocales: true);
            colombianismos = new List<string>();
            var jsonPath = Path.Combine(env.ContentRootPath, "Resources", "colombianismos.json");

            if (File.Exists(jsonPath))
            {
                var json = File.ReadAllText(jsonPath);
                var doc = JsonDocument.Parse(json);
                foreach(var palabra in doc.RootElement.GetProperty("words").EnumerateArray())
                {
                    var p = palabra.GetString();
                    if (!string.IsNullOrWhiteSpace(p))
                    {
                        colombianismos.Add(p.ToLowerInvariant());
                    }
                }
            }
        }

        public bool EsSoez(string texto)
        {
            if (detector.IsPhraseProfane(texto))
            {
                return true;
            }

            var normalizado = NormalizarTexto(texto);

            var palabras = normalizado.Split([' ', ',', '.', '?', '\n', '\r', '-', '_'],
                StringSplitOptions.RemoveEmptyEntries);

            return colombianismos.Any(g => palabras.Contains(g) ||
                                            palabras.Any(p => p.StartsWith(g)));
        }

        public string? PrimerMatch(string texto)
        {
            throw new NotImplementedException();
        }

        public string NormalizarTexto(string texto)
        {
            return texto
                .Replace("0", "o")
                .Replace("1", "i")
                .Replace("3", "e")
                .Replace("4", "a")
                .Replace("5", "s")
                .Replace("6", "g")
                .Replace("7", "t")
                .Replace("@", "a")
                .Replace("$", "s")
                .Replace("!", "i");
        }
    }
}
