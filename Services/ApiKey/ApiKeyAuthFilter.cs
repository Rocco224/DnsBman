using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;

namespace DnsBman.Services.ApiKey
{
    public class ApiKeyAuthFilter : IAuthorizationFilter
    {
        private readonly IApiKeyValidation _apiKeyValidation;

        public ApiKeyAuthFilter(IApiKeyValidation apiKeyValidation)
        {
            _apiKeyValidation = apiKeyValidation;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            try
            {
                string userApiKey = context.HttpContext.Request.Headers[Constants.ApiKeyHeaderName].ToString();

                if (string.IsNullOrWhiteSpace(userApiKey))
                {
                    context.Result = new BadRequestResult();
                    return;
                }

                if (!_apiKeyValidation.IsValidApiKey(userApiKey))
                    context.Result = new UnauthorizedResult();
            }
            catch (Exception ex)
            {
                throw new Exception("Si è verificato un errore durante l'autorizzazione basata sulla chiave API.", ex);
            }
        }
    }
}
