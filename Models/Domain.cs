using System.Text.Json.Serialization;

namespace DnsBman.Models
{
    public class Domain
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Master {  get; set; }

        [JsonPropertyName("Last_check")]
        public string LastCheck { get; set; }
        public string Type { get; set; }

        [JsonPropertyName("Notified_Serial")]
        public string NotifiedSerial { get; set; }
        public string Account {  get; set; }
        public string Soa { get; set; }
        public bool Attivo { get; set; }
        public bool Locked { get; set; }
        public int Ttl { get; set; }
        public List<Record> Records { get; set; }
    }
}
