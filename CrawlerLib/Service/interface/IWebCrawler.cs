using System;
using System.Threading.Tasks;
using Crawler.Lib.Model;

namespace Crawler.Lib.Service.Interface
{
    public interface IWebCrawler : IObservable<Anchor>
    {
        Task Crawl(Anchor uri);
    }
}
