using System.Text;
using System.Text.Json;

namespace DnsBman.Services
{
    public class RecordClientService
    {
        private readonly HttpClient _httpClient;

        public RecordClientService(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("ArubaApi");
        }

        public async Task<HttpResponseMessage> PostRecord(int domainId, string name, string value)
        {
            try
            {
                using StringContent recordModel = new(
                    JsonSerializer.Serialize(new
                    {
                        IdDomain = domainId,
                        Type = 2,
                        Name = name,
                        Content = value
                    }),
                Encoding.UTF8,
                "application/json");

                HttpResponseMessage responceMessage = await _httpClient.PostAsync(
                    "api/domains/dns/record",
                    recordModel);

                return responceMessage;
            }
            catch (Exception ex)
            {
                throw new Exception($"{ex.Message}");
            }
        }

        public async Task<HttpResponseMessage> PutRecord(int recordId, string newName, string newValue)
        {
            try
            {
                using StringContent recordModel = new(
                    JsonSerializer.Serialize(new
                    {
                        IdRecord = recordId,
                        Name = newName,
                        Content = newValue
                    }),
                Encoding.UTF8,
                "application/json");

                HttpResponseMessage responceMessage = await _httpClient.PutAsync(
                    "api/domains/dns/record",
                    recordModel);

                return responceMessage;
            }
            catch (Exception ex)
            {
                throw new Exception($"{ex.Message}");
            }
        }

        public async Task<HttpResponseMessage> DeleteRecord(int idRecord)
        {
            try
            {
                HttpResponseMessage responceMessage = await _httpClient.DeleteAsync($"api/domains/dns/record/{idRecord}");

                return responceMessage;
            }
            catch (Exception ex)
            {
                throw new Exception($"{ex.Message}");
            }
        }
    }
}
