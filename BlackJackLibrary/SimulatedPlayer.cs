using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlackJackLibrary
{
    public class SimulatedPlayer : Player
    {
        public Int32 StopScore { get; set; }

        public SimulatedPlayer(String name, Int32 stopScore) : base(name)
        {
            this.UserName = name;
            this.PlayerCards = new List<Card>();
            this.StopScore = stopScore;
            this.HandValue = 0;
            this.CurrentBet = 0.00m;
            this.CurrentMoney = 500.00m;
            this.EndOfRound = false;
            this.WinCounter = 0;
            this.LossCounter = 0;
            this.DrawCounter = 0;
            this.BlackJackCounter = 0;
        }

        public override void Hit(Deck blackjackDeck)
        {
            Card card = blackjackDeck.DrawCard();
            this.PlayerCards.Add(card);
            if (card.ValueName.Equals("Ace"))
            {
                if (this.HandValue > 10)
                {
                    card.CardValue = 1;
                } else
                {
                    card.CardValue = 11;
                }
                this.CalculateHandValue();
            }
            else
            {
                this.CalculateHandValue();
            }
            //Set ace values to 1;
            Boolean ace = false;
            while (this.HandValue > 21 && !ace)
            {
                foreach (Card playerCard in this.PlayerCards)
                {
                    if (playerCard.ValueName.Contains("Ace"))
                    {
                        ace = true;
                        this.CalculateHandValue();
                    }
                    if (this.HandValue <= 21)
                    {
                        break;
                    }
                }
                if (!ace)
                {
                    ace = true;
                    break;
                }
            }
        }

        public override void Stand()
        {
            this.EndOfRound = true;
        }

        public override void DoubleDown(Deck blackJackDeck)
        {
            this.CurrentBet = this.CurrentBet * 2.50m; ;
            this.CurrentMoney -= this.CurrentBet;
            this.Hit(blackJackDeck);
            this.EndOfRound = true;
        }

        public override void Surrender()
        {
            this.EndOfRound = true;
        }

        public override void ResetHand()
        {
            this.EndOfRound = false;
            this.HandValue = 0;
            this.PlayerCards = new List<Card>();
        }

        public override void CalculateHandValue()
        {
            this.HandValue = 0;
            foreach (Card card in this.PlayerCards)
            {
                this.HandValue += card.CardValue;
            }
        }

        public override void BetMoney()
        {
            Decimal extraBet = this.CurrentBet;
            this.CurrentMoney -= extraBet;
            this.CurrentBet += extraBet;
        }
    }
}
