using System;
using LightningLibrary.Tests;
using System.Collections.Generic;

namespace LightningConsole
{
    class MainClass
    {
        static Dictionary<int, string> seeds;
        public static void Initialize()
        {
            seeds = new Dictionary<int, string>();
            seeds.Add(0, "ryanseed_0");
            seeds.Add(1, "ryanseed_1");
            seeds.Add(2, "ryanseed_2");
            seeds.Add(3, "ryanseed_3");
            seeds.Add(4, "ryan1seed");
            seeds.Add(5, "seed12345678ryan12345678");
        }

        public static void Main(string[] args)
        {
            Initialize();


            //LightningTests lightningTests = new LightningTests();
            //lightningTests.RPCTest();

            LightningLibrary.Wallets.HDWallet hDWallet = new LightningLibrary.Wallets.HDWallet("ryan1seed");
            LightningTests lightningTests = new LightningTests();
            lightningTests.CreateBasicSwap((uint)0, seeds[0], seeds[1]);

            ExplorerTests explorerTests = new ExplorerTests();
            //explorerTests.GetUnspent();
            explorerTests.SegwitTestNet();
            //explorerTests.GetUnspentTestNet();

            WitnessTests witnessTests = new WitnessTests(seeds[0]);
            witnessTests.SegwitAddressTest(seeds[0]);
            witnessTests.P2WSH(seeds[0], seeds[1], seeds[2], seeds[3]);


            witnessTests.P2SH(seeds[0], seeds[1], seeds[2], seeds[3]);

            witnessTests.MultiSigFromBook();

            Console.WriteLine("Multi Sig");
            witnessTests.MultiSig(seeds[0], seeds[1], seeds[2], seeds[3]);

            return;

            Console.WriteLine("Get the P2WPKH signature");
            witnessTests.P2WPKH();

            Console.WriteLine("Review P2PKH (old way)");
            witnessTests.P2PK_H();


            witnessTests.P2WSHScript();
            Console.ReadLine();
            //Console.WriteLine("Hello World!");
        }
    }
}
