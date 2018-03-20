using Crawler.Lib.Model;
using Crawler.Lib.Service.Implementation;
using Crawler.Lib.Service.Interface;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Crawler.Lib.Tests
{
    public class UriParserTests
    {
        [Fact]
        public async void UriParser_GivenHtmlWith10Anchors_Returns10Anchors()
        {
            var webHeaderCollection = new WebHeaderCollection();
            webHeaderCollection["content-type"] = @"text/html";

            var logger = new Mock<ILogger<Service.Implementation.UriParser>>();
            var mockWebClient = new Mock<IWebClient>();
            mockWebClient
                .Setup(m => m.DownloadDataTaskAsync(""))
                .Returns(Task.FromResult(Encoding.ASCII.GetBytes("blah blah")));
            mockWebClient
                .Setup(m => m.DownloadStringTaskAsync(new Uri("http://mytesturi.com/")))
                .Returns(Task.FromResult(Resources.appthem));
            mockWebClient
                .Setup(m => m.ResponseHeaders)
                .Returns(webHeaderCollection);            // ["content-type"].StartsWith(@"text/", StringComparison.Ordinal

            var uriParser = new Service.Implementation.UriParser(logger.Object, mockWebClient.Object);
            var mockAnchors = new MockAnchors();
            uriParser.Subscribe(mockAnchors);
            await uriParser.Crawl(new Model.Anchor { Uri = new System.Uri("http://mytesturi.com") });

            Assert.True(mockAnchors.Count == 10);
        }

        private class MockAnchors : List<Anchor>, IObserver<Anchor>
        {
            public void OnCompleted()
            {
                //
            }

            public void OnError(Exception error)
            {
                //
            }

            public void OnNext(Anchor anchor)
            {
                Add(anchor);
            }
        }
    }
}
