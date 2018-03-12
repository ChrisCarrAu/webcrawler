using System;
using WebCrawler.Model;

namespace WebCrawler.Repository.Interface
{
    internal interface IProcessedSet
    {
        bool Add(Anchor uri);
        bool Processed(Anchor uri);

    }
}
