using System;
using WebCrawler.Model;


namespace WebCrawler.Service.Interface
{
    interface ICrawlFarm : IObserver<Anchor>
    {
        void Run();
    }
}
