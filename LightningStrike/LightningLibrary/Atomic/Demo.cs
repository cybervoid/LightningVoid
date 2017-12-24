using System;
using NBitcoin.BouncyCastle.Math;

namespace LightningLibrary.Atomic
{
    /// <summary>
    /// https://en.bitcoin.it/wiki/Atomic_cross-chain_trading
    /// </summary>

    public class Demo
    {
        public Demo()
        {
        }

        public void Run()
        {
            //Person A picks a random number x
            Person personA;
            Random random = new Random();
            personA = new Person(random);
        }
    }

    public class Person
    {
        public BigInteger RandomNumber { get; set; }
        public Random Random { get; set; }
        public AtomicTransaction Transaction { get; set; }
        public Person(Random random)
        {
            this.Random = random;
            this.AtomicTransaction = new AtomicTransaction();
        }

        public void GetRandom()
        {
            RandomNumber = new BigInteger(this.Random.Next(8, 60), this.Random);
        }
    }

    public class AtomicTransaction 
    {
        public Person Counterparty { get; set; }
        //decimal Amount 
        public AtomicTransaction()
        {

        }

        public AtomicTransaction(Person counterParty)
        {
            this.Counterparty = counterParty;
        }
    }
}
