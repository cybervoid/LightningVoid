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
            //Create send transaction.

            //get redeem script
            var redeemScript = multi.GetRedeemScript(2, keys.ToArray());
            Transaction received = transactions[0];
            ScriptCoin coin = received.Outputs.AsCoins().First().ToScriptCoin(redeemScript);

            //create transaction:

            BitcoinAddress destination = BitcoinAddress.Create("2N8hwP1WmJrFF5QWABn38y63uYLhnJYJYTF"); //the faucet return address
            TransactionBuilder builder = new TransactionBuilder();
            builder.AddCoins(coin);
            builder.Send(destination, Money.Coins(1.299m));
            builder.SendFees(Money.Coins(0.001m));
            builder.SetChange(destination);

            var unsigned = builder.BuildTransaction(sign: false);

            var signedA = builder.AddCoins(coin).AddKeys(keys[0].PrivateKey).SignTransaction(unsigned);
            Transaction signedB = builder.AddCoins(coin).AddKeys(keys[1].PrivateKey).SignTransaction(signedA);
        
            Transaction fullySigned = builder.AddCoins(coin).CombineSignatures(signedA, signedB);

            Console.WriteLine(fullySigned.ToHex());
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

        public void TxDebug(uint path, params string[] seed)
        {
            Transaction tx = new Transaction("01000000000101fb1db914dd2be5311ebde6f86f1d49551057557fa22021a3a59623fa086836b2000000002322002029ccc4c03f9609ff9f79b0b7d3ade093ffd7da6d37c06edc65bfb898d4aee069ffffffff01e01dbe070000000017a914a9974100aeee974a20cda9a2f545704a0ab54fdc87040047304402204e4c8a3ebf889821e574edb805bb6b65523e5a000ee4729db7e7d2888741c5830220058df53eb3b6f052a8dab2f897901d85242297dfa6aafebeb577913c2782c16a01483045022100f03b655ffedbd262f98ed3a562b32bc676e565b457f3fe89f557b755f4ab0d7502200e6c3006351a220c8fcf0a563701537b8979ceb289170d16ff8da760083a62c80147522103a0b99131aca0a5c696fe9f7e63b987f185050d2e6b25f80731f9f0ab83702d862102103f9e18fb85e862a20ff9d7afb0172661296f08d988508a538a68c96a1c4a5752ae00000000");
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
        }

    }
}
