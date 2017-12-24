using System;
using System.Collections.Generic;

namespace LightningLibrary.Atomic.Algorithmic
{
    public class ATransaction
    {
        public decimal Amount { get; set; }
        public CoinType CoinType { get; set; }
        public Party ToParty { get; set; }
        public Party FromParty { get; set; }
        public string Script { get; set; }
        public List<string> Signatures { get; set; }
        public bool Valid { get; set; } = false;
        public ATransaction InputTransaction { get; set; }

        public ATransaction()
        {
            this.Signatures = new List<string>();
        }

        public ATransaction(decimal amount, Party fromParty) : this()
        {
            this.Amount = amount;
            this.FromParty = fromParty;
        }

        internal void Hasher()
        {
            throw new NotImplementedException();
        }

        public ATransaction(decimal amount, Party fromParty, Party toParty) : this(amount, fromParty)
        {
            this.ToParty = toParty;
        }

        public ATransaction(decimal amount, Party fromParty, ATransaction inputTransaction) : this(amount, fromParty)
        {
            this.InputTransaction = inputTransaction;
        }


        internal bool Verify(ATransaction newTx1)
        {
            return true;
            //throw new NotImplementedException();
        }

        internal void Reject()
        {
            //Todo reject transactions
            //throw new NotImplementedException();
        }

        public void Sign(Party signingParty)
        {
            Signatures.Add(signingParty.PsuedoSignature);
        }
    }
}
