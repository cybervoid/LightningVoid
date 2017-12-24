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

        /// <summary>
        /// Pay 2 script hash - pre-segwit
        /// </summary>
        public void P2SH(string seed0, string seed1, string seed2, string seed3)
        {
            //https://programmingblockchain.gitbooks.io/programmingblockchain/content/other_types_of_ownership/p2sh_pay_to_script_hash.html
            HDWallet walletAlice = new HDWallet(seed1); //signer 1
            HDWallet walletBob = new HDWallet(seed0); //signer 2
            HDWallet walletSatoshi = new HDWallet(seed2); //does not sign
            HDWallet walletNico = new HDWallet(seed3);
            uint path = 0;

            Key bob = walletBob.GetPrivateKey(path).PrivateKey;
            Key alice = walletAlice.GetPrivateKey(path).PrivateKey;
            Key satoshi = walletSatoshi.GetPrivateKey(path).PrivateKey;

            Script scriptPubKey = PayToMultiSigTemplate.Instance
                            .GenerateScriptPubKey(2, new[] { bob.PubKey, alice.PubKey, satoshi.PubKey });

            Console.WriteLine("Script " + scriptPubKey);
            Console.WriteLine("With P2SH Payment: " + scriptPubKey.PaymentScript);
            Console.WriteLine(scriptPubKey.PaymentScript.Hash.GetAddress(Network.Main));
            Console.WriteLine();
            //Set up a new transaction to use this address
            //var redeemAddress = scriptPubKey.PaymentScript.Hash.GetAddress(Network.Main);
            Script redeemScript = PayToMultiSigTemplate.Instance
                                    .GenerateScriptPubKey(2, new[] { bob.PubKey, alice.PubKey, satoshi.PubKey });
            Console.WriteLine(redeemScript.Hash); //redeemScript.Hash is NOT the bitcoin address
            Transaction received = new Transaction();
            //Pay to the script hash
            received.Outputs.Add(new TxOut(Money.Coins(1.0m), redeemScript.Hash));
            //Warning: Warning: The payment is sent to redeemScript.Hash and not to redeemScript!

            //to spend what they have, any owner will need to create script coin.
            ScriptCoin coin = received.Outputs.AsCoins().First().ToScriptCoin(redeemScript);
            TransactionBuilder builder = new TransactionBuilder();
            Transaction unsigned = builder.AddCoins(coin).Send(walletNico.GetBitcoinAddress(path), Money.Coins(1.0m)).BuildTransaction(sign: false);
            Helpers.ChangeColor();
            Console.WriteLine(unsigned);
            Helpers.ResetColor();
            Console.WriteLine("Alice signs transaction:");
            //Key alice = walletAlice.GetPrivateKey().PrivateKey;
            Transaction aliceSigned = builder.AddCoins(coin).AddKeys(alice).SignTransaction(unsigned);

            Helpers.ChangeColor();
            Console.WriteLine(aliceSigned);
            Console.WriteLine("Script sig, one 0 should be replaced with a hash sig");
            Helpers.ResetColor();
            Console.WriteLine("Bob signs transaction:");
            //At this line, SignTransaction(unSigned) has the identical functionality with the SignTransaction(aliceSigned).
            //It's because unsigned transaction has already been signed by Alice privateKey from above.
            Transaction bobSigned = builder.AddCoins(coin).AddKeys(bob).SignTransaction(aliceSigned);
            Helpers.ChangeColor();
            Console.WriteLine(bobSigned);
            Helpers.ResetColor();

            Console.WriteLine("Combine Signatures");
            Transaction fullySigned = builder.AddCoins(coin).CombineSignatures(aliceSigned, bobSigned);
            Helpers.ChangeColor();
            Console.WriteLine(fullySigned.ToString());
            Helpers.ResetColor();

            Console.WriteLine();
        }

        public void P2WSH(string seed0, string seed1, string seed2, string seed3)
        {
            var key = new Key();
            Console.WriteLine(key.PubKey.ScriptPubKey.WitHash.ScriptPubKey);
            Console.WriteLine();
        }

        public void HDWalletTest(string seed0, string seed1, string seed2, string seed3)
        {
            //https://programmingblockchain.gitbooks.io/programmingblockchain/content/other_types_of_ownership/p2sh_pay_to_script_hash.html
            HDWallet walletAlice = new HDWallet(seed1); //signer 1
            HDWallet walletBob = new HDWallet(seed0); //signer 2
            HDWallet walletSatoshi = new HDWallet(seed2); //does not sign
            HDWallet walletNico = new HDWallet(seed3);
            uint path = 0;

            ExtKey bobPrivKey = walletBob.GetPrivateKey(path);
            ExtPubKey keyBob = walletBob.GetPublicKey(path);

            Console.WriteLine("Gen Pub Address:\t" + keyBob.PubKey.GetAddress(Network.Main));
            Console.WriteLine("Gen Prv Address:\t" + bobPrivKey.PrivateKey.PubKey.GetAddress(Network.Main));
            Console.WriteLine("Gen Add Address:\t" + walletBob.GetBitcoinAddress(path));
            //ExtPubKey keyAlice = walletAlice.GetPublicKey(path);
            //ExtPubKey keySatoshi = walletAlice.GetPublicKey(path);
            Console.WriteLine();
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
