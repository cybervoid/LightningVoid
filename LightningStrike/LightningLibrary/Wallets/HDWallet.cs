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
            throw new NotImplementedException("Not implemented, use GetPrivateKey(path)");
            return _MasterPrivateKey;            
        }

        public ExtKey GetPrivateKey(uint path)
        {
            return _MasterPrivateKey.Derive(path);
        }


        public ExtPubKey GetPublicKey()
        {
            throw new NotImplementedException("Not implemented, use GetPublicKey(path)");
            return _MasterPublicKey;
        }
        /// <summary>
        /// Derives an HD public key based on a path
        /// </summary>
        /// <param name="path">The BIP32 derived path</param>
        /// <returns></returns>
        public ExtPubKey GetPublicKey(uint path)
        {
            //string wifStr = _MasterPublicKey.Derive(path).ToString(_Network);
            //return ExtPubKey.Parse(wifStr);
            return _MasterPublicKey.Derive(path);
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

        public BitcoinAddress GetBitcoinAddress(uint path)
        {
            var hash = GetPublicKey(path).PubKey.Hash;
            var address = hash.GetAddress(_Network);
            return address;
        }

    }
}
