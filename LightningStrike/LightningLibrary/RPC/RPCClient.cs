using System;
namespace LightningLibrary.RPC
{
    public class RPCClient
    {

        private string _Authentication;
        private readonly Uri _Address;
        public RPCClient(string address)
        {
            this._Address = new Uri(address);
        }
    }
}
