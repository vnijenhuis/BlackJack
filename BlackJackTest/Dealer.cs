using System;
using System.Collections.Generic;

namespace BlackJack
{
    public class Dealer
    {

        public List<Card> DealerCards { get; set; }
        public Int32 HandValue { get; set; }
        public Int32 OpenCardValue { get; set; }

        public Dealer()
        {
            this.DealerCards = new List<Card>();
            this.HandValue = 0;
            this.OpenCardValue = 0;
        }

        public void ResetHand()
        {
            this.HandValue = 0;
            this.OpenCardValue = 0;
            this.DealerCards = new List<Card>();
        }

        public void Hit(Deck blackjackDeck)
        {
            if (this.HandValue <= 16)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Card card = blackjackDeck.DrawCard();
                Console.WriteLine("Dealer Drawn card: " + card.CardValue + " : " + card.CardName);
                this.DealerCards.Add(card);
                if (card.ValueName.Equals("Ace"))
                {
                    if (this.HandValue >= 10)
                    {
                        card.CardValue = 11;
                    }
                    else
                    {
                        card.CardValue = 1;
                    }
                }
                this.HandValue += card.CardValue;
                Console.ResetColor();
            }
        }

        public void OpenCard(Deck blackjackDeck)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Open card");
            Card card = blackjackDeck.DrawCard();
            this.DealerCards.Add(card);
            if (card.ValueName.Equals("Ace"))
            {
                if (this.HandValue <= 10)
                {
                    card.CardValue = 11;
                }
                else
                {
                    card.CardValue = 1;
                }
            }
            this.HandValue += card.CardValue;
            this.OpenCardValue += card.CardValue;
            Console.WriteLine("The Dealers Open card is {0} with a value of {1}!", card.CardName, card.CardValue);
            Console.ResetColor();
        }
    }
}
