using System;
using Crawler.Lib.Model;

namespace Crawler.Lib.Service.Interface
{
    public interface IUriQueueListener : IObserver<Anchor>
    {
    }
}