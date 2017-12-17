using NBitcoin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightningLibrary.Wallets
{
    public interface IWallet
    {
        WalletType Type { get; set; }
        ExtKey GetPrivateKey();
        ExtPubKey GetPublicKey();
        BitcoinAddress GetBitcoinAddress();
    }
}
