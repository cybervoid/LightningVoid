using System;
using System.Collections.Generic;
using System.Linq;
using LightningLibrary.Objects;
using LightningLibrary.REST;
using LightningLibrary.Utilities;
using LightningLibrary.Wallets;
using NBitcoin;
using NBitcoin.DataEncoders;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NBitcoin.RPC;


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

        /// <summary>
        /// NEW!!  So far alright?
        /// </summary>
        public void SegwitTestNet()
        {
            //The faucet: https://testnet.manu.backend.hamburg/faucet
            //https://bitcoin.stackexchange.com/questions/59231/how-to-sign-a-segwit-transaction-via-nbitcoin
            var net = NBitcoin.Network.TestNet;
            BlockExplorer explorer = new BlockExplorer("https://testnet.blockexplorer.com/");
            HDWallet wallet = new HDWallet("seed12345678ryan12345678", net);
            uint path = 0;
            var extkey = wallet.GetPrivateKey(path);
            Key k = extkey.PrivateKey;

            //This gives you a Bech32 address (currently not really interoperable in wallets, so you need to convert it into P2SH)
            var address = k.PubKey.WitHash.GetAddress(net);

            var p2sh = address.GetScriptAddress();
            //p2sh is now an interoperable P2SH segwit address

            //SENT TO: 2NGSoYM3yLi9SXvZc5yYcSwHWyCWzgBrP9X 
            //TXID 1397a4cc480879eae604ce871c47a4d690c6ea6a6dcfd7e38d95f31b81593556

            var response = explorer.GetUnspent(p2sh.ToString());
            List<ExplorerUnspent> unspent = response.Convert<List<ExplorerUnspent>>();
            List<Transaction> transactions = new List<Transaction>();
            foreach (var item in unspent)
            {
                string txcontent = "{\"txid\":\"6636b3fedb57be81232f92f80fa8d3df9a0f07305af2c7f705a7f353e516b1d7\",\"version\":1,\"locktime\":0,\"vin\":[{\"txid\":\"50b7d9a5fa1281e7020fa8a152835756e0d57d6b4d634b4251ab500e8630bc3e\",\"vout\":1,\"scriptSig\":{\"asm\":\"0014a16f4ba22e84c364ec4f8fe19d8a48762156b41e\",\"hex\":\"160014a16f4ba22e84c364ec4f8fe19d8a48762156b41e\"},\"sequence\":4294967295,\"n\":0,\"addr\":\"2N9viNVJ5MsAM8MXdUuATDwKeMjMLQkaXyR\",\"valueSat\":193987962850,\"value\":1939.8796285,\"doubleSpentTxID\":null}],\"vout\":[{\"value\":\"2.00000000\",\"n\":0,\"scriptPubKey\":{\"hex\":\"a9142f672b3ea4af55d9da43e507e3c060d2e34521ba87\",\"asm\":\"OP_HASH160 2f672b3ea4af55d9da43e507e3c060d2e34521ba OP_EQUAL\",\"addresses\":[\"2MwZsLbuB328gxHRfr1VDrfDrK6aWicQcAW\"],\"type\":\"scripthash\"},\"spentTxId\":null,\"spentIndex\":null,\"spentHeight\":null},{\"value\":\"1937.87862850\",\"n\":1,\"scriptPubKey\":{\"hex\":\"a914859695c2cb37ee30bf6a18943c8c27a3ac0e6faa87\",\"asm\":\"OP_HASH160 859695c2cb37ee30bf6a18943c8c27a3ac0e6faa OP_EQUAL\",\"addresses\":[\"2N5RaFdK3rgsNayXnkTQaSLKVBB3brW7G4m\"],\"type\":\"scripthash\"},\"spentTxId\":\"892e6facc4fc596852d41af367dd41f7a7ec1b11a319a202bc0d4c5b8f792f2f\",\"spentIndex\":0,\"spentHeight\":1255964}],\"blockhash\":\"000000000007dbc3ffd03559f2192b299bc0c20aa0aea8e1939f2731e01103e8\",\"blockheight\":1255946,\"confirmations\":123,\"time\":1514228293,\"blocktime\":1514228293,\"valueOut\":1939.8786285,\"size\":138,\"valueIn\":1939.8796285,\"fees\":0.001}";
                ExplorerResponse txResponse = explorer.GetTransaction(item.txid);
                RawFormat format = RawFormat.Satoshi;
                var tx = Transaction.Parse(txResponse.data, format, net);
                transactions.Add(tx);
            }
            //For spending, it works the same as a a normal P2SH
            //You need to get the ScriptCoin, the RedeemScript of you script coin should be k.PubKey.WitHash.ScriptPubKey.            var redeemScript = k.PubKey.WitHash.ScriptPubKey;
            var redeemScript = k.PubKey.WitHash.ScriptPubKey;
            Transaction received = transactions[0];
            ScriptCoin coin = received.Outputs.AsCoins().First().ToScriptCoin(redeemScript);
            //1397a4cc480879eae604ce871c47a4d690c6ea6a6dcfd7e38d95f31b81593556

            BitcoinAddress destination = BitcoinAddress.Create("2N8hwP1WmJrFF5QWABn38y63uYLhnJYJYTF"); //the faucet return address
            TransactionBuilder builder = new TransactionBuilder();
            builder.AddCoins(coin);
            builder.AddKeys(k);
            builder.Send(destination, Money.Coins(1.99m));
            builder.SendFees(Money.Coins(0.001m));
            builder.SetChange(p2sh);
            var signedTx = builder.BuildTransaction(true);

            Console.WriteLine(signedTx.ToHex());
            string x = ";;";
            //Assert.True(builder.Verify(signedTx));
        }
       

        public TxDestination GetRedeemHash(Script scriptPubKey)
        {
            if (scriptPubKey == null)
                throw new ArgumentNullException("scriptPubKey");
            return PayToScriptHashTemplate.Instance.ExtractScriptPubKeyParameters(scriptPubKey) as TxDestination
                    ??
                    PayToWitScriptHashTemplate.Instance.ExtractScriptPubKeyParameters(scriptPubKey);
        }
    }
}
