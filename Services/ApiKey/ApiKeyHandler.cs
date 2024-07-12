using DnsBman.Data;
using DnsBman.Models;
using DnsBman.Models.IdentityModels;
using Microsoft.AspNetCore.Identity;
using System.Security.Cryptography;

namespace DnsBman.Services.ApiKey
{
    public class ApiKeyHandler
    {
        private readonly DnsBmanContext _context;

        public ApiKeyHandler(DnsBmanContext context)
        {
            _context = context;
        }

        public string GenerateKey()
        {
            try
            {
                var bytes = new byte[64];
                using (var rng = RandomNumberGenerator.Create())
                {
                    rng.GetBytes(bytes);
                }
                return Convert.ToBase64String(bytes);
            }
            catch (Exception ex)
            {
                throw new Exception("Errore durante la generazione della chiave API.", ex);
            }
        }

        public async Task AssignApiKeyToUser(ApplicationUser user)
        {
            try
            {
                UsersApiKey apiKey = new();

                apiKey.Value = GenerateKey();
                apiKey.IdUser = user.Id;
                apiKey.IdUserNavigation = user;

                await _context.UsersApiKeys.AddAsync(apiKey);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Errore durante l'assegnazione della chiave API all'utente {user.Id}.", ex);
            }
        }
    }
}
