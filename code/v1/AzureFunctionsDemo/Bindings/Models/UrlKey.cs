using Microsoft.WindowsAzure.Storage.Table;

namespace AzureFunctionsDemo.Bindings.Models
{
    public class UrlKey : TableEntity
    {
        public int Id { get; set; }
    }
}
