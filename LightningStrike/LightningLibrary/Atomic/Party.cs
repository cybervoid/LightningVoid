using System;
using System.Collections.Generic;
using NBitcoin.BouncyCastle.Math;
namespace LightningLibrary.Atomic
{
    public class Party
    {
        public BigInteger RandomNumber { get; set; }
        public Random Random { get; set; }
        public List<ATransaction> Transactions { get; set; }
        public string PsuedoSignature { get; set; }
        public string PsudoAddress { get; set; }
        //public event Receiver;
        public Party()
        {
        }

        public Party(Random random, string name)
        {
            this.Random = random;
            this.Transactions = new List<ATransaction>();
            this.PsudoAddress = name + " address";
            this.PsudoAddress = name + " signature";
            //this.Receiver += Receive_Transaction;
        }

        internal bool Accepted(ATransaction receivedTx1)
        {
            throw new NotImplementedException();
        }

        internal ATransaction Receive()
        {
            //receive the transaction with an event
            return new ATransaction();
            //throw new NotImplementedException();
        }

        internal void Send(ATransaction tx1)
        {
            throw new NotImplementedException();
        }

        internal void Broadcast(ATransaction tx2)
        {
            
            throw new NotImplementedException();
        }

        public void SetRandom()
        {
            RandomNumber = new BigInteger(this.Random.Next(2, 20), this.Random);
        }


        public ATransaction CreateTransaction(decimal amount, CoinType coinType,Party toParty, int? index = null)
        {
            ATransaction tx;
            if (index != null)
            {
                tx = new ATransaction(amount, toParty, Transactions[0]);
            }
            else
            {
                tx = new ATransaction(amount, toParty);
            }
            tx.Amount = amount;
            tx.CoinType = coinType;
            Transactions.Add(tx);
            return tx;
        }


    }
}
