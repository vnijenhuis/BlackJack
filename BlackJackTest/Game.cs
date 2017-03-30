using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BlackJack
{
    public class Game
    {
        public List<Player> PlayerList { get; set; }
        public List<Player> PlayerListCurrentGame { get; set; }
        public Deck BlackJackDeck { get; set; }
        public Dealer BlackJackDealer { get; set; }
        public Int32 AmountOfDecks { get; set; }
        public String yesPattern = @"y|yes|yeah|ya|ye|hit me";
        public List<Player> PlayerWinnerList { get; set; }
        public List<Player> PlayerLoserList { get; set; }

        public Game()
        {
            this.PlayerList = new List<Player>();
            this.PlayerListCurrentGame = new List<Player>();
            this.BlackJackDealer = new Dealer();
            this.PlayerWinnerList = new List<Player>();
            this.PlayerLoserList = new List<Player>();
        }

        public void RunBlackJackGame()
        {
            this.CreatePlayerList();
            this.DefineDeckSize();
            while (PlayerList.Count != 0)
            {
                foreach (Player player in PlayerList)
                {

                    Console.WriteLine("{0}, how much do you wish to bet? You currently have {1}", player.Name, player.CurrentMoney);
                    decimal output;
                    while (!decimal.TryParse(Console.ReadLine(), out output))
                    {
                        Console.WriteLine("You can't bet with {0}! Please enter the amount of cash in numbers.", player.CurrentBet);
                    }
                    player.CurrentBet = output;
                    Console.WriteLine("{0}, you just bet {1}, good luck!", player.Name, player.CurrentBet);
                }
                this.PlayerListCurrentGame = this.PlayerList;
                this.StartBlackJackRound();
            }
        }

        private void CreatePlayerList()
        {
            Console.WriteLine("Please enter an amount of players for a game of BlackJack");
            int players;
            while (!int.TryParse(Console.ReadLine(), out players))
            {
                Console.WriteLine("Please enter a number of players.");
            }
            for (int i = 0; i < players; i++)
            {
                Console.WriteLine("Please enter a player name.");
                String inputName = Console.ReadLine();
                Player player = new Player(inputName);
                PlayerList.Add(player);
            }
        }

        private void DefineDeckSize()
        {
            this.AmountOfDecks = 0;
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

        private void StartingHandRound()
        {
            this.BlackJackDealer.ResetHand();
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
                if (BlackJackDealer.OpenCardValue == 0)
                {
                    this.BlackJackDealer.OpenCard(this.BlackJackDeck);
                }
                else if (BlackJackDealer.DealerCards.Count == 1)
                {
                    this.BlackJackDealer.Hit(this.BlackJackDeck);
                }
            }
        }

        private void StartBlackJackRound()
        {
            this.BlackJackDeck = new Deck(AmountOfDecks);
            this.StartingHandRound();
            Boolean continueRound = true;
            List<Player> playerStoppedWithRound = new List<Player>();
            while (continueRound)
            {
                foreach (Player player in PlayerListCurrentGame)
                {
                    if (!player.EndOfRound)
                    {
                        if (player.HandValue < 21 && BlackJackDealer.HandValue < 21)
                        {
                            Console.WriteLine("{0}, your current hand is worth {1}. Do you want to draw a card? (y/n)", player.Name, player.HandValue);
                            String input = Console.ReadLine();
                            if (Regex.IsMatch(input.ToLower(), yesPattern))
                            {
                                Console.WriteLine("Player said YES");
                                player.Hit(this.BlackJackDeck);
                            }
                            else
                            {
                                Console.WriteLine("Player said NO");
                                player.EndOfRound = true;
                            }
                        }
                        else
                        {
                            continueRound = false;
                            break;
                        }
                    } else
                    {
                        playerStoppedWithRound.Add(player);
                    }
                }
                BlackJackDealer.Hit(this.BlackJackDeck);
                if (playerStoppedWithRound.Count == PlayerListCurrentGame.Count)
                {
                    while (BlackJackDealer.HandValue <= 16)
                    {
                        BlackJackDealer.Hit(this.BlackJackDeck);
                    }
                    continueRound = false;
                    break;
                }
            }
            Boolean hasAWinner = CheckWinConditions();
        }

        private Boolean CheckWinConditions()
        {
            Boolean hasWinner = false;
            List<Player> playerStopList = new List<Player>();
            foreach (Player currentPlayer in PlayerListCurrentGame)
            {
                String currentBetInDollars = currentPlayer.CurrentBet.ToString("C", new CultureInfo("en-US"));
                //Check win conditions
                Console.WriteLine("{0}, you have {1} and the Dealer has {2}",currentPlayer.Name , currentPlayer.HandValue, BlackJackDealer.HandValue);
                if (BlackJackDealer.HandValue == currentPlayer.HandValue)
                {
                    currentBetInDollars = currentPlayer.CurrentBet.ToString("C", new CultureInfo("en-US"));
                    Console.WriteLine("You and the Dealer have similar scores! You get your bet of {0} back.", currentBetInDollars);
                }
                else if (currentPlayer.HandValue == 21)
                {
                    currentBetInDollars = (currentPlayer.CurrentBet * 2).ToString("C", new CultureInfo("en-US"));
                    Console.WriteLine("{0}, you have blackjack! You win {1}", currentPlayer.Name, currentBetInDollars);
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
                else 
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
                } else
                {
                    currentPlayer.EndOfRound = false;
                }
            }
            foreach (Player player in playerStopList)
            {
                Console.WriteLine("{0} has left the game with {1}", player.Name, player.CurrentMoney);
                PlayerList.Remove(player);
            }
            return hasWinner;
        }
    }
}
