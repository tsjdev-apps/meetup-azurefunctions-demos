using Newtonsoft.Json;

namespace AzureFunctionsDemo.Mite.Models
{
    public class ProjectEntry
    {
        [JsonProperty("budget")]
        public int Budget { get; set; }

        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }
        
        public int ConsumedBudget { get; set; }
    }
}
