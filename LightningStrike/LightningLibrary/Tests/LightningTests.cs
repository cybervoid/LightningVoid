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
using LightningLibrary.Objects;

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
                //Console.WriteLine(address.ToString());
            }

            MultiSig multi = new MultiSig(NBitcoin.Network.TestNet);
            var p2sh = multi.GetP2SHAddress(2, keys.ToArray());
            Console.WriteLine(p2sh.ToString());
            //multi: b2366808fa2396a5a32120a27f55571055491d6ff8e6bd1e31e52bdd14b91dfb

            REST.BlockExplorer explorer = new REST.BlockExplorer("https://testnet.blockexplorer.com/");

            //var tx = explorer.GetTransaction("b2366808fa2396a5a32120a27f55571055491d6ff8e6bd1e31e52bdd14b91dfb");

            var response = explorer.GetUnspent(p2sh.ToString());
            List<ExplorerUnspent> unspent = response.Convert<List<ExplorerUnspent>>();
            List<Transaction> transactions = new List<Transaction>();
            foreach (var item in unspent)
            {
                
                ExplorerResponse txResponse = explorer.GetTransaction(item.txid);
                RawFormat format = RawFormat.Satoshi;
                var tx = Transaction.Parse(txResponse.data, format, Network.TestNet);
                transactions.Add(tx);
            }
            Console.ReadLine();
        }

        public ExtKey GetKey(uint path, string seed)
        {
            var wallet = new HDWallet(seed);
            return wallet.GetPrivateKey(path);
        }

        public void GetUnspent(ExtKey key)
        {
            //b5a4fa34d6ff45d66fcce713e8db9ff0308f1687dc83171bccdb1d17df84c7eb
        }

    }
}
