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
            //builder.
            var unsigned = builder.BuildTransaction(sign: false);

            var signedA = builder.AddCoins(coin).AddKeys(keys[0].PrivateKey).SignTransaction(unsigned);
            Transaction signedB = builder.AddCoins(coin).AddKeys(keys[1].PrivateKey).SignTransaction(signedA);
        
            Transaction fullySigned = builder.AddCoins(coin).CombineSignatures(signedA, signedB);

            Console.WriteLine(fullySigned.ToHex());
            Console.ReadLine();
        }

        /// <summary>
        /// This method implements the Programming Blockchain code multi-signature transactions as segwit enabled.
        /// </summary>
        /// <param name="path">Path.</param>
        /// <param name="seed">Seed.</param>
        public void StepByStep(uint path, params string[] seed)
        {
            List<ExtKey> keys = new List<ExtKey>();
            Segwit segwit = new Segwit(NBitcoin.Network.TestNet);

            Key bob = GetKey(path, seed[0]).PrivateKey;
            Key alice = GetKey(path, seed[1]).PrivateKey;

            for (int i = 0; i < seed.Length; i++)
            {
                var key = GetKey(path, seed[i]);
                keys.Add(key);
                //Console.WriteLine(address.ToString());
            }
            NBitcoin.Network _Network = NBitcoin.Network.TestNet;

            //MultiSig multi = new MultiSig(NBitcoin.Network.TestNet);
            List<PubKey> pubKeys = new List<PubKey>();
            for (int i = 0; i < keys.Count; i++)
            {
                pubKeys.Add(keys[i].PrivateKey.PubKey);
            }

            Console.WriteLine("Section: P2SH (Pay To Script Hash)");
            Script pubKeyScript = PayToMultiSigTemplate.Instance.GenerateScriptPubKey(2, pubKeys.ToArray());
            Console.WriteLine("Generated pubKeyScript: \n\r" + pubKeyScript.ToString());
            Console.WriteLine();
            Console.WriteLine("P2SH: \t (Pay to Script Hash): Generate Payment Script");
            var paymentScript = pubKeyScript.PaymentScript;
            Console.WriteLine(paymentScript.ToString());
            Console.WriteLine();
            Console.WriteLine("Generate P2SH:\t(Pay To Script Hash) \t from scriptPubKey");
            var p2shPayToAddress = pubKeyScript.Hash.GetAddress(_Network);
            Console.WriteLine(p2shPayToAddress);
            Console.WriteLine();
            Console.WriteLine("Simulate a transaction");
            Transaction p2shReceived = new Transaction();
            p2shReceived.Outputs.Add(new TxOut(Money.Coins(1.0m), pubKeyScript.Hash)); //Warning: The payment is sent to redeemScript.Hash and not to redeemScript!
            //A script coin is used to spend when a combination of the owners want to spend the coins
            ScriptCoin p2shCoin = p2shReceived.Outputs.AsCoins().First().ToScriptCoin(pubKeyScript);
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("Section: P2WSH (Pay to Witness Script Hash)");

            Console.WriteLine();
            Console.WriteLine("Simulate a fake P2WSH payment script");
            var fakeKey = new Key();
            Console.WriteLine(fakeKey.PubKey.ScriptPubKey.WitHash.ScriptPubKey);
            //Output: 0 4c55134a297baf494323fd517df5780a0b0e8f7f8f794a0ea79ab94fd4498923
            Console.WriteLine(pubKeyScript.WitHash.ScriptPubKey);
            //Output: 0 29ccc4c03f9609ff9f79b0b7d3ade093ffd7da6d37c06edc65bfb898d4aee069
           
            Console.WriteLine("Simulate a fake segwit transaction");
            var addressFake = fakeKey.PubKey.WitHash.GetAddress(_Network);
            var p2shFake = addressFake.GetScriptAddress();
            var redeemScriptFake = fakeKey.PubKey.WitHash.ScriptPubKey;
            Transaction fakeReceived = new Transaction();
            fakeReceived.Outputs.Add(new TxOut(Money.Coins(1.0m), redeemScriptFake.WitHash));
            ScriptCoin coin = fakeReceived.Outputs.AsCoins().First().ToScriptCoin(redeemScriptFake);
            Console.WriteLine("Create a script coin");
            Transaction p2wshReceived = new Transaction();
            p2wshReceived.Outputs.Add(new TxOut(Money.Coins(1.0m), pubKeyScript.WitHash));
            //This did not throw an error:
            ScriptCoin p2wshCoin = p2wshReceived.Outputs.AsCoins().First().ToScriptCoin(pubKeyScript);


            throw new NotSupportedException("P2W over P2SH is NOT Multi-sig compatible!");
            //Source: https://bitcoincore.org/en/segwit_wallet_dev/#creation-of-p2sh-p2wpkh-address

            Console.WriteLine();
            Console.WriteLine("Section: P2W* over P2SH");
            Console.WriteLine();
            Console.WriteLine("Steps to Create a P2W* Over P2SH...");
            Console.WriteLine("\t1. Replacing the ScriptPubKey by its P2SH equivalent.");
            Console.WriteLine("\t2. The former ScriptPubKey will be placed as the only push in the scriptSig in the spending transaction");
            Console.WriteLine("\t3. All other data will be pushed in the witness of the spending transaction.");
            Console.WriteLine();
            Console.WriteLine("1. Replacing the ScriptPubKey by its P2SH equivalent.");
            //todo code to replace Script pub Key

            Console.WriteLine("Printing the ScriptPubKey: (fake)");
            Console.WriteLine(fakeKey.PubKey.WitHash.ScriptPubKey.Hash.ScriptPubKey);
            Script fakeScriptPubKey = fakeKey.PubKey.WitHash.ScriptPubKey.Hash.ScriptPubKey;
            Console.WriteLine("Printing the ScriptPubKey: (multi-sig)");
            Console.WriteLine(pubKeyScript.WitHash.ScriptPubKey.Hash.ScriptPubKey);
            Script multiScriptPubKey = pubKeyScript.WitHash.ScriptPubKey.Hash.ScriptPubKey;
            Console.WriteLine("Which gave us a well known P2SH scriptPubKey.");

            Console.WriteLine("Replacing the ScriptPubKey by its P2SH equivalent.");
            //Fake
            Console.WriteLine(fakeKey.PubKey.ScriptPubKey.WitHash.ScriptPubKey.Hash.ScriptPubKey);
            //Multi sig
            Console.WriteLine(pubKeyScript.WitHash.ScriptPubKey.Hash.ScriptPubKey);
            //OR maybe?
            //Console.WriteLine(pubKeyScript.PaymentScript.WitHash.ScriptPubKey.Hash.ScriptPubKey);
            Console.WriteLine("2. The former ScriptPubKey will be placed as the only push in the scriptSig in the spending transaction");
            //todo code to place former ScriptPubKey as the only push in the scriptSig in the spending transaction


            Console.WriteLine("3. All other data will be pushed in the witness of the spending transaction.");
            //todo code to push all other data in the witness of the spending transaction
            Transaction fake_p2w_over_p2shReceived = new Transaction();
            fakeReceived.Outputs.Add(new TxOut(Money.Coins(1.0m), redeemScriptFake.WitHash));
            Transaction p2w_Over_P2shReceived = new Transaction();

            Console.ReadLine();
        }

        public void ScriptExamples()
        {
            throw new NotImplementedException("Not yet implemented");
            //Get examples here: http://n.bitcoin.ninja/checktx
            //cNUnpYpMsJXYCERYBciJnsWBpcYEFjdcbq6dxj4SskGhs7uHuJ7Q
            //var _Network = Network.SegNet;
            Key key = Key.Parse("cNUnpYpMsJXYCERYBciJnsWBpcYEFjdcbq6dxj4SskGhs7uHuJ7Q", Network.TestNet);
        }

        public void NewCreateBasicSwap(uint path, params string[] seed)
        {
            List<ExtKey> keys = new List<ExtKey>();
            Segwit segwit = new Segwit(NBitcoin.Network.TestNet);
            for (int i = 0; i < seed.Length; i++)
            {
                var key = GetKey(path, seed[i]);
                //var address = segwit.GetP2SHAddress(key);
                keys.Add(key);
                //Console.WriteLine(address.ToString());
            }
            NBitcoin.Network _Network = NBitcoin.Network.TestNet;

            MultiSig multi = new MultiSig(NBitcoin.Network.TestNet);
            List<PubKey> pubKeys = new List<PubKey>();
            for (int i = 0; i < keys.Count; i++)
            {
                pubKeys.Add(keys[i].PrivateKey.PubKey);
            }
            Script pubKeyScript = PayToMultiSigTemplate.Instance.GenerateScriptPubKey(2, pubKeys.ToArray());

            BitcoinAddress address = pubKeyScript.WitHash.GetAddress(_Network);
            BitcoinScriptAddress p2sh = address.GetScriptAddress();
            Console.WriteLine("Send money here: " + p2sh.ToString());
            REST.BlockExplorer explorer = new REST.BlockExplorer("https://testnet.blockexplorer.com/");
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
            //var redeemScript = PayToMultiSigTemplate.Instance.GenerateScriptPubKey(2, pubKeys.ToArray());// multi.GetRedeemScript(2, keys.ToArray());

            Transaction received = transactions[0];
            ScriptCoin coin = received.Outputs.AsCoins().First().ToScriptCoin(pubKeyScript.WitHash.ScriptPubKey.Hash.ScriptPubKey);
            //create transaction:
            BitcoinAddress destination = BitcoinAddress.Create("2N8hwP1WmJrFF5QWABn38y63uYLhnJYJYTF"); //the faucet return address
            TransactionBuilder builder = new TransactionBuilder();
            builder.AddCoins(coin);
            builder.Send(destination, Money.Coins(1.299m));
            builder.SendFees(Money.Coins(0.001m));
            builder.SetChange(destination);
            //builder.
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
