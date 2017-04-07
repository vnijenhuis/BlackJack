using System;
using System.Collections.Generic;
using System.Linq;
using static BlackJackLibrary.CardValue;

namespace BlackJackLibrary
{
    public class Deck
    {
        public List<Card> CardDeck { get; set; }

        public Deck(Int32 numberOfDecks)
        {
            CardDeck = new List<Card>();
            foreach (Suit suitItem in Enum.GetValues(typeof(Suit))) {
                foreach (Rank rankItem in Enum.GetValues(typeof(Rank)))
                {
                    Card card = new Card(suitItem, rankItem);
                    CardDeck.Add(card);
                }
            }
            ShuffleDeck();
        }

        public void ShuffleDeck()
        {
            Random rand = new Random();
            CardDeck = CardDeck.OrderBy(card => rand.Next()).Select(card => card).ToList();
        }

        public Card DrawCard()
        {
            Card currentCard = CardDeck[0];
            CardDeck.Remove(currentCard);
            return currentCard;
        }
    }
}
