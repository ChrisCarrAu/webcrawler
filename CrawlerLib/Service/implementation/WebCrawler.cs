using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Crawler.Lib.Model;
using Crawler.Lib.Repository.Implementation;
using Crawler.Lib.Service.Interface;

namespace Crawler.Lib.Service.Implementation
{
    public delegate void LinkFound(Anchor link);

    /// <summary>
    /// Crawls a website, looking for anchors.
    /// Implements IObservable<Uri> and returns Anchor URIs to Observers.
    /// </summary>
    public class WebCrawler : IWebCrawler
    {
        private readonly WebClient _webClient;
        private readonly List<IObserver<Anchor>> _observers;

        public WebCrawler()
        {
            _webClient = new WebClient();
            _observers = new List<IObserver<Anchor>>();
        }

        public void Crawl(Anchor anchor)
        {
            try
            {
                _webClient.DownloadData(anchor.Uri);
                anchor.Headers = _webClient.ResponseHeaders;

                if (anchor.Headers["content-type"].StartsWith(@"text/", StringComparison.Ordinal))
                {
                    _webClient.DownloadStringCompleted += WebClient_DownloadStringCompleted;
                    _webClient.DownloadStringAsync(anchor.Uri, anchor);
                }
            }
            catch (WebException e)
            {
                anchor.Exception = e;
            }
        }

        private void WebClient_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            try
            {
                var baseAnchor = (e.UserState as Anchor);

                var htmlDocument = new HtmlDocument();
                htmlDocument.LoadHtml(e.Result);

                var anchors = htmlDocument.DocumentNode.SelectNodes("//a");

                if (null == anchors)
                    return;

                foreach (var anchor in anchors)
                {
                    var node = new Anchor
                    {
                        Uri = new Uri(baseAnchor.Uri, anchor.Attributes["href"].Value),
                        Parent = baseAnchor
                    };
                    _observers.ForEach(observer => observer.OnNext(node));
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
