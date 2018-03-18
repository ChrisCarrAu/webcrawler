using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace Crawler.Lib.Model
{
    public class Anchor
    {
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
