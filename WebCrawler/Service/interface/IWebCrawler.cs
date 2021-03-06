﻿using System;
using WebCrawler.Model;

namespace WebCrawler.Service.Interface
{
    interface IWebCrawler : IObservable<Anchor>
    {
        void Crawl(Anchor uri);
    }
}
