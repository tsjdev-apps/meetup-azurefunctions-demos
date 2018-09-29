using System;
using System.Net.Mail;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace AzureFunctionsDemo.Triggers
{
    public static class PeriodicStatusReportFunction
    {
        [FunctionName("PeriodicStatusReportFunction")]
        public static void Run([TimerTrigger("0 */1 * * * *")]TimerInfo myTimer, ExecutionContext context, ILogger log)
        {
            try
            {
                var config = new ConfigurationBuilder()
                .SetBasePath(context.FunctionAppDirectory)
                .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();

                var fromEmail = config["statusReportFromMail"];
                var toEmail = config["statusReportToMail"];
                var mailPassword = config["statusReportMailPassword"];
                var smtpHost = config["statusReportHost"];
                var smtpPort = config["statusReportPort"];

                var mail = new MailMessage(fromEmail, toEmail);

                var client = new SmtpClient
                {
                    Port = Convert.ToInt32(smtpPort),
                    EnableSsl = true,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Host = smtpHost,
                    Credentials = new System.Net.NetworkCredential(fromEmail, mailPassword)
                };

                mail.Subject = "Status Report";
                mail.Body = $"Current Time on Server: {DateTime.Now.ToLocalTime()}";


                client.Send(mail);
            }
            catch (Exception ex)
            {
                log.LogError("PeriodicStatusReportFunction - Error", ex);
            }
        }
    }
}
