namespace WebCrawler.Repository.Interface
{
    internal interface IProcessedSet
    {
        bool Add(string uri);
        bool Processed(string uri);

    }
}
