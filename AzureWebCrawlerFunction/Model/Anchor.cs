using System;
using System.Collections.Generic;
using System.Net;
using Newtonsoft.Json;

namespace AzureWebCrawlerFunction.Model
{
    public class Anchor
    {
        [JsonProperty(Required = Required.Always)]
        public Uri Uri { get; set; }

        public Anchor Parent { get; set; }

        public WebHeaderCollection Headers { get; set; }

        public Exception Exception { get; set; }

        public IEnumerable<string> Jumps
        {
            get
            {
                var myParent = Parent;
                while (null != myParent)
                {
                    yield return myParent.Uri.ToString();
                    myParent = myParent.Parent;
                }
            }
        }
    }
}
