using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NBitcoin;
namespace LightningLibrary.Wallets
{
    /// <summary>
    /// Implements a BIP-32 HD Wallet
    /// </summary>
    public class HDWallet : IWallet
    {
        private byte[] _seed;
        private ExtKey _MasterPrivateKey;
        private ExtPubKey _MasterPublicKey; 
        private Network _Network; 
        public WalletType Type { get; set; }
        /// <summary>
        /// Uses the main network as default
        /// </summary>
        /// <param name="seed"></param>
        public HDWallet(string seed) : this(seed, Network.Main)
        {
            this.Type = WalletType.BIP32;
        }

        public HDWallet(string seed, Network network)
        {
            _seed = Encoding.Unicode.GetBytes(seed);            
            _MasterPrivateKey = new ExtKey(_seed);
            _MasterPublicKey = _MasterPrivateKey.Neuter();
            this._Network = network;
        }
        
        public ExtKey GetPrivateKey()
        {                
            return _MasterPrivateKey;            
        }

        public ExtKey GetPrivateKey(uint path)
        {
            //possibility 1
            string wifMaybe = _MasterPrivateKey.Derive(path).ToString(_Network);
            return ExtKey.Parse(wifMaybe);

            //possible 2
            //string wifStr = _MasterPublicKey.Derive(path).ToString(_Network);
            //return ExtKey.Parse(wifStr);
           

            //possibility 3
            //return _MasterPrivateKey.Derive(path);
        }


        public ExtPubKey GetPublicKey()
        {
            return _MasterPublicKey;
        }
        /// <summary>
        /// Derives an HD public key based on a path
        /// </summary>
        /// <param name="path">The BIP32 derived path</param>
        /// <returns></returns>
        public ExtPubKey GetPublicKey(uint path)
        {            
            string wifStr = _MasterPublicKey.Derive(path).ToString(_Network);
            return ExtPubKey.Parse(wifStr);
        }

        /// <summary>
        /// The master public key's address
        /// </summary>
        /// <returns>The public address</returns>
        public BitcoinAddress GetBitcoinAddress()
        {
            var hash = _MasterPublicKey.PubKey.Hash;
            var address = hash.GetAddress(_Network);
            return address;
        }

        /// <summary>
        /// The public key's address from a derived path.
        /// </summary>
        /// <param name="path">The derivation path</param>
        /// <returns>The public address</returns>
        public BitcoinAddress GetBitcoinAddress(uint path)
        {
            var hash = GetPublicKey(path).PubKey.Hash;
            var address = hash.GetAddress(_Network);
            return address;
        }
    }
}
