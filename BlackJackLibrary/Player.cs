using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlackJackLibrary
{
    public class Player
    {
        public String UserName { get; set; }
        public List<Card> PlayerCards { get; set; }
        public Int32 HandValue { get; set; }
        public Decimal CurrentMoney { get; set; }
        public Boolean EndOfRound { get; set; }
        public Decimal CurrentBet { get; set; }
        public Int32 WinCounter { get; set; }
        public Int32 LossCounter { get; set; }
        public Int32 DrawCounter { get; set; }
        public Int32 BlackJackCounter { get; set; }

        public Player(String name)
        {
            this.UserName = name;
            this.PlayerCards = new List<Card>();
            this.HandValue = 0;
            this.CurrentBet = 0.00m;
            this.CurrentMoney = 500.00m;
            this.EndOfRound = false;
            this.WinCounter = 0;
            this.LossCounter = 0;
            this.DrawCounter = 0;
            this.BlackJackCounter = 0;
        }

        public Player(String name, Decimal money, Int32 wins, Int32 losses, Int32 draws)
        {
            this.UserName = name;
            this.PlayerCards = new List<Card>();
            this.HandValue = 0;
            this.CurrentBet = 0.00m;
            this.CurrentMoney = 500.00m;
            this.EndOfRound = false;
            this.WinCounter = wins;
            this.LossCounter = losses;
            this.DrawCounter = draws;
            this.BlackJackCounter = 0;
        }

        public virtual void Hit(Deck blackjackDeck)
        {
            Card card = blackjackDeck.DrawCard();
            this.PlayerCards.Add(card);
            if (card.CardRank.ToString().Equals("Ace"))
            {
                Console.WriteLine("Player {0} has drawn {1}! Do you want the value to be 1 or 11? (Current Score: {2}).", this.UserName, card.CardName, this.HandValue);
                int output;
                while (!int.TryParse(Console.ReadLine(), out output) && output != 1 && output != 11)
                {
                    Console.WriteLine("You can't choose the value {0} for {1}!", output, card.CardName);
                }
                card.CardValue = output;
                this.CalculateHandValue();
            }
            else
            {
                this.CalculateHandValue();
                Console.WriteLine("Player {0} has drawn {1} with a value of {2}! Total score is {3}.", this.UserName, card.CardName, card.CardValue, this.HandValue);
                Console.ReadKey();
            }
            //Set ace values to 1;
            this.TryChangeAceScore();
        }

        public virtual void Stand()
        {
            Console.WriteLine("{0} did not draw a card.", this.UserName);
            this.EndOfRound = true;
        }

        public virtual void DoubleDown(Deck blackJackDeck)
        {
            String currentBetInDollars = this.CurrentBet.ToString("C", new CultureInfo("en-US"));
            Console.WriteLine("{0}, you choose to Double-down! Your current bet is {1}. With how much would you like to increase your bet? (Increase with atleast {2}).", this.UserName, currentBetInDollars, currentBetInDollars);
            decimal output;
            while (!decimal.TryParse(Console.ReadLine(), out output) || output < this.CurrentBet || output > this.CurrentMoney)
            {
                if (output.GetType() == this.CurrentMoney.GetType())
                {
                    String outputInDollars = output.ToString("C", new CultureInfo("en-US"));
                    Console.WriteLine("You can't Double-down with just {0}! Please increase your bet with atleast", outputInDollars, currentBetInDollars);
                }
                else
                {
                    Console.WriteLine("You can't bet with {0}! Please enter the amount of cash in numbers.", output);
                }
            }
            this.CurrentBet += output;
            this.CurrentMoney -= output;
            currentBetInDollars = this.CurrentBet.ToString("C", new CultureInfo("en-US"));
            Console.WriteLine("Your current bet was changed to {0}!.", currentBetInDollars);
            this.Hit(blackJackDeck);
            this.EndOfRound = true;
        }

        public virtual void BetMoney()
        {
            String currentMoneyInDollars = this.CurrentMoney.ToString("C", new CultureInfo("en-US"));
            Console.WriteLine("{0}, how much do you wish to bet? You currently have {1}", this.UserName, currentMoneyInDollars);
            decimal playerBet;
            while (!decimal.TryParse(Console.ReadLine(), out playerBet) || playerBet <= 99.99m || playerBet > this.CurrentMoney)
            {
                if (playerBet.GetType() == this.CurrentMoney.GetType())
                {
                    String outputInDollars = playerBet.ToString("C", new CultureInfo("en-US"));
                    Console.WriteLine("You can't bet with {0}! Please enter the amount of cash in numbers.", outputInDollars);
                }
                else
                {
                    Console.WriteLine("You can't bet with {0}! Please enter the amount of cash in numbers.", playerBet);
                }
            }
            this.CurrentBet += playerBet;
            this.CurrentMoney -= playerBet;
            String currentBetInDollars = playerBet.ToString("C", new CultureInfo("en-US"));
            Console.WriteLine("{0}, you just bet {1}, good luck!", this.UserName, currentBetInDollars);
        }

        public virtual void Surrender()
        {
            this.EndOfRound = true;
        }

        public virtual void ResetHand()
        {
            this.EndOfRound = false;
            this.HandValue = 0;
            this.PlayerCards = new List<Card>();
        }

        public virtual void CalculateHandValue()
        {
            this.HandValue = 0;
            foreach (Card card in this.PlayerCards)
            {
                this.HandValue += card.CardValue;
            }
        }

        public virtual void TryChangeAceScore()
        {
            Boolean ace = false;
            while (this.HandValue > 21 && !ace)
            {
                foreach (Card playerCard in this.PlayerCards)
                {
                    if (playerCard.CardRank.Equals("Ace"))
                    {
                        playerCard.CardValue = 1;
                        ace = true;
                        this.CalculateHandValue();
                        Console.WriteLine("Changed score of your {0} to a score of {1}", playerCard.CardName, playerCard.CardValue);
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
    }
}
