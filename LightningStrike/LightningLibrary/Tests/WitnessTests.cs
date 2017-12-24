using LightningLibrary.Wallets;
using NBitcoin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LightningLibrary.Utilities;
namespace LightningLibrary.Tests
{
    public class WitnessTests
    {
        string _seed;
        public WitnessTests(string seed)
        {
            this._seed = seed;
        }


        /// <summary>
        /// Important: P2PK 
        /// </summary>
        public void P2PK_H()
        {
            //https://programmingblockchain.gitbooks.io/programmingblockchain/content/other_types_of_ownership/p2pk[h]_pay_to_public_key_[hash].html
            HDWallet hdWallet = new HDWallet(_seed);
            uint path = 0;
            var privateKey = hdWallet.GetPrivateKey();
            var pubKey = hdWallet.GetPublicKey(path);

            Console.WriteLine("We learned that a Bitcoin Address was the hash of a public key:");
            var publicKeyHash = pubKey.PubKey.Hash;
            Console.WriteLine(publicKeyHash.GetAddress(NBitcoin.Network.Main));
            var address = hdWallet.GetBitcoinAddress(path);
            Console.WriteLine(address.ToString());
            //var bitcoinAddress = publicKeyHash.GetAddress(Network.Main);
            //Console.WriteLine(publicKeyHash); // 41e0d7ab8af1ba5452b824116a31357dc931cf28
            //Console.WriteLine(bitcoinAddress); // 171LGoEKyVzgQstGwnTHVh3TFTgo5PsqiY

            Console.ReadLine();
        }

        public void P2WPKH()
        {
            //https://programmingblockchain.gitbooks.io/programmingblockchain/content/other_types_of_ownership/p2wpkh_pay_to_witness_public_key_hash.html
            HDWallet hdWallet = new HDWallet(_seed);
            uint path = 0;
            var privateKey = hdWallet.GetPrivateKey();
            var pubKey = hdWallet.GetPublicKey(path);

            Console.WriteLine("In NBitcoin, spending a P2WPKH output is no different from spending a normal P2PKH. To get the ScriptPubKey from a public key simply use PubKey.WitHash instead of PubKey.Hash");
            var publicKeyWitnessHash = pubKey.PubKey.WitHash;

            Console.WriteLine(publicKeyWitnessHash.GetAddress(NBitcoin.Network.Main));
            //var address = hdWallet.GetBitcoinAddress(path);
            //Console.WriteLine(address.ToString());
        }

        public void MultiSigFromBook()
        {
            Key bob = new Key();
            Key alice = new Key();
            Key satoshi = new Key();

            var scriptPubKey = PayToMultiSigTemplate.Instance.GenerateScriptPubKey(2, new[] { bob.PubKey, alice.PubKey, satoshi.PubKey });

            Console.WriteLine(scriptPubKey);
            var received = new Transaction();
            received.Outputs.Add(new TxOut(Money.Coins(1.0m), scriptPubKey));
            Coin coin = received.Outputs.AsCoins().First();
            BitcoinAddress nico = new Key().PubKey.GetAddress(Network.Main);
            TransactionBuilder builder = new TransactionBuilder();
            Transaction unsigned = builder
                                  .AddCoins(coin)
                                  .Send(nico, Money.Coins(1.0m))
                                  .BuildTransaction(sign: false);
            Helpers.ChangeColor();
            Console.WriteLine(unsigned);
            Helpers.ResetColor();
            Transaction aliceSigned = builder
                                    .AddCoins(coin)
                                    .AddKeys(alice)
                                    .SignTransaction(unsigned);
            Console.WriteLine("Check signed:");
            Helpers.ChangeColor();
            Console.WriteLine(aliceSigned);
            Helpers.ResetColor();

            Transaction bobSigned = builder
                                    .AddCoins(coin)
                                    .AddKeys(bob)
                                    //At this line, SignTransaction(unSigned) has the identical functionality with the SignTransaction(aliceSigned).
                                    //It's because unsigned transaction has already been signed by Alice privateKey from above.
                                    .SignTransaction(aliceSigned);

            Transaction fullySigned = builder
                                        .AddCoins(coin)
                                        .CombineSignatures(aliceSigned, bobSigned);

            Console.WriteLine(fullySigned);
            Console.WriteLine("End of book example");
            Console.WriteLine();
        }

