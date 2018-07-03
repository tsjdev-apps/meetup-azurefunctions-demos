using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using AzureFunctionsDemo.CognitiveServices.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.ProjectOxford.Face;
using Microsoft.ProjectOxford.Face.Contract;

namespace AzureFunctionsDemo.CognitiveServices
{
    public static class CognitiveServicesEmotionFunction
    {
        [FunctionName("CognitiveServicesEmotionFunction")]
        public static async Task<HttpResponseMessage> Run([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "CognitiveServices/Emotion")]HttpRequestMessage req, TraceWriter log)
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
                var service = new FaceServiceClient(apiKey, $"https://{domainEndpoint}.api.cognitive.microsoft.com/face/v1.0");
                var faceAttributes = new[] { FaceAttributeType.Emotion };

                Face[] result = null;

                if (string.IsNullOrEmpty(url))
                    result = await service.DetectAsync(new MemoryStream(image), true, false, faceAttributes);
                else
                    result = await service.DetectAsync(url, true, false, faceAttributes);

                var emotionResult = result?.Select(r => new { r.FaceId, r.FaceRectangle, r.FaceAttributes.Emotion });

                // send the result back
                return req.CreateResponse(HttpStatusCode.OK, emotionResult);
            }
            catch (Exception e)
            {
                return req.CreateResponse(HttpStatusCode.BadRequest, e.Message);
            }
        }
    }
}
