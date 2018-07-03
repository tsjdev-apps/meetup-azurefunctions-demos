using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Alexa.NET.Request;
using Alexa.NET.Request.Type;
using Alexa.NET.Response;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;

namespace AzureFunctionsDemo.Alexa
{
    public static class AlexaHelloNameFunction
    {
        [FunctionName("AlexaHelloNameFunction")]
        public static async Task<HttpResponseMessage> Run([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "Alexa/HelloName")]HttpRequestMessage req, TraceWriter log)
        {
            log.Info("AlexaHelloNameFunction - Started");

            // Get request body
            var skillRequest = await req.Content.ReadAsAsync<SkillRequest>();
            var locale = skillRequest.Request.Locale;

            // get name from body data
            var intentRequest = (IntentRequest)skillRequest.Request;

            if (intentRequest == null)
                return null;

            var name = intentRequest.Intent.Slots.ContainsKey("name") ? intentRequest.Intent.Slots["name"].Value : null;

            if (name == null)
            {
                log.Info("AlexaHelloNameFunction - No name detected");

                if (locale.ToLower().StartsWith("de"))
                    return req.CreateResponse(HttpStatusCode.OK, CreateSkillResponse("Ich habe leider deinen Namen nicht richtig verstanden...", "Hello Name!", "Leider wurde dein Name nicht erkannt..."));

                return req.CreateResponse(HttpStatusCode.OK, CreateSkillResponse("Unfortunately, I did not understand your name correctly...", "Hello Name!", "Unfortunately, your name was not recognized..."));
            }

            log.Info($"AlexaHelloNameFunction - Name: {name}");

            if (locale.ToLower().StartsWith("de"))
                return req.CreateResponse(HttpStatusCode.OK, CreateSkillResponse($"Wie geht es denn so, {name.ToUpper()}? Freut mich dich kennenzulernen.", "Hello Name!", $"Hallo {name.ToUpper()}!"));

            return req.CreateResponse(HttpStatusCode.OK, CreateSkillResponse($"How are you, {name.ToUpper()}? I am pleased to meet you.", "Hello Name!", $"Hello {name.ToUpper()}!"));
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
