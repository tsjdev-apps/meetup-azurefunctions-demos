using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Alexa.NET.Request;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;

namespace AzureFunctionsDemo.Alexa
{
    public static class AlexaHelloWorldFunction
    {
        [FunctionName("AlexaHelloWorldFunction")]
        public static async Task<HttpResponseMessage> Run([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "Alexa/HelloWorld")]HttpRequestMessage req, TraceWriter log)
        {
            log.Info("AlexaHelloWorldFunction - Started.");

            // Get request body
            var skillRequest = await req.Content.ReadAsAsync<SkillRequest>();

            if (skillRequest.Request.Locale.ToLower().StartsWith("de"))
            {
                return req.CreateResponse(HttpStatusCode.OK, new
                {
                    version = "1.0",
                    response = new
                    {
                        outputSpeech = new
                        {
                            type = "PlainText",
                            text = "Hallo Welt aus einer Azure Function!"
                        },
                        card = new
                        {
                            type = "Simple",
                            title = "Hello World",
                            content = "Hallo Welt aus einer Azure Function."
                        },
                        shouldEndSession = true
                    }
                });
            }

            return req.CreateResponse(HttpStatusCode.OK, new
            {
                version = "1.0",
                response = new
                {
                    outputSpeech = new
                    {
                        type = "PlainText",
                        text = "Hello World from an Azure Function!"
                    },
                    card = new
                    {
                        type = "Simple",
                        title = "Hello World",
                        content = "Hello World from Azure Functions."
                    },
                    shouldEndSession = true
                }
            });
        }
    }
}
