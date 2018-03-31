using AzureWebCrawlerFunction.Model;
using AzureWebCrawlerFunction.Service.Interface;
using Microsoft.Extensions.Logging;
using System;
using System.Net;
using System.Threading.Tasks;

namespace AzureWebCrawlerFunction.Service.Implementation
{
    /// <summary>
    /// Crawls a document looking for anchors.
    /// </summary>
    public class UriParser : IUriParser
    {
        private readonly IWebClient _webClient;
        private readonly ILogger<UriParser> _logger;

        public UriParser(ILogger<UriParser> logger, IWebClient webClient)
        {
            _logger = logger;
            _webClient = webClient;
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

                    foreach (var _anchor in htmlDocument.AnchorReferences)
                    {
                        var node = new Anchor
                        {
                            Uri = new Uri(anchor.Uri, _anchor),
                            Parent = anchor
                        };
                    }
                }
            }
            catch (WebException e)
            {
                _logger.LogError($"{anchor.Uri.ToString()}");
                _logger.LogError($"{e.ToString()}");
                anchor.Exception = e;
            }
            catch (ArgumentException arge)
            {
                _logger.LogError($"{anchor.Uri.ToString()}");
                _logger.LogError($"{arge.ToString()}");
                anchor.Exception = arge;
            }
            finally
            {
                _logger.LogInformation($">>> Crawl Complete {anchor.Uri.ToString()}");
            }
        }
    }
}
