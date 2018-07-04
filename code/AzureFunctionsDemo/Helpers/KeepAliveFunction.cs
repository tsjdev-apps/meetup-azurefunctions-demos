using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;

namespace AzureFunctionsDemo.Helpers
{
    public static class KeepAliveFunction
    {
        [FunctionName("KeepAliveFunction")]
        public static void Run([TimerTrigger("0 */9 * * * *")]TimerInfo myTimer, TraceWriter log)
        {
            log.Info($"Keep Alive Function executed at: {DateTime.Now}");
        }
    }
}
