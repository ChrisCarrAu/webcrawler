using System;
using WebCrawler.Model;

namespace WebCrawler.Service.Interface
{
    interface IWebCrawler
    {
        void Crawl(Anchor uri);

    }
}
