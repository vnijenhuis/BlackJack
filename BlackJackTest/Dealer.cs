using System;
using System.Collections.Generic;

namespace BlackJack
{
    public class Dealer
    {

        public Card DealerFacedownCard { get; set; }
        public List<Card> DealerOpenCards { get; set; }
        public Int32 FaceDownCardValue { get; set; } 
        public Int32 TotalCardValue { get; set; }
        public Int32 OpenCardValue { get; set; }
        public Int32 WinCounter { get; set; }
        public Int32 LossCounter { get; set; }
        public Int32 DrawCounter { get; set; }
        public Int32 BlackJackCounter { get; set; }
        public Decimal CasinoEarnings { get; set; }

        public Dealer()
        {
            this.DealerOpenCards = new List<Card>();
            this.DealerFacedownCard = new Card("Empty", "", 0);
            this.CasinoEarnings = 0.00m;
            this.FaceDownCardValue = 0;
            this.OpenCardValue = 0;
            this.WinCounter = 0;
            this.BlackJackCounter = 0;
        }

        public void ResetHand()
        {
            this.OpenCardValue = 0;
            this.TotalCardValue = 0;
        }

        public void OpenCard(Deck blackjackDeck)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Card drawnCard = blackjackDeck.DrawCard();
            if (drawnCard.ValueName.Equals("Ace"))
            {
                if (this.TotalCardValue <= 10)
                {
                    drawnCard.CardValue = 11;
                }
                else
                {
                    drawnCard.CardValue = 1;
                }
            }
            if (DealerOpenCards.Count == 1 && this.DealerFacedownCard.CardValue == 0)
            {
                this.DealerFacedownCard = drawnCard;
                this.FaceDownCardValue = DealerFacedownCard.CardValue;
                this.CalculateTotalCardValue();
                Console.WriteLine(this.DealerOpenCards.ToString() + "    " + this.DealerFacedownCard.CardValue);
                Console.WriteLine("The Dealers has drawn a card and has put it face down.");
            }
            else if (DealerOpenCards.Count >= 1)
            {
                Card newOpenCard = this.DealerFacedownCard;
                this.DealerFacedownCard = drawnCard;
                this.FaceDownCardValue = DealerFacedownCard.CardValue;
                this.DealerOpenCards.Add(newOpenCard);
                this.CalculateTotalCardValue();
                Console.WriteLine(this.DealerOpenCards + "    " + this.DealerFacedownCard);
                Console.WriteLine("The Dealers Open card is a {0} with a value of {1} with a total Open card value of {2}!", newOpenCard.CardName, newOpenCard.CardValue, this.OpenCardValue);
            }
            else
            {
                DealerOpenCards.Add(drawnCard);
                this.CalculateTotalCardValue();
                Console.WriteLine(this.DealerOpenCards + "    " + this.DealerFacedownCard);
                Console.WriteLine("The Dealers first Open card is a {0} with a value of {1}!", drawnCard.CardName, drawnCard.CardValue);
            }
            Console.ResetColor();
        }

        public void SimulateOpenCard(Deck blackjackDeck)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Card drawnCard = blackjackDeck.DrawCard();
            if (drawnCard.ValueName.Equals("Ace"))
            {
                if (this.TotalCardValue <= 10)
                {
                    drawnCard.CardValue = 11;
                }
                else
                {
                    drawnCard.CardValue = 1;
                }
            }
            if (DealerOpenCards.Count == 1)
            {
                this.DealerFacedownCard = drawnCard;
                this.FaceDownCardValue = DealerFacedownCard.CardValue;
                Console.WriteLine(this.DealerOpenCards + "    " + this.DealerFacedownCard);
            }
            else if (DealerOpenCards.Count >= 1)
            {
                Card newOpenCard = this.DealerFacedownCard;
                this.DealerFacedownCard = drawnCard;
                this.FaceDownCardValue = DealerFacedownCard.CardValue;
                this.DealerOpenCards.Add(newOpenCard);
                Console.WriteLine(this.DealerOpenCards + "    " + this.DealerFacedownCard);
            }
            else
            {
                DealerOpenCards.Add(drawnCard);
                Console.WriteLine(this.DealerOpenCards + "    " + this.DealerFacedownCard);
            }
            this.CalculateTotalCardValue();
            Console.ResetColor();
        }

        public void CalculateTotalCardValue()
        {
            this.TotalCardValue = this.FaceDownCardValue;
            foreach (Card card in this.DealerOpenCards)
            {
                this.TotalCardValue += card.CardValue;
            }
            this.CalculateOpenHandValue();
        }

        public void CalculateOpenHandValue()
        {
            this.OpenCardValue = 0;
            foreach (Card card in this.DealerOpenCards)
            {
                this.OpenCardValue += card.CardValue;
            }
        }

        public void RevealAllCards()
        {
            foreach (Card openCard in this.DealerOpenCards)
            {
                Console.WriteLine("The the Open card is a {0} with a value of {1}.", openCard.CardName, openCard.CardValue);
            }
            Console.WriteLine("The Face down card is a {0} with a value of {1}.", this.DealerFacedownCard.CardName, this.FaceDownCardValue);
            Console.WriteLine("The Dealer has a total card value of {0}!", this.TotalCardValue);
        }

        internal void NaturalBlackJackPayout(Player player)
        {
            Decimal payout = player.CurrentBet * 1.50m;
            Console.WriteLine("Currentbet {0} gives payout of {1}", player.CurrentBet, payout);
            this.CasinoEarnings -= payout;
            player.CurrentMoney += payout;
        }
    }
}
