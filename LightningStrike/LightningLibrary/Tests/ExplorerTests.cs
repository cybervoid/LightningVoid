using System;
using LightningLibrary.REST;

namespace LightningLibrary.Tests
{
    public class ExplorerTests
    {
        public ExplorerTests()
        {
        }

        public void GetUnspent()
        {
            BlockExplorer explorer = new BlockExplorer();

            explorer.GetUnspent("3K1ZqF1W3X81S1GUcNysS74p5ar4JZWdjJ");
        }
    }
}
