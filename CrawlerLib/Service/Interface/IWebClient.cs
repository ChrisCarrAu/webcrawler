﻿using System;
using System.Net;
using System.Threading.Tasks;

namespace Crawler.Lib.Service.Interface
{
    public interface IWebClient
    {
        WebHeaderCollection ResponseHeaders { get; }

        Task<byte[]> DownloadDataTaskAsync(string address);
        Task<byte[]> DownloadDataTaskAsync(Uri address);

        Task<string> DownloadStringTaskAsync(Uri address);
    }
}
