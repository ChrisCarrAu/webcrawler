using System;
using Crawler.Lib.Model;

namespace Crawler.Lib.Service.Interface
{
    public interface IWebCrawler : IObservable<Anchor>
    {
        void Crawl(Anchor uri);
    }
}
