using System;
using Crawler.Lib.Model;


namespace Crawler.Lib.Service.Interface
{
    public interface ICrawlFarm : IObserver<Anchor>
    {
        void Run(int maxDegreeOfParallelism);
    }
}
