using System;
using System.Collections.Generic;
using System.Net;

namespace Crawler.Lib.Model
{
    public class Anchor
    {
        public Uri Uri { get; set; }

        public Anchor Parent { get; set; }

        public WebHeaderCollection Headers { get; set; }

        public Exception Exception { get; set; }

        public int JumpCount
        {
            get
            {
                var count = 0;
                var myParent = Parent;
                while (null != myParent)
                {
                    count++;
                    myParent = myParent.Parent;
                }
                return count;
            }
        }

        public List<string> Jumps
        {
            get
            {
                var jumps = new List<string> {Uri.ToString()};
                var myParent = Parent;
                while (null != myParent)
                {
                    jumps.Add(myParent.Uri.ToString());
                    myParent = myParent.Parent;
                }
                return jumps;
            }
        }
    }
}
