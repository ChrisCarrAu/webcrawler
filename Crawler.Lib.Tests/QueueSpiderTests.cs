using Crawler.Lib.Model;
using Crawler.Lib.Repository.Implementation;
using Crawler.Lib.Service.Implementation;
using Crawler.Lib.Service.Interface;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace Crawler.Lib.Tests
{
    public class QueueSpiderTests
    {
        [Fact]
        public async void SpiderProcesses_Crawl_ReturnsCrawledAnchor()
        {
            var logger = new Mock<ILogger<QueueSpider>>();
            var serviceProvider = new Mock<IServiceProvider>();
            var uriQueue = new ObservableUriQueue();
            var processedSet = new ProcessedSet();

            serviceProvider
                .Setup(x => x.GetService(typeof(IUriParser)))
                .Returns(new MockUriParser());
            uriQueue.Enqueue(new Anchor { Uri = new Uri("http://testurl.com/") });

            var queueSpider = new QueueSpider(logger.Object, serviceProvider.Object, uriQueue, processedSet);
            await queueSpider.Crawl();

            Assert.True(processedSet.Processed("http://testurl2.com/"));
            Assert.True(uriQueue.IsEmpty);
        }
    }

    public class MockUriParser : IUriParser, IObservable<Anchor>
    {
        private readonly List<IObserver<Anchor>> _observers = new List<IObserver<Anchor>>();

        public async Task Crawl(Anchor anchor)
        {
            var node = new Anchor
            {
                Uri = new Uri(anchor.Uri, @"http://testurl2.com/"),
                Parent = anchor
            };
            _observers.ForEach(observer => observer.OnNext(node));

            _observers.ForEach(observer => observer.OnCompleted());
        }

        public IDisposable Subscribe(IObserver<Anchor> observer)
        {
            _observers.Add(observer);
            return new Unsubscriber<Anchor>(_observers, observer);
        }
    }
}
