using Crawler.Lib.Model;
using Crawler.Lib.Repository.Implementation;
using Crawler.Lib.Service.implementation;
using Crawler.Lib.Service.Interface;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace Crawler.Lib.Tests
{
    public class QueueSpiderTests
    {
        [Fact]
        public async void ReturnAnchors()
        {
            var logger = new Mock<ILogger<QueueSpider>>();
            var serviceProvider = new Mock<IServiceProvider>();
            var uriQueue = new ObservableUriQueue();
            var processedSet = new ProcessedSet();

            serviceProvider
                .Setup(x => x.GetService(typeof(IUriParser)))
                .Returns(new MockUriParser());
            uriQueue.Enqueue(new Anchor { Uri = new Uri("http://testUrl.com") });

            var queueSpider = new QueueSpider(logger.Object, serviceProvider.Object, uriQueue, processedSet);
            await queueSpider.Crawl();
        }


    }

    public class MockUriParser : IUriParser, IObservable<Anchor>
    {
        private readonly List<IObserver<Anchor>> _observers = new List<IObserver<Anchor>>();

        public async Task Crawl(Anchor uri)
        {
            foreach (var observer in _observers)
            {
                observer.OnCompleted();
            }
        }

        public IDisposable Subscribe(IObserver<Anchor> observer)
        {
            _observers.Add(observer);
            return new Unsubscriber<Anchor>(_observers, observer);
        }
    }
}
