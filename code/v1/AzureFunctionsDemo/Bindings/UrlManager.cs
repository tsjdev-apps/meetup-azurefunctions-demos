using AzureFunctionsDemo.Bindings.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace AzureFunctionsDemo.Bindings
{
    public static class UrlManager
    {
        [FunctionName("UrlManagerAddUrlFunction")]
        public static async Task<HttpResponseMessage> AddUrl([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "Bindings/UrlManager/Add")]HttpRequestMessage req, [Table("urls", "1", "KEY", Take = 1)]UrlKey keyTable, [Table("urls")]CloudTable outTable, TraceWriter log)
        {
            log.Info("UrlManagerAddUrlFunction - Started");

            // parse query parameter
            var url = req.GetQueryNameValuePairs().FirstOrDefault(q => string.Compare(q.Key, "url", StringComparison.OrdinalIgnoreCase) == 0).Value;

            if (string.IsNullOrEmpty(url))
                return req.CreateResponse(HttpStatusCode.BadRequest, "Please provide an URL as parameter within the request");

            if (keyTable == null)
            {
                keyTable = new UrlKey { PartitionKey = "1", RowKey = "KEY", Id = 1024 };
                var addKey = TableOperation.Insert(keyTable);
                await outTable.ExecuteAsync(addKey);
            }

            var idx = keyTable.Id;
            const string alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            var s = string.Empty;

            while (idx > 0)
            {
                s += alphabet[idx % alphabet.Length];
                idx /= alphabet.Length;
            }

            var code = string.Join(string.Empty, s.Reverse());

            log.Info($"UrlManagerAddUrlFunction - Code: {code}");

            var urlData = new UrlData { PartitionKey = $"{code[0]}", RowKey = code, Url = url };

            keyTable.Id += 10;

            var operation = TableOperation.Replace(keyTable);
            await outTable.ExecuteAsync(operation);
            operation = TableOperation.Insert(urlData);
            await outTable.ExecuteAsync(operation);

            return req.CreateResponse(HttpStatusCode.OK, urlData.RowKey);
        }

        [FunctionName("UrlManagerGoToUrlFunction")]
        public static async Task<HttpResponseMessage> RedirectToUrl([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "Bindings/UrlManager/Go/{shortUrl}")]HttpRequestMessage req, [Table("urls")]CloudTable inputTable, string shortUrl, TraceWriter log)
        {
            log.Info("UrlManagerGoToUrlFunction - Started");

            if (string.IsNullOrWhiteSpace(shortUrl))
                return req.CreateResponse(HttpStatusCode.BadRequest, "Please provide an shorted URL.");

            shortUrl = shortUrl.ToUpper();

            var operation = TableOperation.Retrieve<UrlData>(shortUrl[0].ToString(), shortUrl);
            var result = await inputTable.ExecuteAsync(operation);

            var url = "http://www.medialesson.de";

            if (result != null && result.Result is UrlData data)
                url = data.Url;

            log.Info($"UrlManagerGoToUrlFunction - Url: {url}");

            var response = req.CreateResponse(HttpStatusCode.Redirect);
            response.Headers.Add("Location", url);

            return response;
        }
    }
}
