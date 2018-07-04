using Newtonsoft.Json;

namespace AzureFunctionsDemo.Mite.Models
{
    public class ProjectEntryWrapper
    {
        [JsonProperty("project")]
        public ProjectEntry ProjectEntry { get; set; }
    }
}
