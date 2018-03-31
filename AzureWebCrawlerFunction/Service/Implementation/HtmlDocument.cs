using System.Collections.Generic;
using System.Linq;
using AzureWebCrawlerFunction.Service.Interface;

namespace AzureWebCrawlerFunction.Service.Implementation
{
    public class HtmlDocument : IHtmlDocument
    {
        private readonly HtmlAgilityPack.HtmlDocument _htmlDocument = new HtmlAgilityPack.HtmlDocument();

        public void LoadHtml(string html)
        {
            _htmlDocument.LoadHtml(html);
        }

        /// <summary>
        /// Returns all anchor references in this document or empty enumerable if none
        /// </summary>
        public IEnumerable<string> AnchorReferences
        {
            get
            {
                var anchorNodes = _htmlDocument.DocumentNode.SelectNodes("//a");
                if (null == anchorNodes)
                {
                    return Enumerable.Empty<string>();
                }
                return anchorNodes.Where(node => node.Attributes.Contains("href")).Select(node => node.Attributes["href"].Value);
            }
        }
    }
}
