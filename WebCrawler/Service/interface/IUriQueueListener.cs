using System;
using WebCrawler.Model;

namespace WebCrawler.Service.Interface
{
    internal interface IUriQueueListener : IObserver<Anchor>
    {
    }
}