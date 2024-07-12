using DnsBman.Data;
using DnsBman.Models;

namespace DnsBman.Services.ApiKey
{
    public class ApiKeyValidation : IApiKeyValidation
    {
        private readonly DnsBmanContext _context;
        public ApiKeyValidation(DnsBmanContext context)
        {
            _context = context;
        }
        public bool IsValidApiKey(string userApiKey)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(userApiKey))
                    return false;
                UsersApiKey? apiKey = _context.UsersApiKeys.FirstOrDefault(k => k.Value == userApiKey);
                if (apiKey == null)
                    return false;
                return true;
            }
            catch (Exception)
            {
                throw new Exception("Errore durante la validazione della chiave API.");
            }
        }
    }
}
