using HtmlAgilityPack;
using System;
using System.Linq;
using System.Net;
using WebCrawler.Model;
using WebCrawler.Repository.Interface;
using WebCrawler.Service.Interface;

namespace WebCrawler.Service.Implementation
{
    internal delegate void LinkFound(Anchor link);

    internal class WebCrawler : IWebCrawler
    {
        private readonly WebClient _webClient;
        private readonly IUriQueue _uriQueue;
        private Uri _baseUri;
        private readonly IProcessedSet _processedSet;

        public WebCrawler(IUriQueue uriQueue, IProcessedSet processedSet)
        {
            _webClient = new WebClient();
            _uriQueue = uriQueue;
            _processedSet = processedSet;
        }

        public void Crawl(Anchor anchor)
        {
            _baseUri = anchor.Uri;
            _webClient.DownloadStringCompleted += WebClient_DownloadStringCompleted;
            _webClient.DownloadStringAsync(anchor.Uri);
        }

        private void WebClient_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            try
            {
                var htmlDocument = new HtmlDocument();
                htmlDocument.LoadHtml(e.Result);

                var anchors = htmlDocument.DocumentNode.SelectNodes("//a");

                if (null == anchors)
                    return;

                foreach (var anchor in anchors.Select(node => new Anchor { Uri = new Uri(_baseUri, node.Attributes["href"].Value) } ))
                {
                    if (_processedSet.Processed(anchor))
                        continue;

                    _processedSet.Add(anchor);
                    _uriQueue.Enqueue(anchor);
                }
            }
            catch (Exception exception)
            {
                // TODO: Log errors somehow
            }
        }
    }
}
