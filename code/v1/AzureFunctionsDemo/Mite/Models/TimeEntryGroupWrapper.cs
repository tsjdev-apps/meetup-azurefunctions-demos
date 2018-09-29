using Newtonsoft.Json;

namespace AzureFunctionsDemo.Mite.Models
{
    public class TimeEntryGroupWrapper
    {
        [JsonProperty("time_entry_group")]
        public TimeEntryGroup TimeEntryGroup { get; set; }
    }
}
