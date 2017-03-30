using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BlackJack
{
    class Game
    {
        public List<Player> PlayerList { get; set; }
        public List<Player> PlayerListCurrentGame { get; set; }
        public Int32 AmountOfPlayers { get; set; }
        public Deck BlackJackDeck { get; set; }
        public Dealer BlackJackDealer { get; set; }
        public Int32 AmountOfDecks { get; set; }
        public String yesPattern = @"y|yes|yeah|ya|ye|hit me";
        public List<Player> PlayerWinnerList { get; set; }
        public List<Player> PlayerLoserList { get; set; }
        public Game()
        {
            this.SetAmountOfPlayers();
            this.CreatePlayerList();
            this.BlackJackDealer = new Dealer();
            while (PlayerList.Count != 0)
            {
                this.PlayerListCurrentGame = this.PlayerList;
                this.StartBlackJackRound();
            }
        }

        public void SetAmountOfPlayers()
        {
            Console.WriteLine("Please enter an amount of players for a game of BlackJack");
            int players;
            while (!int.TryParse(Console.ReadLine(), out players))
            {
                Console.WriteLine("Please enter a number of players.");
            }
            this.AmountOfPlayers = players;
        }

        private void CreatePlayerList()
        {
            for (int i = 0; i < this.AmountOfPlayers; i++)
            {
                int id = i;
                id++;
                Player player = new Player(id);
                PlayerList.Add(player);
            }
        }

        private void DefineDeckSize()
        {
            if (PlayerList.Count <= 2)
            {
                AmountOfDecks = 1;
            }
            else if (PlayerList.Count <= 4)
            {
                AmountOfDecks = 2;
            }
            else if (PlayerList.Count <= 6)
            {
                AmountOfDecks = 3;
            } else
            {
                AmountOfDecks = 4;
            }
        }

        public void StartingHandRound()
        {
            for (int i = 0; i < 2; i++)
            {
                foreach (Player currentPlayer in PlayerListCurrentGame)
                {
                    if (i == 0)
                    {
                        currentPlayer.ResetHand();
                        currentPlayer.Hit(this.BlackJackDeck);
                    } else
                    {
                        currentPlayer.Hit(this.BlackJackDeck);
                    }
                }
                if (i == 0) {
                    this.BlackJackDealer.ResetHand();
                    this.BlackJackDealer.OpenCard(this.BlackJackDeck);
                } else
                {
                    this.BlackJackDealer.Hit(this.BlackJackDeck);
                }
            }
        }

        public void StartBlackJackRound()
        {
            this.BlackJackDeck = new Deck(AmountOfDecks);
            this.StartingHandRound();
            Boolean continueRound = true;
            while (continueRound)
            {
                foreach (Player player in PlayerListCurrentGame)
                {
                    if (!player.EndOfRound && player.HandValue < 21 && BlackJackDealer.HandValue < 21)
                    {
                        Console.WriteLine("Your current hand is worth {0}. Do you want to draw a card? (y/n)", player.HandValue);
                        String input = Console.ReadLine();
                        if (Regex.IsMatch(input.ToLower(), yesPattern))
                        {
                            player.Hit(this.BlackJackDeck);
                        }
                        else
                        {
                            player.EndOfRound = true;
                        }
                        //draw card, y/n?                   done
                        //dealer 1 card                     done
                        //Check if a wincondition was met.  Not implemented correctly.
                    } else
                    {
                        continueRound = false;
                        break;
                    }
                }
                BlackJackDealer.Hit(this.BlackJackDeck);
            }
            CheckWinConditions();
        }

        public void CheckWinConditions()
        {
            List<Player> playerStopList = new List<Player>();
            foreach (Player currentPlayer in PlayerListCurrentGame)
            {
                String currentBetInDollars = currentPlayer.CurrentBet.ToString("C", new CultureInfo("en-US"));
                //Check win conditions
                Console.WriteLine("You have {0} and the Dealer has {1}", currentPlayer.HandValue, BlackJackDealer.HandValue);
                if (currentPlayer.HandValue == 21)
                {
                    currentBetInDollars = (currentPlayer.CurrentBet * 2).ToString("C", new CultureInfo("en-US"));
                    Console.WriteLine("You have blackjack! You win {0}", currentBetInDollars);
                    currentPlayer.CurrentMoney = (currentPlayer.CurrentMoney + (currentPlayer.CurrentBet * 2));
                }
                else if (BlackJackDealer.HandValue == 21)
                {
                    currentBetInDollars = (currentPlayer.CurrentBet * 2).ToString("C", new CultureInfo("en-US"));
                    Console.WriteLine("The Dealer has blackjack! You lose {0}", currentBetInDollars);
                    currentPlayer.CurrentMoney = (currentPlayer.CurrentMoney - (currentPlayer.CurrentBet * 2));
                }
                else if (BlackJackDealer.HandValue < 21 && BlackJackDealer.HandValue < currentPlayer.HandValue && currentPlayer.HandValue < 21)
                {
                    currentBetInDollars = currentPlayer.CurrentBet.ToString("C", new CultureInfo("en-US"));
                    Console.WriteLine("You have a higher score then the dealer! You win {0}", currentBetInDollars);
                    currentPlayer.CurrentMoney = (currentPlayer.CurrentMoney + currentPlayer.CurrentBet);
                }
                else if (BlackJackDealer.HandValue > 21 && currentPlayer.HandValue < 21)
                {
                    currentBetInDollars = currentPlayer.CurrentBet.ToString("C", new CultureInfo("en-US"));
                    Console.WriteLine("The dealer has score that exceeds the blackjack limit! You win {0}", currentBetInDollars);
                    currentPlayer.CurrentMoney = (currentPlayer.CurrentMoney + currentPlayer.CurrentBet);
                }
                else if (BlackJackDealer.HandValue == currentPlayer.HandValue)
                {
                    currentBetInDollars = currentPlayer.CurrentBet.ToString("C", new CultureInfo("en-US"));
                    Console.WriteLine("You and the Dealer have similar scores! You get your bet of {0} back.", currentBetInDollars);
                }
                {
                    currentBetInDollars = currentPlayer.CurrentBet.ToString("C", new CultureInfo("en-US"));
                    Console.WriteLine("The Dealer has a higher score then you! You lose {0}", currentBetInDollars);
                    currentPlayer.CurrentMoney = (currentPlayer.CurrentMoney - currentPlayer.CurrentBet);
                }
                //Replay
                Console.WriteLine("Wanna play again? (y/n)");
                if (!Regex.IsMatch(Console.ReadLine().ToLower(), yesPattern))
                {
                    playerStopList.Add(currentPlayer);
                }
            }
            foreach (Player player in playerStopList)
            {
                PlayerList.Remove(player);
            }
        }
    }
}
