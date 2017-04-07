using System;
using static BlackJackLibrary.CardValue;

namespace BlackJackLibrary
{
    public class Card
    {
        public Suit CardSuit { get; private set; }
        public Rank CardRank { get; set; }
        public Int32 CardValue { get; set; }
        public String CardName { get; private set; }
        public Card(Suit suit, Rank rank)
        {
            this.CardSuit = suit;
            this.CardRank = rank;
            this.CardValue = (Int32)rank;
            this.CardName = rank + " of " + suit;
        }
    }
}
