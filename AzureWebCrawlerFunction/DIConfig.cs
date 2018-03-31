using System.Collections.Generic;
using System.Text;
using Autofac;
using AzureFunctions.Autofac.Configuration;
using AzureWebCrawlerFunction.Service.Implementation;
using AzureWebCrawlerFunction.Service.Interface;
using UriParser = System.UriParser;

namespace AzureWebCrawlerFunction
{
    public class DIConfig
    {
        public DIConfig()
        {
            DependencyInjection.Initialize(builder =>
                {
                    builder.RegisterType<HtmlDocument>().As<IHtmlDocument>();
                    builder.RegisterType<UriParser>().As<IUriParser>();
                    builder.RegisterType<WebClient>().As<IWebClient>();
                }
            );
        }
    }
}
