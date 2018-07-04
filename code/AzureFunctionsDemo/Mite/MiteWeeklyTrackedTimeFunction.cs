using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using AzureFunctionsDemo.Mite.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.WindowsAzure.Storage.Blob;
using Newtonsoft.Json;
using Tweetinvi;

namespace AzureFunctionsDemo.Mite
{
    public static class MiteWeeklyTrackedTimeFunction
    {
        [FunctionName("MiteWeeklyTrackedTimeFunction")]
        public static async Task Run([TimerTrigger("0 0 0 * * 6", RunOnStartup = true)]TimerInfo myTimer, [Blob("cache/timeentries.json")]CloudBlockBlob blob, TraceWriter log)
        {
            var baseUrl = "https://{0}.mite.yo.lk/time_entries.json?group_by=day&api_key={1}&at=this_week&user_id=current";

            var twitterConsumerKey = GetEnvironmentVariable("twitterConsumerKey");
            var twitterConsumerSecret = GetEnvironmentVariable("twitterConsumerSecret");
            var twitterAccessToken = GetEnvironmentVariable("twitterAccessToken");
            var twitterAccessTokenSecret = GetEnvironmentVariable("twitterAccessTokenSecret");

            var miteTenant = GetEnvironmentVariable("miteTenant");
            var miteApiKey = GetEnvironmentVariable("miteApiKey");

            var requestUrl = string.Format(baseUrl, miteTenant, miteApiKey);

            var httpClient = new HttpClient();
            var response = await httpClient.GetStringAsync(requestUrl);

            var timeEntries = (JsonConvert.DeserializeObject<IEnumerable<TimeEntryGroupWrapper>>(response)).Select(wrapper => wrapper.TimeEntryGroup);

            var outputJsonString = JsonConvert.SerializeObject(timeEntries);
            blob.Properties.ContentType = "application/json";
            await blob.UploadTextAsync(outputJsonString);

            var totalMinutes = timeEntries.Sum(t => t.Minutes);
            var timeSpan = TimeSpan.FromMinutes(totalMinutes);
            var twitterString = $"This week I worked {totalMinutes} minutes, which means {timeSpan.Days}d, {timeSpan.Hours}h and {timeSpan.Minutes}m.";

            Auth.SetUserCredentials(twitterConsumerKey, twitterConsumerSecret, twitterAccessToken, twitterAccessTokenSecret);
            Tweet.PublishTweet(twitterString);
        }

        private static string GetEnvironmentVariable(string name)
        {
            return Environment.GetEnvironmentVariable(name, EnvironmentVariableTarget.Process);
        }
    }
}
