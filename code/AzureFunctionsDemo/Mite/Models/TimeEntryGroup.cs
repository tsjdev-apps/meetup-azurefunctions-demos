using Newtonsoft.Json;
using System;

namespace AzureFunctionsDemo.Mite.Models
{
    public class TimeEntryGroup
    {
        [JsonProperty("minutes")]
        public int Minutes { get; set; }

        [JsonProperty("day")]
        public DateTime Date { get; set; }
    }
}
