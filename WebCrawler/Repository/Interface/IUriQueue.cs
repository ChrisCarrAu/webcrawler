using System;
using WebCrawler.Model;

namespace WebCrawler.Repository.Interface
{
    internal interface IUriQueue : IObservable<Anchor>
    {
        void Enqueue(Anchor uri);
        bool TryDequeue(out Anchor uri);
    }
}
