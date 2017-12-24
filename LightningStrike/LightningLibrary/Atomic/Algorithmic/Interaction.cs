using System;
namespace LightningLibrary.Atomic.Algorithmic
{
    public class Interaction
    {
        public Interaction()
        {
        }

        public void Run()
        {
            Random random = new Random();
            Party partyA = new Party(random, "A - Party");
            partyA.SetRandom();
            Party partyB = new Party(random, "B - Party");
            decimal x = 0.5m;
            ATransaction tx1 = partyA.CreateTransaction(x, CoinType.Bitcoin, partyB); //new ATransaction(x, partyA);
            tx1.Hasher();
            partyA.Send(tx1);


            ReceivedTxHandler(partyB);

            if(VerifyReceivedTx(partyA, ref tx1)== false)
            {
                return;
            }

            partyA.Transactions.Add(tx1); //do not yet broadcast.
            //party A creates transaction 2, sends to self
            decimal w = 0.2m; //is this equal to x or less than x?
            ATransaction tx2 = partyA.CreateTransaction(w, CoinType.Bitcoin, partyA, 0);
            partyA.Send(tx2);

            ReceivedTxHandler(partyB);
            if(VerifyReceivedTx(partyA, ref tx2))
            {
                partyA.Broadcast(tx2);
            }
        }

        public void ReceivedTxHandler(Party party)
        {
            ATransaction receivedTx1 = party.Receive();
            if (party.Accepted(receivedTx1))
            {
                receivedTx1.Sign(party);
                party.Send(receivedTx1);
            }
            else
                receivedTx1.Reject();
            
        }

        public bool VerifyReceivedTx(Party party, ref ATransaction sentTransaction)
        {
            
            var newTx1 = party.Receive();
            if (sentTransaction.Verify(newTx1) == true)
            {
                sentTransaction = newTx1;
                sentTransaction.Valid = true;
                return true;
            }
            else
                sentTransaction.Valid = false;

            return false;

        }
    }
}
