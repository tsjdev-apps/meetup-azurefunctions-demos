using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Alexa.NET.Request;
using Alexa.NET.Response;

namespace AzureFunctionsDemo.Alexa
{
    public static class AlexaHelloWorldFunction
    {
        [FunctionName("AlexaHelloWorldFunction")]
        public static async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "alexa/helloworld")]HttpRequest req, ILogger log)
        {
            log.LogInformation("AlexaHelloWorldFunction - Started");

            // Get request body
            var content = await req.ReadAsStringAsync();

            // Deserialize object to SkillRequest
            var skillRequest = JsonConvert.DeserializeObject<SkillRequest>(content);

            // Return SkillResponse
            return new OkObjectResult(
                new SkillResponse
                {                    
                    Version = "1.0",
                    Response = new ResponseBody
                    {
                        OutputSpeech = new PlainTextOutputSpeech { Text = "Hello World from an Azure Function!" },
                        Card = new SimpleCard { Title = "Hello World", Content = "Hello World form an Azure Function!" },
                        ShouldEndSession = true
                    }
                });
        }
    }
}
