using System;
using System.Collections.Generic;

namespace BlackJackLibrary
{
    public class Dealer
    {

        public Card DealerFacedownCard { get; set; }
        public List<Card> DealerOpenCards { get; set; }
        public Decimal CasinoEarnings { get; set; }
        public List<Decimal> PayoutMoney { get; set; }
        public Int32 FaceDownCardValue { get; set; } 
        public Int32 TotalCardValue { get; set; }
        public Int32 OpenCardValue { get; set; }
        public Int32 WinCounter { get; set; }
        public Int32 LossCounter { get; set; }
        public Int32 DrawCounter { get; set; }
        public Int32 BlackJackCounter { get; set; }
        private Boolean HardMode { get; set; }

        public Dealer()
        {
            this.DealerOpenCards = new List<Card>();
            this.DealerFacedownCard = new Card("Empty", "", 0);
            this.CasinoEarnings = 500000000.00m;
            this.FaceDownCardValue = 0;
            this.OpenCardValue = 0;
            this.WinCounter = 0;
            this.BlackJackCounter = 0;
            this.PayoutMoney = new List<Decimal>();
            this.HardMode = false;
        }

        public void ResetHand()
        {
            this.PayoutMoney = new List<Decimal>();
            this.DealerOpenCards = new List<Card>();
            this.DealerFacedownCard = new Card("Empty", "", 0);
            this.FaceDownCardValue = 0;
            this.OpenCardValue = 0;
            this.TotalCardValue = 0;
        }

        public void OpenCard(Deck blackjackDeck)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Card drawnCard = blackjackDeck.DrawCard();
            if (this.HardMode)
            {
                drawnCard.ValueName = "Ace";
            }
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
                Console.WriteLine("The Dealers has drawn a card and has put it face down.");
            }
            else if (DealerOpenCards.Count >= 1)
            {
                Card newOpenCard = this.DealerFacedownCard;
                this.DealerFacedownCard = drawnCard;
                this.FaceDownCardValue = DealerFacedownCard.CardValue;
                this.DealerOpenCards.Add(newOpenCard);
                this.CalculateTotalCardValue();
                Console.WriteLine("The Dealers Open card is a {0} with a value of {1} with a total Open card value of {2}!", newOpenCard.CardName, newOpenCard.CardValue, this.OpenCardValue);
            }
            else
            {
                DealerOpenCards.Add(drawnCard);
                this.CalculateTotalCardValue();
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
            }
            else if (DealerOpenCards.Count >= 1)
            {
                Card newOpenCard = this.DealerFacedownCard;
                this.DealerFacedownCard = drawnCard;
                this.FaceDownCardValue = DealerFacedownCard.CardValue;
                this.DealerOpenCards.Add(newOpenCard);
            }
            else
            {
                DealerOpenCards.Add(drawnCard);
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
            Console.ForegroundColor = ConsoleColor.Red;
            foreach (Card openCard in this.DealerOpenCards)
            {
                Console.WriteLine("The the Open card is a {0} with a value of {1}.", openCard.CardName, openCard.CardValue);
            }
            Console.WriteLine("The Face down card is a {0} with a value of {1}.", this.DealerFacedownCard.CardName, this.FaceDownCardValue);
            Console.WriteLine("The Dealer has a total card value of {0}!", this.TotalCardValue);
            Console.ResetColor();
        }

        public virtual void CollectBets(Decimal payout)
        {
            Console.WriteLine("*****CollectBets*****");
            Console.WriteLine("Current casino earnings {0}, current payout {1}.", this.CasinoEarnings, payout);
            this.CasinoEarnings = (this.CasinoEarnings + payout);
            Console.WriteLine("Current casino earnings {0}.", this.CasinoEarnings);
        }

        public virtual void PayPlayerBet(Player player)
        {
            Console.WriteLine("*****PayPlayerBet***** {0}", player.Name);
            Console.WriteLine("Current casino earnings {0}, current payout {1}.", this.CasinoEarnings, player.CurrentBet);
            this.CasinoEarnings -= player.CurrentBet;
            Console.WriteLine("Current casino earnings {0}.", this.CasinoEarnings);
            Console.WriteLine("Current bet {0}, current money {1}.", player.CurrentBet, player.CurrentMoney);
            player.CurrentMoney += (player.CurrentBet * 2);
            player.CurrentBet = 0.00m;
            Console.WriteLine("Current bet {0}, current money {1}.", player.CurrentBet, player.CurrentMoney);
        }


        public virtual void ReturnPlayerBet(Player player)
        {
            Console.WriteLine("*****ReturnPlayerBet***** {0}", player.Name);
            Console.WriteLine("Current bet {0}, current money {1}.", player.CurrentBet, player.CurrentMoney);
            player.CurrentMoney += player.CurrentBet;
            player.CurrentBet = 0.00m;
            Console.WriteLine("Current bet {0}, current money {1}.", player.CurrentBet, player.CurrentMoney);
        }

        public virtual void PayBlackjackPlayerBet(Player player, Decimal playerBlackjackPayout)
        {
            Console.WriteLine("*****PayBlackjackPlayerBet***** {0}", player.Name);
            Console.WriteLine("Current casino earnings {0}, current payout {1}.", this.CasinoEarnings, player.CurrentBet);
            this.CasinoEarnings -= player.CurrentBet;
            Console.WriteLine("Current casino earnings {0}.", this.CasinoEarnings);
            Console.WriteLine("Current bet {0}, current money {1}.", player.CurrentBet, player.CurrentMoney);
            Decimal finalPayout = (player.CurrentBet + playerBlackjackPayout);
            player.CurrentMoney += finalPayout;
            player.CurrentBet = 0.00m;
            Console.WriteLine("Current bet {0}, current money {1}.", player.CurrentBet, player.CurrentMoney);
        }

        public void NaturalBlackJackPayout(Player player)
        {
            Decimal payout = (player.CurrentBet * 0.50m);
            Console.WriteLine("*****NaturalBlackJackPayout***** {0}", player.Name);
            Console.WriteLine("Current casino earnings {0}, current payout {1}.", this.CasinoEarnings, player.CurrentBet);
            this.CasinoEarnings -= payout;
            Console.WriteLine("Current casino earnings {0}.", this.CasinoEarnings);
            Console.WriteLine("Current bet {0}, current money {1}.", player.CurrentBet, player.CurrentMoney);
            player.CurrentMoney += payout;
            Console.WriteLine("Current bet {0}, current money {1}.", player.CurrentBet, player.CurrentMoney);
        }
    }
}
