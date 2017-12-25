using System;
using System.Collections.Generic;

namespace LightningLibrary.Objects
{
    public class ExplorerRequest
    {
        public List<string> Parameters { get; set; }
        public ExplorerRequest()
        {
            Parameters = new List<string>();
        }
    }
}
