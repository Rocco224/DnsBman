using System.Text.Json.Serialization;

namespace DnsBman.Models
{
    public class Record
    {
        public int Id { get; set; }
        public int DomainId { get; set; }
        public string Name { get; set; }
        public int Type { get; set; }
        public string Content { get; set; }
        public int Ttl { get; set; }
        public string Prio { get; set; }

        [JsonPropertyName("Change_date")]
        public string ChangeDate { get; set; }
        public bool Disabled { get; set; }
        public string OrderName { get; set; }
        public bool Auth {  get; set; }
    }
}
