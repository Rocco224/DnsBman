using DnsBman.Models;
using System.Text.Json;

namespace DnsBman.Services
{
    public class DomainService
    {
        private readonly HttpClient _httpClient;

        public DomainService(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("ArubaApi");
        }

        public async Task<Domain> GetDomain(string serviceName)
        {
            try
            {
                HttpResponseMessage responceMessage = await _httpClient.GetAsync($"api/domains/dns/{serviceName}/details");

                if (responceMessage.IsSuccessStatusCode)
                {
                    string responceContent = await responceMessage.Content.ReadAsStringAsync();
                    var domain = JsonSerializer.Deserialize<Domain>(responceContent);

                    if (domain != null)
                    {
                        return domain;
                    }

                    throw new Exception("Responce's content not deserialized correctly");
                }

                throw new Exception($"{await responceMessage.Content.ReadAsStringAsync()}");
            }
            catch (Exception ex)
            {
                throw new Exception($"{ex.Message}");
            }
        }

        public bool IsSubdomainAvailable(List<Domain> domains, string subdomain) // Da modificare per guardare nel mio db?
        {
            try
            {
                foreach (var domain in domains)
                {
                    var cnameRecords = domain.Records.FindAll(record => record.Type == 2);

                    foreach (var record in cnameRecords)
                    {
                        int dotIndex = record.Name.IndexOf('.');
                        string recordName = record.Name.Substring(0, dotIndex);

                        if (recordName == subdomain)
                        {
                            return false;
                        }
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error: {ex.Message}");
            }
        }
    }
}
