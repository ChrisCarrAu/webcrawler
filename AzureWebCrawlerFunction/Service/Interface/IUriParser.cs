using System;
using System.Threading.Tasks;
using AzureWebCrawlerFunction.Model;

namespace AzureWebCrawlerFunction.Service.Interface
{
    public interface IUriParser
    {
        Task Crawl(Anchor uri);
    }
}
