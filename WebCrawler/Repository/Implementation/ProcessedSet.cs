using System;
using System.Collections.Generic;
using WebCrawler.Model;
using WebCrawler.Repository.Interface;

namespace WebCrawler.Repository.Implementation
{
    internal class ProcessedSet : IProcessedSet
    {
        private readonly HashSet<Anchor> _set;

        public ProcessedSet()
        {
            _set = new HashSet<Anchor>();
        }

        public bool Add(Anchor uri)
        {
            return _set.Add(uri);
        }

        public bool Processed(Anchor uri)
        {
            return _set.Contains(uri);
        }
    }
}
