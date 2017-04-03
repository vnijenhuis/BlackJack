using System;
using System.Collections.Generic;
using System.Linq;

namespace BlackJack
{
    public class Deck
    {
        public List<Card> CardDeck { get; set; }

        public Deck(Int32 numberOfDecks)
        {
            CardDeck = new List<Card>();
            for (int i = 0; i < numberOfDecks; i++)
            {
                List<String> types = new List<String> { "Hearts", "Diamonds", "Clubs", "Spades" };
                foreach (String type in types)
                {
                    CardDeck.Add(new Card(type, "Ace", 1));
                    CardDeck.Add(new Card(type, "Two", 2));
                    CardDeck.Add(new Card(type, "Three", 3));
                    CardDeck.Add(new Card(type, "Four", 4));
                    CardDeck.Add(new Card(type, "Five", 5));
                    CardDeck.Add(new Card(type, "Six", 6));
                    CardDeck.Add(new Card(type, "Seven", 7));
                    CardDeck.Add(new Card(type, "Eight", 8));
                    CardDeck.Add(new Card(type, "Nine", 9));
                    CardDeck.Add(new Card(type, "Ten", 10));
                    CardDeck.Add(new Card(type, "Jack", 10));
                    CardDeck.Add(new Card(type, "Queen", 10));
                    CardDeck.Add(new Card(type, "Ace", 10));
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
