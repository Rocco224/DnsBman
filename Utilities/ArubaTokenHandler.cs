using DnsBman.Models;
using System.Text;
using System.Text.Json;

namespace DnsBman.Utilities
{
    public class ArubaTokenHandler
    {
        private static readonly HttpClient _httpClient = new HttpClient();

        public static async Task<ArubaToken> GetToken()
        {
            try
            {
                HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Post, "https://api.arubabusiness.it/auth/token");

                // request header
                requestMessage.Headers.Add("Authorization-Key", ConfigurationHandler.GetArubaAuthKey());
                // request body  
                string requestBody = ConfigurationHandler.GetArubaCredentials();
                
                requestMessage.Content = new StringContent(requestBody, Encoding.UTF8, "application/x-www-form-urlencoded");

                using HttpResponseMessage responseMessage = await _httpClient.SendAsync(requestMessage);

                if (responseMessage.IsSuccessStatusCode)
                {
                    string responseContent = await responseMessage.Content.ReadAsStringAsync();
                    var token = JsonSerializer.Deserialize<ArubaToken>(responseContent);

                    if(token != null)
                    {
                        return token;
                    }

                    throw new Exception($"Error: {responseMessage.StatusCode} {responseContent}");
                }
                else
                {
                    string responseContent = await responseMessage.Content.ReadAsStringAsync();
                    throw new Exception($"Error: {responseMessage.StatusCode} {responseContent}");
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error: {ex.Message}");
            }
        }

        public static bool IsExpiring(ArubaToken token)
        {
            try
            {
                DateTime now = DateTime.Now;
                DateTime expiresIn15Minutes = now.AddMinutes(15);

                return token.Expires < expiresIn15Minutes || token.Expires <= now;
            }
            catch (Exception ex)
            {
                throw new Exception("Si è verificato un'eccezione durante il controllo del token di scadenza.", ex);
            }
        }
    }
}
