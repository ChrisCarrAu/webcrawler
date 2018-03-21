using System;
using System.Threading.Tasks;
using Crawler.Lib.Model;

namespace Crawler.Lib.Service.Interface
{
    public interface IQueueSpider : IObserver<Anchor>
    {
        Task Crawl(int maxDegreeOfParallelism);
    }
}
