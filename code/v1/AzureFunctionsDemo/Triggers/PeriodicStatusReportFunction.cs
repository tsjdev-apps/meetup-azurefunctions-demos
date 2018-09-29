using System;
using System.Net.Mail;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;

namespace AzureFunctionsDemo.Triggers
{
    public static class PeriodicStatusReportFunction
    {
        [FunctionName("PeriodicStatusReportFunction")]
        public static void Run([TimerTrigger("0 */1 * * * *", RunOnStartup = true)]TimerInfo myTimer, TraceWriter log)
        {
            var fromEmail = GetEnvironmentVariable("statusReportFromMail");
            var toEmail = GetEnvironmentVariable("statusReportToMail");
            var mailPassword = GetEnvironmentVariable("statusReportMailPassword");
            var smtpHost = GetEnvironmentVariable("statusReportHost");
            var smtpPort = GetEnvironmentVariable("statusReportPort");

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

            try
            {
                client.Send(mail);
            }
            catch (Exception ex)
            {
                log.Error("StatusReportFunction - Error", ex);
            }
        }

        private static string GetEnvironmentVariable(string name)
        {
            return Environment.GetEnvironmentVariable(name, EnvironmentVariableTarget.Process);
        }
    }
}
