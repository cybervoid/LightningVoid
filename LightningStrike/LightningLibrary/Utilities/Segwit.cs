using System;
using System.Collections.Generic;
using System.Linq;
using NBitcoin;
namespace LightningLibrary.Utilities
{
    /// <summary>
    /// A segwit functions manager
    /// </summary>
    /// <References>
    /// https://programmingblockchain.gitbooks.io/programmingblockchain/content/other_types_of_ownership/p2sh_pay_to_script_hash.html
    /// https://programmingblockchain.gitbooks.io/programmingblockchain/content/other_types_of_ownership/p2w_over_p2sh.html
    /// </References>
    public class Segwit
    {
        private Network _Network;
        public Segwit(Network network)
        {
            this._Network = network;
        }

        //public BitcoinAddress GetSegwitAddress(ExtKey key)
        //{
        //    return GetSegwitAddress(key.PrivateKey);
        //}

        //public BitcoinAddress GetSegwitAddress(Key key)
        //{
        //    return GetSegwitAddress(new Key[] { key }, 1);
        //}

        //public BitcoinAddress GetSegwitAddress(Key[] key, int signatureMinimum = 1)
        //{

        //    if (signatureMinimum <= 0)
        //        throw new ArgumentException("1 signature must be required");
        //    if (key.Length == 0)
        //        throw new ArgumentException("1 key must be provided");

        //    List<PubKey> pubKeys = new List<PubKey>();

        //    for (int i = 0; i < key.Length; i++)
        //    {
        //        pubKeys.Add(key[i].PubKey);
        //    }
        //    var script = CreateMultisigScript(pubKeys.ToArray(),signatureMinimum);
        //    return script.Hash.GetAddress(_Network);
        //}

        //public BitcoinAddress GetSegwitAddress(ExtKey[] key, int requiredSignatures = 1)
        //{
        //    List<Key> keys = new List<Key>();

        //    for (int i = 0; i < key.Length; i++)
        //    {
        //        if(key[i] != null)
        //          keys.Add(key[i].PrivateKey);
        //    }
        //    return GetSegwitAddress(key.ToArray(), requiredSignatures);
        //}

        //public Script GetSegwitScriptPubKey(ExtKey key)
        //{
        //    return GetSegwitScriptPubKey(key.PrivateKey);
        //}

        //public Script GetSegwitScriptPubKey(Key key)
        //{
        //    //Replacing the ScriptPubKey by its P2SH equivalent.
        //    return key.PubKey.ScriptPubKey.WitHash.ScriptPubKey.Hash.ScriptPubKey;
        //}

        ///// <summary>
        ///// Multi sign
        ///// </summary>
        ///// <returns>The segwit script.</returns>
        ///// <param name="keys">Keys.</param>
        ///// <param name="requiredSignatures">Required signatures.</param>
        //public Script CreateMultisigScript(PubKey[] keys, int requiredSignatures = 1)
        //{
        //    if (keys.Length == 0)
        //        throw new ArgumentException("Required signatures cannot be 0.");
        //    Script redeemScript = PayToMultiSigTemplate.Instance.GenerateScriptPubKey(requiredSignatures, keys);

        //    return redeemScript;
        //}

        /// <summary>
        /// Gets the Segwit backward compatible address. (Send money to this address)
        /// </summary>
        /// <returns>The p2sh address.</returns>
        public BitcoinScriptAddress GetP2SHAddress(Key k)
        {
            //This gives you a Bech32 address (currently not really interoperable in wallets, so you need to convert it into P2SH)
            var address = k.PubKey.WitHash.GetAddress(_Network);
            var p2sh = address.GetScriptAddress(); // This is P2SH address.
            return p2sh;
        }
        /// <summary>
        /// Gets the Segwit backward compatible address. (Send money to this address)
        /// </summary>
        /// <returns>The p2sh address.</returns>
        public BitcoinScriptAddress GetP2SHAddress(ExtKey key)
        {
            return GetP2SHAddress(key.PrivateKey);
        }

        public Script GetRedeemScript(ExtKey key)
        {
            return GetRedeemScript(key.PrivateKey);
        }

        public Script GetRedeemScript(Key key)
        {
             return key.PubKey.WitHash.ScriptPubKey;
        }

        public ScriptCoin GetSegwitCoin(Transaction previousTransactionReceived, Script redeemScript)
        {
            ScriptCoin coin = previousTransactionReceived.Outputs.AsCoins().First().ToScriptCoin(redeemScript);
            return coin;
        }
    }
}
