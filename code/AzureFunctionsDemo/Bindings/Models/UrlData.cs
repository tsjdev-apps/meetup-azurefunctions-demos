using Microsoft.WindowsAzure.Storage.Table;

namespace AzureFunctionsDemo.Bindings.Models
{
    public class UrlData : TableEntity
    {
        public string Url { get; set; }
    }
}
