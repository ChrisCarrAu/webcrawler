using Crawler.Lib.Model;
using Crawler.Lib.Repository.Implementation;
using Crawler.Lib.Service.Interface;
using HtmlAgilityPack;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

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

        private readonly ILogger<WebCrawler> _logger;

        public WebCrawler(ILogger<WebCrawler> logger)
        {
            _webClient = new WebClient();
            _observers = new List<IObserver<Anchor>>();

            _logger = logger;
        }

        public async Task Crawl(Anchor anchor)
        {
            try
            {
                _logger.LogInformation($">>> Start Crawling {anchor.Uri.ToString()}");
                var data = await _webClient.DownloadDataTaskAsync(anchor.Uri);
                anchor.Headers = _webClient.ResponseHeaders;

                if (anchor.Headers["content-type"].StartsWith(@"text/", StringComparison.Ordinal))
                {
                    var contents = await _webClient.DownloadStringTaskAsync(anchor.Uri);

                    var htmlDocument = new HtmlDocument();
                    htmlDocument.LoadHtml(contents);

                    var anchors = htmlDocument.DocumentNode.SelectNodes("//a");

                    if (null == anchors)
                        return;

                    foreach (var _anchor in anchors)
                    {
                        if (_anchor.Attributes.Contains("href"))
                        {
                            var node = new Anchor
                            {
                                Uri = new Uri(anchor.Uri, _anchor.Attributes["href"].Value),
                                Parent = anchor
                            };
                            _observers.ForEach(observer => observer.OnNext(node));
                        }
                    }
                }
            }
            catch (WebException e)
            {
                _observers.ForEach(observer => observer.OnError(e));
                anchor.Exception = e;
            }
            catch (ArgumentException arge)
            {
                _observers.ForEach(observer => observer.OnError(arge));
                anchor.Exception = arge;
            }
            finally
            {
                _logger.LogInformation($">>> Crawl Complete {anchor.Uri.ToString()}");
                _observers.ForEach(observer => observer.OnCompleted());
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
