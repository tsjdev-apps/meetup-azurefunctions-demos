using Newtonsoft.Json;

namespace AzureFunctionsDemo.CognitiveServices.Models
{
    public class CognitiveServicesRequestItem
    {
        [JsonProperty("apiKey")]
        public string ApiKey { get; set; }

        [JsonProperty("domainEndpoint")]
        public string DomainEndpoint { get; set; }

        [JsonProperty("url")]
        public string Url { get; set; }

        [JsonProperty("imageBytes")]
        public byte[] ImageBytes { get; set; }
    }
}
