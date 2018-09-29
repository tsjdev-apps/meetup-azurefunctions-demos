using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Alexa.NET.Request;
using Alexa.NET.Request.Type;
using Alexa.NET.Response;

namespace AzureFunctionsDemo.Alexa
{
    public static class AlexaHelloNameFunction
    {
        [FunctionName("AlexaHelloNameFunction")]
        public static async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "alexa/helloname")]HttpRequest req, ILogger log)
        {
            log.LogInformation("AlexaHelloNameFunction - Started");

            // Get request body
            var content = await req.ReadAsStringAsync();

            // Deserialize object to SkillRequest
            var skillRequest = JsonConvert.DeserializeObject<SkillRequest>(content);

            // Check for launchRequest
            if (skillRequest.Request is LaunchRequest)
                return new OkObjectResult(CreateSkillResponse("Welcome to Hello Name! Just give me the name of the person I should welcome today.", "Hello Name", "Welcome to Hello Name!", false));

            // get name from body data
            var intentRequest = (IntentRequest)skillRequest.Request;
            var name = intentRequest.Intent.Slots.ContainsKey("name") ? intentRequest.Intent.Slots["name"].Value : null;

            if (name == null)
            {
                log.LogInformation("AlexaHelloNameFunction - No name detected");
                return new OkObjectResult(CreateSkillResponse("Unfortunately, I did not understand your name correctly...", "Hello Name!", "Unfortunately, your name was not recognized..."));
            }

            log.LogInformation($"AlexaHelloNameFunction - Name: {name}");
            return new OkObjectResult(CreateSkillResponse($"How are you, {name.ToUpper()}? I am pleased to meet you.", "Hello Name!", $"Hello {name.ToUpper()}!"));
        }

        private static SkillResponse CreateSkillResponse(string outputSpeech, string cardTitle, string cardContent, bool shouldEndSession = true)
        {
            var response = new SkillResponse
            {
                Version = "1.0",
                Response = new ResponseBody
                {
                    OutputSpeech = new PlainTextOutputSpeech { Text = outputSpeech },
                    Card = new SimpleCard { Title = cardTitle, Content = cardContent },
                    ShouldEndSession = shouldEndSession
                }
            };

            return response;
        }
    }
}
