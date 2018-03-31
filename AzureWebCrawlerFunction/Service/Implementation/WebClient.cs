using System;
using System.Net;
using System.Threading.Tasks;
using AzureWebCrawlerFunction.Service.Interface;

namespace AzureWebCrawlerFunction.Service.Implementation
{
    public class WebClient : IWebClient
    {
        private readonly System.Net.WebClient _webClient = new System.Net.WebClient();

        public WebHeaderCollection ResponseHeaders => _webClient.ResponseHeaders;

        public Task<byte[]> DownloadDataTaskAsync(string address)
        {
            return _webClient.DownloadDataTaskAsync(address);
        }

        public Task<byte[]> DownloadDataTaskAsync(Uri address)
        {
            return _webClient.DownloadDataTaskAsync(address);
        }

        public Task<string> DownloadStringTaskAsync(Uri address)
        {
            return _webClient.DownloadStringTaskAsync(address);
        }
    }
}
