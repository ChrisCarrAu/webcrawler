using System;
using System.Collections.Generic;
using WebCrawler.Model;
using WebCrawler.Repository.Interface;

namespace WebCrawler.Repository.Implementation
{
    internal class ProcessedSet : IProcessedSet
    {
        private readonly HashSet<string> _set;

        public ProcessedSet()
        {
            _set = new HashSet<string>();
        }

        public bool Add(string uri)
        {
            return _set.Add(uri);
        }

        public bool Processed(string uri)
        {
            return _set.Contains(uri);
        }
    }
}
