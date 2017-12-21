using RestSharp.Authenticators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RestSharp;
using LightningLibrary.RPC;

namespace LightningLibrary.Tests
{
    public class LightningTests
    {
        public void Run(string seed)
        {
            //var client = new RestClient("http://192.168.1.18")
            //{
            //    Authenticator = new HttpBasicAuthenticator("kek", "kek")
            //};
            
        }

        public void RPCTest()
        {
            //var privateConnection = new ConnectionOptions()
            //{
            //    Port = "9735",
            //    Url = "http://192.168.1.18"
            //};

            //var service = new LightningService(privateConnection);
            //var x = service.ListPeers();

        }
    }
}
