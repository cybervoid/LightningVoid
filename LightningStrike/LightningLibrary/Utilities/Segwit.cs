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
    /// https://bitcoincore.org/en/segwit_wallet_dev
    /// </References>
    public class Segwit
    {
        private Network _Network;
        public Segwit(Network network)
        {
            this._Network = network;
        }


        /// <summary>
        /// Gets the Segwit backward compatible address. (Send money to this address)
        /// </summary>
        /// <returns>The p2sh address.</returns>
        public BitcoinScriptAddress GetP2SHAddress(Key key)
        {
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
