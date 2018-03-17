using System;
using Crawler.Lib.Model;

namespace Crawler.Lib.Repository.Interface
{
    public interface IUriQueue : IObservable<Anchor>
    {
        void Enqueue(Anchor uri);
        bool TryDequeue(out Anchor uri);
    }
}
