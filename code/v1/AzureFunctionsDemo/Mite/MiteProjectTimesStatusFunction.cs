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

namespace AzureFunctionsDemo.Mite
{
    public static class MiteProjectTimesStatusFunction
    {
        [FunctionName("MiteProjectTimesStatusFunction")]
        public static async Task Run([TimerTrigger("0 0 0 * * 6", RunOnStartup = true)]TimerInfo myTimer, [Blob("cache/projects.json")]CloudBlockBlob blob, TraceWriter log)
        {
            var baseUrl = "https://{0}.mite.yo.lk/{1}";
            var projectsRequestUrl = "projects.json?api_key={0}";

            var miteTenant = GetEnvironmentVariable("miteTenant");
            var miteApiKey = GetEnvironmentVariable("miteApiKey");

            var requestUrl = string.Format(baseUrl, miteTenant, string.Format(projectsRequestUrl, miteApiKey));

            var httpClient = new HttpClient();
            var response = await httpClient.GetStringAsync(requestUrl);

            var projects = (JsonConvert.DeserializeObject<IEnumerable<ProjectEntryWrapper>>(response)).Select(wrapper => wrapper.ProjectEntry);

            var entryTasks = new List<Task<IEnumerable<TimeEntry>>>();

            foreach (var project in projects)
                entryTasks.Add(GetTimeEntriesForProject(httpClient, miteTenant, miteApiKey, baseUrl, project.Id));

            await Task.WhenAll(entryTasks);

            foreach (var task in entryTasks)
            {
                var project = projects.SingleOrDefault(p => p.Id == task.Result.FirstOrDefault()?.ProjectId);
                if (project != null)
                    project.ConsumedBudget = task.Result.Sum(e => e.Minutes);
            }

            var outputJsonString = JsonConvert.SerializeObject(projects);
            blob.Properties.ContentType = "application/json";

            await blob.UploadTextAsync(outputJsonString);
        }


        private static async Task<IEnumerable<TimeEntry>> GetTimeEntriesForProject(HttpClient httpClient, string tenant, string apiKey, string baseUrl, int projectId)
        {
            var requestUrl = string.Format(baseUrl, tenant, $"time_entries.json?project_id={projectId}&api_key={apiKey}");
            var response = await httpClient.GetStringAsync(requestUrl);

            return (JsonConvert.DeserializeObject<IEnumerable<TimeEntryWrapper>>(response)).Select(wrapper => wrapper.TimeEntry);
        }

        private static string GetEnvironmentVariable(string name)
        {
            return Environment.GetEnvironmentVariable(name, EnvironmentVariableTarget.Process);
        }
    }
}
