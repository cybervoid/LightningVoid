using RestSharp.Authenticators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RestSharp;
using LightningLibrary.RPC;
using LightningLibrary.Wallets;
using LightningLibrary.Utilities;
using NBitcoin;

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

        public void CreateBasicSwap(uint path, params string[] seed)
        {
            List<ExtKey> keys = new List<ExtKey>();
            Segwit segwit = new Segwit(NBitcoin.Network.TestNet);
            for (int i = 0; i < seed.Length; i++)
            {
                var key = GetKey(path, seed[i]);
                var address = segwit.GetP2SHAddress(key);
                keys.Add(key);
                Console.WriteLine(address.ToString());
            }

            Console.ReadLine();
        }

        public ExtKey GetKey(uint path, string seed)
        {
            var wallet = new HDWallet(seed);
            return wallet.GetPrivateKey(path);
        }

    }
}
