using System.Text.Json.Serialization;

namespace DnsBman.Models
{
    public class ArubaToken
    {
        [JsonPropertyName("access_token")] 
        public string AccessToken { get; set; }

        [JsonPropertyName("token_type")]
        public string TokenType { get; set; }

        [JsonPropertyName("expires_in")]
        public int ExpiresIn { get; set; }

        [JsonPropertyName("username")]
        public string Username { get; set; }

        [JsonPropertyName("firstname")]
        public string Firstname { get; set; }

        [JsonPropertyName("lastname")]
        public string Lastname { get; set; }

        [JsonPropertyName("organization")]
        public string Organization { get; set; }
        public DateTime Issued { get; set; } = DateTime.Now;
        public DateTime Expires { get; set; } = DateTime.Now.AddDays(1);

        public ArubaToken() { }
    }
}
