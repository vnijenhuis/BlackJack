using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlackJack
{
    public class Player
    {
        public String Name { get; set; }
        public List<Card> PlayerCards { get; set; }
        public Int32 HandValue { get; set; }
        public Decimal CurrentMoney { get; set; }
        public Decimal CurrentBet { get; set; }
        public Boolean EndOfRound { get; set; }

        public Player(String name)
        {
            this.Name = name;
            this.PlayerCards = new List<Card>();
            this.HandValue = 0;
            this.CurrentMoney = 500.00m;
            this.CurrentBet = 0.00m;
            this.EndOfRound = false;
        }

        public void Hit(Deck blackjackDeck)
        {
            Card card = blackjackDeck.DrawCard();
            this.PlayerCards.Add(card);
            if (card.ValueName.Equals("Ace"))
            {
                Console.WriteLine("Player {0} has drawn {1}! Do you want the value to be 1 or 11?", this.Name, card.CardName);
                card.CardValue = int.Parse(Console.ReadLine());
                this.HandValue += card.CardValue;
            }
            else
            {
                Console.WriteLine("Player {0} has drawn {1} with a value of {2}!", this.Name, card.CardName, card.CardValue);
                this.HandValue += card.CardValue;
            }
        }

        public void Stand()
        {
            throw new NotSupportedException("Stand function is not supported in this version");
        }

        public void DoubleDown()
        {
            throw new NotSupportedException("DoubleDown function is not supported in this version");
        }

        public void Surrender()
        {
            throw new NotSupportedException("Surrender function is not supported in this version");
        }

        public void ResetHand()
        {
            this.HandValue = 0;
            this.PlayerCards = new List<Card>();
        }
    }
}
