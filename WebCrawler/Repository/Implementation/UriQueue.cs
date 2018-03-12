﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks.Dataflow;
using WebCrawler.Model;
using WebCrawler.Repository.Interface;

namespace WebCrawler.Repository.Implementation
{
    internal class UriQueue : IUriQueue, IObservable<Anchor>
    {
        private readonly List<IObserver<Anchor>> _observers;
        private readonly ConcurrentQueue<Anchor> _queue = new ConcurrentQueue<Anchor>();

        public UriQueue()
        {
            _observers = new List<IObserver<Anchor>>();
        }

        public void Enqueue(Anchor anchor)
        {
            _queue.Enqueue(anchor);
            _observers.ForEach(observer => observer.OnNext(anchor));
        }

        public bool TryDequeue(out Anchor uri)
        {
            // TODO: Can I use TransformBlock<Anchor, Anchor>
            return _queue.TryDequeue(out uri);
        }

        public IDisposable Subscribe(IObserver<Anchor> observer)
        {
            if (!_observers.Contains(observer))
            {
                _observers.Add(observer);
            }
            return new Unsubscriber<Anchor>(_observers, observer);
        }
    }
}