using AzureCrawler;
using AzureFunctions.Autofac;
using AzureWebCrawlerFunction.Model;
using Newtonsoft.Json;
using AzureWebCrawlerFunction.Service.Implementation;
using AzureWebCrawlerFunction.Service.Interface;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace AzureWebCrawlerFunction
{
    public static class WebCrawlerFunction
    {
        [FunctionName("WebCrawlerFunction")]
        public static void Run(
            [QueueTrigger(ResourceNames.UriQueue, Connection = ResourceNames.ConnectionString)]
            string myQueueItem,
            ILogger log,
            
            [Inject]IUriParser uriParser)
        {
            var crawlUri = JsonConvert.DeserializeObject<Anchor>(myQueueItem);
            log.LogInformation($"Processing Uri {crawlUri.Uri.ToString()}");

            uriParser.Crawl(crawlUri);

            log.LogInformation($"C# Queue trigger function processed: {myQueueItem}");
        }
    }
}
