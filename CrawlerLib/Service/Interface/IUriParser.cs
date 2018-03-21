using System;
using System.Threading.Tasks;
using Crawler.Lib.Model;

namespace Crawler.Lib.Service.Interface
{
    public interface IUriParser : IObservable<Anchor>
    {
        Task Crawl(Anchor uri);
    }
}
