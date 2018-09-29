using Newtonsoft.Json;

namespace AzureFunctionsDemo.Mite.Models
{
    public class TimeEntryWrapper
    {
        [JsonProperty("time_entry")]
        public TimeEntry TimeEntry { get; set; }
    }
}