        public void MultiSig(string seed0, string seed1, string seed2, string seed3)
        {
            //https://programmingblockchain.gitbooks.io/programmingblockchain/content/other_types_of_ownership/multi_sig.html
            HDWallet walletAlice = new HDWallet(seed1); //signer 1
            HDWallet walletBob = new HDWallet(seed0); //signer 2
            HDWallet walletSatoshi = new HDWallet(seed2); //does not sign
            HDWallet walletNico = new HDWallet(seed3);
            uint path = 0;
            ExtPubKey keyBob = walletBob.GetPublicKey(path);
            ExtPubKey keyAlice = walletAlice.GetPublicKey(path);
            ExtPubKey keySatoshi = walletAlice.GetPublicKey(path);
            BitcoinAddress addressNico = walletNico.GetBitcoinAddress(path); //recipient of transaction

            Console.WriteLine("Generate a script to receive coins, it will require 2 of 3 signatures to spend.");
            var scriptPubKey = PayToMultiSigTemplate.Instance.GenerateScriptPubKey(2, new[] { keyBob.PubKey, keyAlice.PubKey, keySatoshi.PubKey });
            Console.WriteLine(scriptPubKey);

            Console.WriteLine("Received coins in a transaction");
            var received = new Transaction();
            received.Outputs.Add(new TxOut(Money.Coins(1.0m), scriptPubKey));
            Coin coin = received.Outputs.AsCoins().First();
            //Nico's wallet.
           
            //use the transaction builder to build an unsigned transaction to Nico.
            TransactionBuilder builder = new TransactionBuilder();
            Transaction unsigned = builder.AddCoins(coin).Send(addressNico, Money.Coins(1.0m)).BuildTransaction(sign: false);
            Helpers.ChangeColor();
            Console.WriteLine(unsigned);
            Helpers.ResetColor();
            Console.WriteLine("Alice signs transaction:");
            //Key alice = walletAlice.GetPrivateKey().PrivateKey;
            Transaction aliceSigned = builder.AddCoins(coin).AddKeys(walletAlice.GetPrivateKey(path)).SignTransaction(unsigned);            

            Helpers.ChangeColor();
            Console.WriteLine(aliceSigned);
            Console.WriteLine("Script sig, one 0 should be replaced with a hash sig");
            Helpers.ResetColor();
            Console.WriteLine("Bob signs transaction:");
            //At this line, SignTransaction(unSigned) has the identical functionality with the SignTransaction(aliceSigned).
            //It's because unsigned transaction has already been signed by Alice privateKey from above.
            Transaction bobSigned = builder.AddCoins(coin).AddKeys(walletBob.GetPrivateKey(path)).SignTransaction(aliceSigned);            
            Helpers.ChangeColor();
            Console.WriteLine(bobSigned);
            Helpers.ResetColor();

            Console.WriteLine("Combine Signatures");
            Transaction fullySigned = builder.AddCoins(coin).CombineSignatures(aliceSigned, bobSigned);
            Helpers.ChangeColor();
            Console.WriteLine(fullySigned.ToString());
            Helpers.ResetColor();
        }

        public void P2WSHScript()
        {
            Console.WriteLine("P2WSH (Pay to Witness Script Hash)");
            var key = new Key();
            Console.WriteLine(key.PubKey.ScriptPubKey);
            Console.WriteLine(key.PubKey.ScriptPubKey.WitHash.ScriptPubKey);
        }

        /// <summary>
        /// To harness the advantages of segwit, while being compatible with old software, P2W over P2SH is allowed. For old node, it will look like a normal P2SH payment.
        /// </summary>
        public void P2WOverP2SH()
        {
            Console.WriteLine("Let’s take the example of P2WPKH over P2SH, also called with the sweet name of P2SH(P2WPKH).");
            var key = new Key();
            Console.WriteLine(key.PubKey.WitHash.ScriptPubKey.Hash.ScriptPubKey);
        }
    }
}
