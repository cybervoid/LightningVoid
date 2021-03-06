﻿using System;
using System.Collections.Generic;
using System.Linq;
using NBitcoin;

namespace LightningLibrary.Utilities
{
    public class MultiSig
    {
        private Network _Network;

        public MultiSig(Network network)
        {
            this._Network = network;
        }

        #region The combined public key and pub key script
        /// <summary>
        /// The equivalent of a public key
        /// </summary>
        /// <returns>The public key script.</returns>
        /// <param name="minimumSignatures">Minimum signatures.</param>
        /// <param name="keys">Keys.</param>
        public Script GetPublicKeyScript(int minimumSignatures, PubKey[] keys)
        {
            //xxxx here
            var scriptPubKey = PayToMultiSigTemplate.Instance.GenerateScriptPubKey(minimumSignatures, keys);
            return scriptPubKey;
        }

        public Script GetPublicKeyScript(int minimumSignatures, Key[] keys)
        {
            List<PubKey> pubKeys = new List<PubKey>();
            for (int i = 0; i < keys.Length; i++)
            {
                pubKeys.Add(keys[i].PubKey);
            }
            return GetPublicKeyScript(minimumSignatures, pubKeys.ToArray());
        }
        /// <summary>
        /// Gets the P2SH address from an array of extended private keys of the claimants. 
        /// </summary>
        /// <param name="minimumSignatures">The minimum number of required signatures to send coins</param>
        /// <param name="keys">the extended private keys</param>
        /// <returns>A P2SH address</returns>
        public BitcoinScriptAddress GetP2SHAddress(int minimumSignatures, ExtKey[] keys)
        {
            List<Key> k = new List<Key>();
            for (int i = 0; i < keys.Length; i++)
            {
                k.Add(keys[i].PrivateKey);
            }
            return GetP2SHAddress(minimumSignatures, k.ToArray());
        }

        /// <summary>
        /// Gets the P2SH address from the private keys of the claimants. 
        /// </summary>
        /// <param name="minimumSignatures">The minimum number of required signatures to send coins</param>
        /// <param name="keys">the private keys</param>
        /// <returns>A P2SH address</returns>
        public BitcoinScriptAddress GetP2SHAddress(int minimumSignatures, Key[] keys)
        {
            Script pubKeyScript = GetPublicKeyScript(minimumSignatures, keys);
            var address = pubKeyScript.WitHash.GetAddress(_Network);
            var p2sh = address.GetScriptAddress();
            return p2sh;
        }

        /// <summary>
        /// Gets the P2SH address from the Public keys of the claimants. 
        /// </summary>
        /// <param name="minimumSignatures">The minimum number of required signatures to send coins</param>
        /// <param name="keys">the public keys</param>
        /// <returns>A P2SH address</returns>
        public BitcoinScriptAddress GetP2SHAddress(int minimumSignatures, PubKey[] keys)
        {
            Script pubKeyScript = GetPublicKeyScript(minimumSignatures, keys);
            var address = pubKeyScript.WitHash.GetAddress(_Network);
            var p2sh = address.GetScriptAddress();
            return p2sh;
        }


        public void Full(int minimumSignatures, Key[] keys)
        {
            List<PubKey> pubKeys = new List<PubKey>();
            for (int i = 0; i < keys.Length; i++)
            {
                pubKeys.Add(keys[i].PubKey);
            }
            Script pubKeyScript = PayToMultiSigTemplate.Instance.GenerateScriptPubKey(minimumSignatures, pubKeys.ToArray());

            var address = pubKeyScript.WitHash.GetAddress(_Network);
            var p2sh = address.GetScriptAddress();
            Console.WriteLine("Send money here: " + p2sh.ToString());


        }
        #endregion

        #region Multi-Sig Segwit Address
        public BitcoinScriptAddress GetP2SHAddress(Key key)
        {
            //OLD
            //This gives you a Bech32 address (currently not really interoperable in wallets, so you need to convert it into P2SH)
            var address = key.PubKey.WitHash.GetAddress(_Network);
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

        #endregion

        #region Multisig Segwit Redeem Script

        public Script GetRedeemScript(int minimumSignatures, ExtKey[] keys)
        {
            List<PubKey> pubKeys = new List<PubKey>();
            for (int i = 0; i < keys.Length; i++)
            {
                pubKeys.Add(keys[i].PrivateKey.PubKey);
            }
            return GetRedeemScript(minimumSignatures, pubKeys.ToArray());
        }

        public Script GetRedeemScript(int minimumSignatures, Key[] keys)
        {
            List<PubKey> pubKeys = new List<PubKey>();
            for (int i = 0; i < keys.Length; i++)
            {
                pubKeys.Add(keys[i].PubKey);
            }
            return GetRedeemScript(minimumSignatures, pubKeys.ToArray());
        }

        public Script GetRedeemScript(int minimumSignatures, PubKey[] keys)
        {
            return GetPublicKeyScript(minimumSignatures, keys);
           
        }

        public Script CreateMultiSigScript(PubKey[] keys, int requiredSignatures = 1)
        {
            if (keys.Length == 0)
                throw new ArgumentException("Required signatures cannot be 0.");
            Script redeemScript = PayToMultiSigTemplate.Instance.GenerateScriptPubKey(requiredSignatures, keys);
            return redeemScript;
        }
        #endregion
    }
}
