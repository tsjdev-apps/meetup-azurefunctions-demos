using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using AzureFunctionsDemo.CognitiveServices.Models;
using Microsoft.ProjectOxford.Vision;
using Microsoft.ProjectOxford.Vision.Contract;

namespace AzureFunctionsDemo.CognitiveServices
{
    public static class CognitiveServicesImageDescriptionFunction
    {
        [FunctionName("CognitiveServicesImageDescriptionFunction")]
        public static async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "cognitiveservices/imagedescription")]HttpRequest req, ILogger log)
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
                var service = new VisionServiceClient(apiKey, $"https://{domainEndpoint}.api.cognitive.microsoft.com/vision/v1.0");
                var visualFeatures = new[] { VisualFeature.Description };

                AnalysisResult result = null;

                if (string.IsNullOrEmpty(url))
                    result = await service.AnalyzeImageAsync(new MemoryStream(image), visualFeatures);
                else
                    result = await service.AnalyzeImageAsync(url, visualFeatures);

                var imageDescriptionResult = result?.Description?.Captions;

                // send the result back
                return new OkObjectResult(imageDescriptionResult);
            }
            catch (Exception e)
            {
                return new BadRequestObjectResult(e.Message);
            }
        }
    }
}
