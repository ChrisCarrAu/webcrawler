namespace Crawler.Lib.Repository.Interface
{
    public interface IProcessedSet
    {
        bool Add(string uri);
        bool Processed(string uri);

    }
}
