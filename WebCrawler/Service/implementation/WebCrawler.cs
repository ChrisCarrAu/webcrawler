using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using WebCrawler.Model;
using WebCrawler.Repository.Implementation;
using WebCrawler.Service.Interface;

namespace WebCrawler.Service.Implementation
{
    internal delegate void LinkFound(Anchor link);

    /// <summary>
    /// Crawls a website, looking for anchors.
    /// Implements IObservable<Uri> and returns Anchor URIs to Observers.
    /// </summary>
    internal class WebCrawler : IWebCrawler
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
                var type = _webClient.ResponseHeaders["content-type"];

                if (type.StartsWith(@"text/"))
                {
                    _webClient.DownloadStringCompleted += WebClient_DownloadStringCompleted;
                    _webClient.DownloadStringAsync(anchor.Uri, anchor);
                }
            }
            catch (WebException e)
            {
                // TODO: Handle error
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

                foreach (var anchor in anchors.Select(node => new Anchor { Uri = new Uri(baseAnchor.Uri, node.Attributes["href"].Value), Parent = baseAnchor } ))
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
