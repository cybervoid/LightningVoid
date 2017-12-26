using System;
using NBitcoin;

using Newtonsoft.Json;

namespace LightningLibrary.Objects
{
    public class ExplorerUnspent
    {

        [JsonProperty("address")]
        public string sAddress
        {
            set
            {
                _sAddress = value;
            }
        }

        [JsonIgnore]
        private string _sAddress;

        [JsonProperty("txid")]
        public string txid { get; set; }
        [JsonProperty("vout")]
        public int vout { get; set; }

        [JsonProperty("scriptPubKey")]
        public string sScriptPubKey
        {
            set
            {
                _sScriptPubKey = value;
            }
        }
        [JsonProperty("amount")]
        public decimal amount { get; set; }
        [JsonProperty("satoshis")]
        public ulong satoshis { get; set; }
        [JsonProperty("height")]
        public uint height { get; set; }

        [JsonProperty("confirmations")]
        public uint confirmations { get; set; }



        [JsonIgnore]
        private BitcoinAddress _address;
        [JsonIgnore]
        private string _sScriptPubKey;

        [JsonIgnore]
        public BitcoinAddress address 
        { 
            get
            {
                if (_address != null)
                    return _address;
                else if (!string.IsNullOrWhiteSpace(_sAddress))
                {
                    _address = BitcoinAddress.Create(_sAddress);
                    return _address;
                }
                else
                    return null;
            }
        }
        [JsonIgnore]
        private Script _scriptPubKey;
        [JsonIgnore]
        public Script scriptPubKey
        {
            get
            {
                if (_scriptPubKey != null)
                    return _scriptPubKey;
                else if (!string.IsNullOrWhiteSpace(_sScriptPubKey))
                {
                    _scriptPubKey = new Script(_sScriptPubKey);
                    return _scriptPubKey;
                }
                else
                    return null;
            }
        }

        [JsonIgnore]
        private Money _Amount;
        [JsonIgnore]
        public Money Amount 
        { 
            get
            {
                if (_Amount == null)
                    _Amount = new Money(this.satoshis);
                
                return _Amount;
            }
        }

        //public BitcoinScriptAddress GetBitcoinScriptAddress(Network network)
        //{
        //    return BitcoinScriptAddress.Create(sScriptPubKey, network);
        //}

        //public TxOut AsOutput()
        //{
            //return new TxOut(Amount, GetBitcoinScriptAddress(Network.TestNet));
        //    return new TxOut(Amount, scriptPubKey.PaymentScript);
        //}

        //public TxIn AsInput()
        //{
        //    return new TxIn(scriptPubKey);
        //}
    }
}
