using System;
using System.Threading.Tasks;
using Crawler.Lib.Model;

namespace Crawler.Lib.Service.Interface
{
    public interface ICrawlFarm : IObserver<Anchor>
    {
        Task Run(int maxDegreeOfParallelism);
    }
}
