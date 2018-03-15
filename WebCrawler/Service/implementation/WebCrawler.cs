using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using WebCrawler.Model;
using WebCrawler.Repository.Implementation;
using WebCrawler.Repository.Interface;
using WebCrawler.Service.Interface;

namespace WebCrawler.Service.Implementation
{
    internal delegate void LinkFound(Anchor link);

    internal class WebCrawler : IWebCrawler
    {
        private readonly WebClient _webClient;
        private readonly List<IObserver<Anchor>> _observers;

        public WebCrawler(/*IUriQueue uriQueue, IProcessedSet processedSet*/)
        {
            _webClient = new WebClient();
            _observers = new List<IObserver<Anchor>>();
        }

        public void Crawl(Anchor anchor)
        {
            _webClient.DownloadStringCompleted += WebClient_DownloadStringCompleted;
            _webClient.DownloadStringAsync(anchor.Uri, anchor);
        }

        private void WebClient_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            try
            {
                Anchor baseAnchor = (e.UserState as Anchor);

                var htmlDocument = new HtmlDocument();
                htmlDocument.LoadHtml(e.Result);

                var anchors = htmlDocument.DocumentNode.SelectNodes("//a");

                if (null == anchors)
                    return;

                foreach (var anchor in anchors.Select(node => new Anchor { Uri = new Uri(baseAnchor.Uri, node.Attributes["href"].Value), JumpCount = baseAnchor.JumpCount + 1 } ))
                {
                    _observers.ForEach(observer => observer.OnNext(anchor));

                }
            }
            catch (Exception exception)
            {
                // TODO: Log errors somehow
            }
        }

        public IDisposable Subscribe(IObserver<Anchor> observer)
        {
            if (!_observers.Contains(observer))
            {
                _observers.Add(observer);
            }
            return new Unsubscriber<Anchor>(_observers, observer);
        }
    }
}
