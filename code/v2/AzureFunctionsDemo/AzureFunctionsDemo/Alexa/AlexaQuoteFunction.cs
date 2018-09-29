using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Net.Http;
using Alexa.NET.Response;
using Alexa.NET.Request;
using Alexa.NET.Request.Type;

namespace AzureFunctionsDemo.Alexa
{

    public static class AlexaQuoteFunction
    {
        static class Statics
        {
            public static string QuoteUrl = "https://api.forismatic.com/api/1.0/?method=getQuote&format=json&lang=en";
        }

        class Quote
        {
            [JsonProperty("quoteText")]
            public string QuoteText { get; set; }

            [JsonProperty("quoteAuthor")]
            public string QuoteAuthor { get; set; }
        }

        [FunctionName("AlexaQuoteFunction")]
        public static async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "alexa/quote")]HttpRequest req, ILogger log)
        {
            log.LogInformation("AlexaQuoteFunction - Started");

            // Get request body
            var content = await req.ReadAsStringAsync();

            // Deserialize object to SkillRequest
            var skillRequest = JsonConvert.DeserializeObject<SkillRequest>(content);

            // Check for launchRequest
            if (skillRequest.Request is LaunchRequest)
                return new OkObjectResult(CreateSkillResponse("Welcome to Random Quote! Everytime you start me and ask for a random quote, I will give it to you..", "Hello Name", "Welcome to Hello Name!", false));

            // Check for IntentRequest
            if (skillRequest.Request is IntentRequest intentRequest)
            {
                switch (intentRequest.Intent.Name)
                {
                    case "Amazon.StopIntent":
                    case "Amazon.CancelIntent":
                        return new OkObjectResult(CreateSkillResponse("Ok", "Random Quote", "Till next time.", true));
                    case "Amazon.HelpIntent":
                        return new OkObjectResult(CreateSkillResponse("Everytime you ask for a random quote, I will tell you one.", "Random Quote", "Everytime you ask for a random quote, I will tell you one.", false));
                    case "RandomQuoteIntent":
                        var quoteString = await new HttpClient().GetStringAsync(Statics.QuoteUrl);
                        var quote = JsonConvert.DeserializeObject<Quote>(quoteString);
                        return new OkObjectResult(CreateSkillResponse($"{quote?.QuoteText?.Trim()} - {quote?.QuoteAuthor}", "Random Quote", $"{quote?.QuoteText?.Trim()} - {quote?.QuoteAuthor}", true));
                }
            }

            return new OkObjectResult(CreateSkillResponse("Something went wrong... Please try again.", "Random Quote", "Something went wrong..."));
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
