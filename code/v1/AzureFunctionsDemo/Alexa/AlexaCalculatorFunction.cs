using System;
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
    public static class AlexaCalculatorFunction
    {
        [FunctionName("AlexaCalculatorFunction")]
        public static async Task<HttpResponseMessage> Run([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "Alexa/Calculator")]HttpRequestMessage req, TraceWriter log)
        {
            log.Info("AlexaCalculatorFunction - Started");

            // Get request body
            var skillRequest = await req.Content.ReadAsAsync<SkillRequest>();
            var locale = skillRequest.Request.Locale.ToLower();

            // check the request type for Launch Request
            if (skillRequest.Request.Type == "LaunchRequest")
            {
                // default launch request
                log.Info("AlexaCalculatorFunction - LaunchRequest");
                return req.CreateResponse(HttpStatusCode.OK, HelpRequest(locale));
            }

            // check the request type for Intent Request
            if (skillRequest.Request.Type == "IntentRequest")
            {
                var intent = ((IntentRequest)skillRequest.Request).Intent;

                log.Info($"AlexaCalculatorFunction - IntentRequest: {intent}");

                if (!intent.Slots.ContainsKey("firstnum") || !intent.Slots.ContainsKey("secondnum"))
                {
                    if (locale.StartsWith("de"))
                        return req.CreateResponse(HttpStatusCode.OK, CreateSkillResponse("Bitte geben Sie zwei Zahlen an, welche addiert, subtrahiert, multipliziert oder dividiert werden sollen.", "Alexa Taschenrechner", "Leider wurden keine Zahlen angegeben. Bitte noch einmal mit einer Mathe-Aufgabe probieren."));

                    return req.CreateResponse(HttpStatusCode.OK, CreateSkillResponse("Please specify two numbers to be added, subtracted, multiplied or divided.", "Alexa Calculator", "Unfortunately, no numbers were given. Please try again with a math task."));
                }


                var num1 = Convert.ToDouble(intent.Slots["firstnum"].Value);
                var num2 = Convert.ToDouble(intent.Slots["secondnum"].Value);
                double result;

                switch (intent.Name)
                {
                    case "AddIntent":
                        result = num1 + num2;

                        if (locale.StartsWith("de"))
                            return req.CreateResponse(HttpStatusCode.OK, CreateSkillResponse($"Das Ergebnis aus der Addition von {num1} und {num2} lautet: {result}.", "Alexa Taschenrechner", $"{num1} + {num2} = {result}."));

                        return req.CreateResponse(HttpStatusCode.OK, CreateSkillResponse($"The result of adding {num1} and {num2} is: {result}.", "Alexa Calculator", $"{num1} + {num2} = {result}."));
                    case "SubstractIntent":
                        result = num1 - num2;

                        if (locale.StartsWith("de"))
                            return req.CreateResponse(HttpStatusCode.OK, CreateSkillResponse($"Das Ergebnis aus der Subtraktion von {num1} und {num2} lautet: {result}.", "Alexa Taschenrechner", $"{num1} - {num2} = {result}."));

                        return req.CreateResponse(HttpStatusCode.OK, CreateSkillResponse($"The result of subtracting {num1} and {num2} is: {result}.", "Alexa Calculator", $"{num1} - {num2} = {result}."));
                    case "MultiplyIntent":
                        result = num1 * num2;

                        if (locale.StartsWith("de"))
                            return req.CreateResponse(HttpStatusCode.OK, CreateSkillResponse($"Das Ergebnis aus der Multiplikation von {num1} und {num2} lautet: {result}.", "Alexa Taschenrechner", $"{num1} * {num2} = {result}."));

                        return req.CreateResponse(HttpStatusCode.OK, CreateSkillResponse($"The result of multiplying {num1} and {num2} is: {result}.", "Alexa Calculator", $"{num1} * {num2} = {result}."));
                    case "DivideIntent":
                        if (num2 == 0)
                        {
                            if (locale.StartsWith("de"))
                                return req.CreateResponse(HttpStatusCode.OK, CreateSkillResponse("Du hast gerade versucht durch 0 zu teilen. Das klappt leider nicht. Versuche es bitte mit einer anderen Aufgaben.", "Alexa Taschenrechner", "Du hast gerade versucht durch 0 zu teilen. Das klappt leider nicht. Versuche es bitte mit einer anderen Aufgaben."));

                            return req.CreateResponse(HttpStatusCode.OK, CreateSkillResponse("You have just tried to divide by 0. This does not work. Please try with a different task.", "Alexa Calculator", "You have just tried to divide by 0. This does not work. Please try with a different task."));
                        }
                        else
                        {
                            result = num1 / num2;

                            if (locale.StartsWith("de"))
                                return req.CreateResponse(HttpStatusCode.OK, CreateSkillResponse($"Das Ergebnis aus der Division von {num1} und {num2} lautet: {result:F2}.", "Alexa Taschenrechner", $"{num1} / {num2} = {result:F2}."));

                            return req.CreateResponse(HttpStatusCode.OK, CreateSkillResponse($"The result of dividing {num1} and {num2} is: {result: F2}.", "Alexa Calculator", $"{num1} / {num2} = {result:F2}."));
                        }
                    default:
                        return req.CreateResponse(HttpStatusCode.OK, HelpRequest(locale));
                }
            }

            return req.CreateResponse(HttpStatusCode.OK, HelpRequest(locale));
        }

        private static SkillResponse HelpRequest(string locale)
        {
            if (locale.StartsWith("de"))
                return CreateSkillResponse("Willkommen zum Alexa-Taschenrechner. Ich kann zwei Zahlen addieren, subtrahieren, multiplizieren und auch dividieren. Frage mich zum Beispiel: Was ist drei plus zwei.", "Alexa Taschenrechner", "Willkommen zum Alexa-Taschenrechner. Ich kann zwei Zahlen addieren, subtrahieren, multiplizieren und auch dividieren. Frage mich zum Beispiel: 3 + 2.");

            return CreateSkillResponse("Welcome to the Alexa Calculator. I can add, subtract, multiply, and even divide two numbers. For example, what is three plus two?", "Alexa Calculator", "Welcome to the Alexa calculator. I can add, subtract, multiply, and even divide two numbers. For example, what is 3 + 2?");
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
