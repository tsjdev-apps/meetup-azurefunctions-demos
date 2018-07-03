using AzureFunctionsDemo.CognitiveServices.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.ProjectOxford.Vision;
using Microsoft.ProjectOxford.Vision.Contract;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace AzureFunctionsDemo.CognitiveServices
{
    public static class CognitiveServicesImageDescriptionFunction
    {
        public static class ImageDescriptionFunction
        {
            [FunctionName("CognitiveServicesImageDescriptionFunction")]
            public static async Task<HttpResponseMessage> Run([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "CognitiveServices/ImageDescription")]HttpRequestMessage req, TraceWriter log)
            {
                try
                {
                    var data = await req.Content.ReadAsAsync<CognitiveServicesRequestItem>();

                    var url = data.Url;
                    var image = data.ImageBytes;
                    var apiKey = data.ApiKey;
                    var domainEndpoint = data.DomainEndpoint;

                    if (string.IsNullOrEmpty(apiKey) || string.IsNullOrEmpty(domainEndpoint))
                        return req.CreateResponse(HttpStatusCode.BadRequest, "Please provide an api key and a domain endpoint");

                    if (string.IsNullOrEmpty(url) && image == null)
                        return req.CreateResponse(HttpStatusCode.BadRequest, "Please provide an image or an url");

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
                    return req.CreateResponse(HttpStatusCode.OK, imageDescriptionResult);
                }
                catch (Exception e)
                {
                    return req.CreateResponse(HttpStatusCode.BadRequest, e.Message);
                }
            }
        }
    }
}
