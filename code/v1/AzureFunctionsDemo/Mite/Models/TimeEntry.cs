using Newtonsoft.Json;
using System;

namespace AzureFunctionsDemo.Mite.Models
{
    public class TimeEntry
    {
        [JsonProperty("project_id")]
        public int ProjectId { get; set; }

        [JsonProperty("minutes")]
        public int Minutes { get; set; }
    }
}
