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
        public Decimal MinimumBet { get; set; }
        public Decimal MaximumBet { get; set; }
        public Int32 FaceDownCardValue { get; set; } 
        public Int32 TotalCardValue { get; set; }
        public Int32 OpenCardValue { get; set; }
        public Int32 WinCounter { get; set; }
        public Int32 LossCounter { get; set; }
        public Int32 DrawCounter { get; set; }
        public Int32 BlackJackCounter { get; set; }
        public Int32 DealerScoreLimit { get; set; }
        private Int32 Difficultly { get; set; }

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
            this.Difficultly = 0;
            this.DealerScoreLimit = 0;
        }

        public Dealer(Int32 difficultly)
        {
            this.DealerOpenCards = new List<Card>();
            this.DealerFacedownCard = new Card("Empty", "", 0);
            this.CasinoEarnings = 500000000.00m;
            this.FaceDownCardValue = 0;
            this.OpenCardValue = 0;
            this.WinCounter = 0;
            this.BlackJackCounter = 0;
            this.PayoutMoney = new List<Decimal>();
            this.Difficultly = difficultly;
            this.DetermineDealerScoreLimit();
        }

        private void DetermineDealerScoreLimit()
        {
            if (this.Difficultly == 100)
            {
                this.MinimumBet = 100.00m;
                this.MaximumBet = 1000000.00m;
                this.DealerScoreLimit = 20;
            }
            else if (this.Difficultly == 75)
            {
                this.MinimumBet = 100.00m;
                this.MaximumBet = 5000.00m;
                this.DealerScoreLimit = 19;
            }
            else if (this.Difficultly == 50)
            {
                this.MinimumBet = 50.00m;
                this.MaximumBet = 2500.00m;
                this.DealerScoreLimit = 18;
            } else if (this.Difficultly == 20)
            {
                this.MinimumBet = 25.00m;
                this.MaximumBet = 750.00m;
                this.DealerScoreLimit = 17;
            } else
            {
                this.MinimumBet = 2.00m;
                this.MaximumBet = 500.00m;
                this.DealerScoreLimit = 16;
            }
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
            Card drawnCard2 = blackjackDeck.DrawCard();
            if (this.Difficultly >= 100 && OpenCardValue == 0)
            {
                blackjackDeck.CardDeck.Add(drawnCard);
                drawnCard.ValueName = "Ace";
                drawnCard.ValueName = "11";
                blackjackDeck.CardDeck.Remove(drawnCard);
            }
            else if (this.Difficultly >= 100 && OpenCardValue >=10)
            {
                if ((drawnCard.CardValue + OpenCardValue) > 21 && drawnCard.CardValue >= 7)
                {
                    Card extraCard = blackjackDeck.DrawCard();
                    blackjackDeck.CardDeck.Add(drawnCard);
                    blackjackDeck.CardDeck.Add(drawnCard2);
                    drawnCard = extraCard;
                    blackjackDeck.ShuffleDeck();
                } else if ((drawnCard2.CardValue + OpenCardValue) > 21 && drawnCard2.CardValue >= 7)
                {
                    Card extraCard = blackjackDeck.DrawCard();
                    blackjackDeck.CardDeck.Add(drawnCard);
                    blackjackDeck.CardDeck.Add(drawnCard2);
                    drawnCard = extraCard;
                    blackjackDeck.ShuffleDeck();
                }
            }
            else if (this.Difficultly >= 75 && OpenCardValue == 0)
            {
                blackjackDeck.CardDeck.Add(drawnCard);
                drawnCard.ValueName = "Jack";
                drawnCard.ValueName = "10";
                blackjackDeck.CardDeck.Remove(drawnCard);
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
            this.CasinoEarnings = (this.CasinoEarnings + payout);
        }

        public virtual void PayPlayerBet(Player player)
        {
            this.CasinoEarnings -= player.CurrentBet;
            player.CurrentMoney += (player.CurrentBet * 2);
            player.CurrentBet = 0.00m;
        }


        public virtual void ReturnPlayerBet(Player player)
        {
            player.CurrentMoney += player.CurrentBet;
            player.CurrentBet = 0.00m;
        }

        public virtual void PayBlackjackPlayerBet(Player player, Decimal playerBlackjackPayout)
        {
            this.CasinoEarnings -= player.CurrentBet;
            Decimal finalPayout = (player.CurrentBet + playerBlackjackPayout);
            player.CurrentMoney += finalPayout;
            player.CurrentBet = 0.00m;
            player.WinCounter += 1;
        }

        public void NaturalBlackJackPayout(Player player)
        {
            Decimal payout = (player.CurrentBet * 0.50m);
            this.CasinoEarnings -= payout;
            player.CurrentMoney += payout;
            player.WinCounter += 1;
        }
    }
}
