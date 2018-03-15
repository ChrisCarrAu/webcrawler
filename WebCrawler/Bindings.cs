using Ninject.Modules;
using WebCrawler.Repository.Implementation;
using WebCrawler.Repository.Interface;
using WebCrawler.Service.implementation;
using WebCrawler.Service.Implementation;
using WebCrawler.Service.Interface;

namespace WebCrawler
{
    public class Bindings : NinjectModule
    {
        public override void Load()
        {
            Bind<IProcessedSet>().To<ProcessedSet>().InSingletonScope();
            Bind<IUriQueue>().To<ObservableUriQueue>().InSingletonScope();
            Bind<IUriQueueListener>().To<UriQueueListener>();
            Bind<IWebCrawler>().To<Service.Implementation.WebCrawler>();
            Bind<ICrawlFarm>().To<CrawlFarm>().InSingletonScope();
        }
    }
}
