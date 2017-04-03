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
        public PlayerCollection TotalPlayerCollection { get; set; }
        public PlayerCollection PlayerCollectionCurrentGame { get; set; }
        public Deck BlackJackDeck { get; set; }
        public Dealer BlackJackDealer { get; set; }
        public Int32 AmountOfDecks { get; set; }
        public String yesPattern = @"y|yes|yeah|ya|ye|hit me";
        public Decimal CurrentBlackJackPayout { get; set; }
        public PlayerCollection PlayersWithBlackJack { get; set; }
        public PlayerCollection PlayersWithoutBlackJack { get; set; }

        public Game()
        {
            this.TotalPlayerCollection = new PlayerCollection();
            this.PlayerCollectionCurrentGame = new PlayerCollection();
            this.BlackJackDealer = new Dealer();
        }

        public void RunBlackJackGame()
        {
            this.CreatePlayerList();
            this.DefineDeckSize();
            while (TotalPlayerCollection.List.Count != 0)
            {
                this.CurrentBlackJackPayout = 0.00m;
                List<Player> playerStoppedList = new List<Player>();
                foreach (Player player in TotalPlayerCollection.List)
                {
                    if (player.CurrentMoney >= 100.0m)
                    {
                        player.BetMoney();
                    } else
                    {
                        playerStoppedList.Add(player);
                    }
                }
                foreach (Player player in playerStoppedList)
                {
                    Console.WriteLine("{0} has left the game with {1}", player.Name, player.CurrentMoney);
                    this.TotalPlayerCollection.List.Remove(player);
                }
                String payoutInDollers = this.CurrentBlackJackPayout.ToString("C", new CultureInfo("en-US"));
                Console.WriteLine("Black Jack payout currently is {0} plus twice your current bet.", payoutInDollers);
                this.PlayerCollectionCurrentGame = this.TotalPlayerCollection;
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
            if (players > 8)
            {
                players = 8;
                Console.WriteLine("Amount of players exceeded 8. The current amount of players will be set to 8.");
            }
            for (int i = 0; i < players; i++)
            {
                Console.WriteLine("Please enter a player name.");
                String inputName = Console.ReadLine();
                Player player = new Player(inputName);
                TotalPlayerCollection.List.Add(player);
            }
        }

        private void DefineDeckSize()
        {
            this.AmountOfDecks = 0;
            if (TotalPlayerCollection.List.Count <= 2)
            {
                AmountOfDecks = 1;
            }
            else if (TotalPlayerCollection.List.Count <= 4)
            {
                AmountOfDecks = 2;
            }
            else if (TotalPlayerCollection.List.Count <= 6)
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
                foreach (Player currentPlayer in PlayerCollectionCurrentGame.List)
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
                this.BlackJackDealer.OpenCard(this.BlackJackDeck);
            }
        }

        private void StartBlackJackRound()
        {
            String hitPattern = "(hit|hi|ht|h)";
            String doubleDownPattern = "(doubledown|doubled|double|down|dbld|dbl|dbd|dld|dd)";
            String standPattern = "(stand|stnd|std|snd|st|s|no more|stop)";
            String surrenderPattern = "(surrender|surrenderPattern|concede|sur|srnd|sr)";
            this.BlackJackDeck = new Deck(AmountOfDecks);
            this.StartingHandRound();
            Boolean continueRound = true;
            List<Player> playerStoppedWithRound = new List<Player>();
            while (continueRound)
            {
                foreach (Player player in PlayerCollectionCurrentGame.List)
                {
                    if (!player.EndOfRound)
                    {
                        if (player.HandValue < 21 && BlackJackDealer.TotalCardValue < 21)
                        {
                            Boolean doTurn = true;
                            while (doTurn)
                            {
                                if (player.HandValue > 21)
                                {
                                    player.TryChangeAceScore();
                                }
                                if (player.HandValue == 21)
                                {
                                    this.BlackJackDealer.NaturalBlackJackPayout(player);
                                    doTurn = false;
                                }
                                else if (player.HandValue < 21)
                                {
                                    Console.WriteLine("{0}, your current hand has a total score of {1}. Hit, Stand, Double-Down or Surrender?", player.Name, player.HandValue);
                                    String playerMove = Console.ReadLine().ToLower();
                                    if (Regex.IsMatch(playerMove, hitPattern))
                                    {
                                        player.Hit(this.BlackJackDeck);
                                    }
                                    else if (Regex.IsMatch(playerMove, standPattern))
                                    {
                                        player.Stand();
                                        doTurn = false;
                                    }
                                    else if (Regex.IsMatch(playerMove, doubleDownPattern) && player.CurrentMoney >= player.CurrentBet && player.HandValue >= 9 && player.HandValue <= 11)
                                    {
                                        player.DoubleDown(this.BlackJackDeck, this.CurrentBlackJackPayout);
                                        doTurn = false;
                                    }
                                    else if (Regex.IsMatch(playerMove, surrenderPattern))
                                    {
                                        player.Surrender();
                                        doTurn = false;
                                    }
                                    else
                                    {
                                        Console.WriteLine("Please use hit, stand, doubledown or surrender as input.");
                                    }
                                } else
                                {
                                    doTurn = false;
                                    Console.WriteLine("Your score exceeded the BlackJack limit!");
                                }
                            }
                        }
                        else
                        {
                            continueRound = false;
                        }
                    } else
                    {
                        playerStoppedWithRound.Add(player);
                    }
                    if (playerStoppedWithRound.Count == PlayerCollectionCurrentGame.List.Count)
                    {
                        break;
                    }
                }
                while (BlackJackDealer.TotalCardValue < 17)
                {
                    BlackJackDealer.OpenCard(this.BlackJackDeck);
                }
                if (playerStoppedWithRound.Count == PlayerCollectionCurrentGame.List.Count)
                {
                    while (BlackJackDealer.TotalCardValue < 17)
                    {
                        BlackJackDealer.OpenCard(this.BlackJackDeck);
                    }
                    continueRound = false;
                    break;
                }
            }
            Console.ReadKey();
            CheckWinConditions();
        }

        private void CheckWinConditions()
        {
            List<Player> playerStopList = new List<Player>();
            PlayersWithBlackJack = new PlayerCollection();
            PlayersWithoutBlackJack = new PlayerCollection();
            PlayerCollectionCurrentGame.LinqSort();
            this.BlackJackDealer.RevealAllCards();
            Console.ForegroundColor = ConsoleColor.Red;
            Console.ResetColor();
            foreach (Player currentPlayer in PlayerCollectionCurrentGame.List)
            {
                Console.WriteLine("{0} has a Hand value of {1}", currentPlayer.Name, currentPlayer.HandValue);
                if (currentPlayer.HandValue == 21)
                {
                    PlayersWithBlackJack.AddPlayer(currentPlayer);
                } else
                {
                    PlayersWithoutBlackJack.AddPlayer(currentPlayer);
                }
            }
            //Check blackjack options.
            if (PlayersWithBlackJack.List.Count > 1 && BlackJackDealer.TotalCardValue != 21)
            {
                Decimal moneyPerPlayer = (CurrentBlackJackPayout / PlayersWithBlackJack.List.Count);
                String moneyPerPlayerInDollars = moneyPerPlayer.ToString("C", new CultureInfo("en-US"));
                foreach (Player currentPlayer in PlayersWithBlackJack.List)
                {
                    Console.WriteLine("{0}, you have blackjack! You win {1}!", currentPlayer.Name, moneyPerPlayerInDollars);
                    currentPlayer.CurrentMoney += moneyPerPlayer;   
                }
            } else if (PlayersWithBlackJack.List.Count > 1 && BlackJackDealer.TotalCardValue == 21)
            {
                foreach (Player currentPlayer in PlayersWithBlackJack.List)
                {
                    String currentBetInDollars = currentPlayer.CurrentBet.ToString("C", new CultureInfo("en-US"));
                    Console.WriteLine("{0}, you and the dealer have blackjack! You get your bet of {1} back.", currentPlayer.Name, currentBetInDollars);
                    currentPlayer.CurrentMoney += currentPlayer.CurrentBet;
                }
            } else if (PlayersWithBlackJack.List.Count == 1)
            {
                Player player = PlayersWithBlackJack.List[0];
                String blackJackPayout = CurrentBlackJackPayout.ToString("C", new CultureInfo("en-US"));
                Console.WriteLine("{0}, you have blackjack! You win {1}!", player.Name, blackJackPayout);
                player.CurrentMoney += CurrentBlackJackPayout;
            } else
            {
                //Execute non-blackjack options in Else
                foreach (Player currentPlayer in PlayersWithoutBlackJack.List)
                {
                    String currentBetInDollars = currentPlayer.CurrentBet.ToString("C", new CultureInfo("en-US"));
                    //Check win conditions
                    Console.WriteLine("{0}, you have {1} and the Dealer has {2}", currentPlayer.Name, currentPlayer.HandValue, BlackJackDealer.TotalCardValue);
                    if (BlackJackDealer.TotalCardValue == currentPlayer.HandValue)
                    {
                        currentBetInDollars = currentPlayer.CurrentBet.ToString("C", new CultureInfo("en-US"));
                        Console.WriteLine("You and the Dealer have similar scores! You get your bet of {0} back.", currentBetInDollars);
                        currentPlayer.CurrentMoney += currentPlayer.CurrentBet;
                    }
                    else if (BlackJackDealer.TotalCardValue < 21 && BlackJackDealer.TotalCardValue < currentPlayer.HandValue && currentPlayer.HandValue < 21)
                    {
                        currentBetInDollars = (currentPlayer.CurrentBet * 2).ToString("C", new CultureInfo("en-US"));
                        Console.WriteLine("You have a higher score then the dealer! You win {0}", currentBetInDollars);
                        currentPlayer.CurrentMoney += (currentPlayer.CurrentBet * 2);
                    }
                    else if (BlackJackDealer.TotalCardValue > 21 && currentPlayer.HandValue < 21)
                    {
                        currentBetInDollars = (currentPlayer.CurrentBet * 2).ToString("C", new CultureInfo("en-US"));
                        Console.WriteLine("The dealer has score that exceeds the blackjack limit! You win {0}", currentBetInDollars);
                        currentPlayer.CurrentMoney += (currentPlayer.CurrentBet * 2);
                    }
                    else if (BlackJackDealer.TotalCardValue > 21 && currentPlayer.HandValue > 21)
                    {
                        currentBetInDollars = currentPlayer.CurrentBet.ToString("C", new CultureInfo("en-US"));
                        Console.WriteLine("Both you and the dealer have a higher score then 21. You get your bet of {0} back.", currentBetInDollars);
                        currentPlayer.CurrentMoney += currentPlayer.CurrentBet;
                    }
                    else
                    {
                        currentBetInDollars = currentPlayer.CurrentBet.ToString("C", new CultureInfo("en-US"));
                        Console.WriteLine("The Dealer has a higher score then you! You lose {0}", currentBetInDollars);
                        currentPlayer.CurrentMoney -= currentPlayer.CurrentBet;
                    }
                }
            }
            foreach (Player currentPlayer in PlayerCollectionCurrentGame.List)
            {
                Console.WriteLine("Wanna play again? (y/n)");
                if (!Regex.IsMatch(Console.ReadLine().ToLower(), yesPattern))
                {
                    playerStopList.Add(currentPlayer);
                }
                else
                {
                    currentPlayer.EndOfRound = false;
                }
            }
            foreach (Player currentPlayer in playerStopList)
            {
                Console.WriteLine("{0} has left the game with {1}", currentPlayer.Name, currentPlayer.CurrentMoney);
                TotalPlayerCollection.List.Remove(currentPlayer);
            }
        }
    }
}
