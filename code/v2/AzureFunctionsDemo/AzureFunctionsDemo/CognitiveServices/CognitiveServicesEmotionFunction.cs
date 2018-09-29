using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.ProjectOxford.Face;
using Microsoft.ProjectOxford.Face.Contract;
using System.Linq;
using AzureFunctionsDemo.CognitiveServices.Models;

namespace AzureFunctionsDemo.CognitiveServices
{
    public static class CognitiveServicesEmotionFunction
    {
        [FunctionName("CognitiveServicesEmotionFunction")]
        public static async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "cognitiveservices/emotion")]HttpRequest req, ILogger log)
        {
            try
            {
                var body = await req.ReadAsStringAsync();
                var cognitiveServicesRequestItem = JsonConvert.DeserializeObject<CognitiveServicesRequestItem>(body);

                var url = cognitiveServicesRequestItem.Url;
                var image = cognitiveServicesRequestItem.ImageBytes;
                var apiKey = cognitiveServicesRequestItem.ApiKey;
                var domainEndpoint = cognitiveServicesRequestItem.DomainEndpoint;

                if (string.IsNullOrEmpty(apiKey) || string.IsNullOrEmpty(domainEndpoint))
                    return new BadRequestObjectResult("Please provide an api key and a domain endpoint");

                if (string.IsNullOrEmpty(url) && image == null)
                    return new BadRequestObjectResult("Please provide an image or an url");

                // analyze image from url with the provided apikey
                var service = new FaceServiceClient(apiKey, $"https://{domainEndpoint}.api.cognitive.microsoft.com/face/v1.0");
                var faceAttributes = new[] { FaceAttributeType.Emotion };

                Face[] result = null;

                if (string.IsNullOrEmpty(url))
                    result = await service.DetectAsync(new MemoryStream(image), true, false, faceAttributes);
                else
                    result = await service.DetectAsync(url, true, false, faceAttributes);

                var emotionResult = result?.Select(r => new { r.FaceId, r.FaceRectangle, r.FaceAttributes.Emotion });

                // send the result back
                return new OkObjectResult(emotionResult);
            }
            catch (Exception e)
            {
                return new BadRequestObjectResult(e.Message);
            }
        }
    }
}
