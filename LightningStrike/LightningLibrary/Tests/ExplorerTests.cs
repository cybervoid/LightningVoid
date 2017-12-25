﻿using System;
using System.Collections.Generic;
using System.Linq;
using LightningLibrary.Objects;
using LightningLibrary.REST;
using LightningLibrary.Utilities;
using LightningLibrary.Wallets;
using NBitcoin;
using Newtonsoft.Json;

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

        public void GetUnspentTestNet()
        {
            var net = NBitcoin.Network.TestNet;
            BlockExplorer explorer = new BlockExplorer("https://testnet.blockexplorer.com/");
            HDWallet wallet = new HDWallet("seed12345678ryan12345678",net);
            uint path = 0;
            var key = wallet.GetPrivateKey(path);

            Segwit segwit = new Segwit(net);
            var s = segwit.GetSegwitAddress(key);
            //txid = 6636b3fedb57be81232f92f80fa8d3df9a0f07305af2c7f705a7f353e516b1d7
            //2MwZsLbuB328gxHRfr1VDrfDrK6aWicQcAW
            //https://www.blocktrail.com/tBTC/tx/6636b3fedb57be81232f92f80fa8d3df9a0f07305af2c7f705a7f353e516b1d7
            var response = explorer.GetUnspent(s.ToString());
            List<ExplorerUnspent> unspent = JsonConvert.DeserializeObject<List<ExplorerUnspent>>(response.data);

            Transaction received = new Transaction();
            received.Outputs.Add(unspent[0].AsOutput());
            Console.WriteLine(received.ToHex());
            //ScriptCoin coin = received.Outputs.AsCoins().First().ToScriptCoin(segwit.GetSegwitScriptPubKey(key));
            Coin coin = received.Outputs.AsCoins().First();

            BitcoinAddress destination = BitcoinAddress.Create("2N8hwP1WmJrFF5QWABn38y63uYLhnJYJYTF");

            
            TransactionBuilder builder = new TransactionBuilder();
            Transaction unsigned = builder.AddCoins(coin).Send(destination, unspent[0].Amount).AddKeys(key).BuildTransaction(true);

            Console.WriteLine(unsigned);
            Console.WriteLine(s);
            Console.ReadLine();
        }
    }
}
