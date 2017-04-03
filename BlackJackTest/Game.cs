﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using BlackJackLibrary;

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
        public PlayerCollection PlayersWithBlackJack { get; set; }
        public PlayerCollection PlayersWithoutBlackJack { get; set; }
        public List<Decimal> PlayerBets { get; set; }
        public List<Decimal> PlayerPayouts { get; set; }

        public Game()
        {
            this.TotalPlayerCollection = new PlayerCollection();
            this.PlayerCollectionCurrentGame = new PlayerCollection();
            this.BlackJackDealer = new Dealer();
        }

        #region Start
        public void StartBlackJack()
        {
            AssignPlayersToTable(1, 4);
            DefineDeckSize();
            while (TotalPlayerCollection.List.Count != 0)
            {
                List<Player> playerStoppedList = new List<Player>();
                foreach (Player player in TotalPlayerCollection.List)
                {
                    if (player.CurrentMoney >= 100.0m)
                    {
                        Decimal bet = player.BetMoney();
                        PlayerBets.Add(bet);
                    }
                    else
                    {
                        playerStoppedList.Add(player);
                    }
                }
                foreach (Player player in playerStoppedList)
                {
                    Console.WriteLine("{0} has left the game with {1}", player.Name, player.CurrentMoney);
                    this.WriteScoreToDatabase(player);
                    this.TotalPlayerCollection.List.Remove(player);
                }
                this.PlayerCollectionCurrentGame = this.TotalPlayerCollection;
                this.StartBlackJackRound();
            }
        }
        #endregion

        #region AssignPlayers
        private Int32 AssignPlayersToTable(Int32 tableNumber, Int32 amountOfPlayers)
        {
            Console.WriteLine("There are currently {0} players. How many players will play blackJack at table {1}?", amountOfPlayers, tableNumber);
            int players;
            while (!int.TryParse(Console.ReadLine(), out players) && players > amountOfPlayers)
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
            Int32 remainingAmountOfPlayers = amountOfPlayers - players;
            return remainingAmountOfPlayers;
        }
        #endregion

        #region DeckSize
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
            }
            else
            {
                AmountOfDecks = 4;
            }
        }
        #endregion

        #region StartingHand
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
                    }
                    else
                    {
                        currentPlayer.Hit(this.BlackJackDeck);
                    }
                }
                this.BlackJackDealer.OpenCard(this.BlackJackDeck);
            }
        } 
        #endregion

        #region BlackJackRound
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
            for (int pIndex = 0; pIndex < PlayerCollectionCurrentGame.List.Count; pIndex++)
                foreach (Player player in PlayerCollectionCurrentGame.List)
                {
                    if (player.HandValue < 21 && BlackJackDealer.TotalCardValue < 21)
                    {
                        Boolean doTurn = true;
                        while (doTurn)
                        {
                            if (player.HandValue > 21)
                            {
                                player.TryChangeAceScore();
                                if (player.HandValue > 21)
                                {
                                    doTurn = false;
                                }
                            }
                            if (player.HandValue < 21)
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
                                else if (Regex.IsMatch(playerMove, doubleDownPattern) && player.CurrentMoney >= PlayerBets[pIndex] && player.HandValue >= 9 && player.HandValue <= 11)
                                {
                                    Decimal newBet = player.DoubleDown(this.BlackJackDeck, PlayerBets[pIndex]);
                                    PlayerBets[pIndex] = newBet;
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
                            }
                            else
                            {
                                doTurn = false;
                                Console.WriteLine("Your score exceeded the BlackJack limit!");
                            }
                        }
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
            Console.WriteLine("Finsihed round");
        }
        #endregion

        //#region WinConditions
        //private void CheckWinConditions()
        //{
        //    Decimal currentBlackJackPayout = 0.00m;
        //    foreach (Decimal bet in PlayerBets)
        //    {
        //        currentBlackJackPayout += bet;
        //    }
        //    List<Player> playerStopList = new List<Player>();
        //    PlayersWithBlackJack = new PlayerCollection();
        //    PlayersWithoutBlackJack = new PlayerCollection();
        //    PlayerCollectionCurrentGame.LinqSort();
        //    this.BlackJackDealer.RevealAllCards();
        //    Console.ForegroundColor = ConsoleColor.Red;
        //    Console.ResetColor();
        //    foreach (Player currentPlayer in PlayerCollectionCurrentGame.List)
        //    {
        //        Console.WriteLine("{0} has a Hand value of {1}", currentPlayer.Name, currentPlayer.HandValue);
        //        if (currentPlayer.HandValue == 21)
        //        {
        //            PlayersWithBlackJack.AddPlayer(currentPlayer);
        //        }
        //        else
        //        {
        //            PlayersWithoutBlackJack.AddPlayer(currentPlayer);
        //        }
        //    }
        //    //Check blackjack options.
        //    if (PlayersWithBlackJack.List.Count == 0 && BlackJackDealer.TotalCardValue == 21)
        //    {
        //        BlackJackDealer.CollectMoney(currentBlackJackPayout);
        //    }
        //    else if (PlayersWithBlackJack.List.Count >= 1 && BlackJackDealer.TotalCardValue == 21 && BlackJackDealer.DealerOpenCards.Count == 2)
        //    {
        //        Boolean playerWithNatural = false;
        //        foreach (Player player in PlayersWithBlackJack.List)
        //        {
        //            if (player.PlayerCards.Count == 2)
        //            {
        //            }
        //        }
        //        if (!playerWithNatural)
        //        {
        //            BlackJackDealer.CollectMoney(currentBlackJackPayout);
        //        }
        //    }
        //    else if (PlayersWithBlackJack.List.Count == 1)
        //    {
        //        Player player = PlayersWithBlackJack.List[0];
        //        String blackJackPayout = currentBlackJackPayout.ToString("C", new CultureInfo("en-US"));
        //        Console.WriteLine("{0}, you have blackjack! You win {1}!", player.Name, blackJackPayout);
        //        player.CurrentMoney += currentBlackJackPayout;
        //        player.CurrentBet = 0.00m;
        //    }
        //    else if (PlayersWithBlackJack.List.Count > 1 && BlackJackDealer.TotalCardValue != 21)
        //    {
        //        Decimal moneyPerPlayer = (currentBlackJackPayout / PlayersWithBlackJack.List.Count);
        //        String moneyPerPlayerInDollars = moneyPerPlayer.ToString("C", new CultureInfo("en-US"));
        //        foreach (Player currentPlayer in PlayersWithBlackJack.List)
        //        {
        //            Console.WriteLine("{0}, you have blackjack! You share {1}!", currentPlayer.Name, moneyPerPlayerInDollars);
        //            currentPlayer.CurrentMoney += moneyPerPlayer;
        //            currentPlayer.CurrentBet = 0.00m;
        //        }
        //    }
        //    else if (PlayersWithBlackJack.List.Count > 1 && BlackJackDealer.TotalCardValue == 21)
        //    {
        //        foreach (Player currentPlayer in PlayersWithBlackJack.List)
        //        {
        //            String currentBetInDollars = currentPlayer.CurrentBet.ToString("C", new CultureInfo("en-US"));
        //            Console.WriteLine("{0}, you and the dealer have blackjack! You get your bet of {1} back.", currentPlayer.Name, currentBetInDollars);
        //            currentPlayer.CurrentMoney += currentPlayer.CurrentBet;
        //            currentBlackJackPayout -= currentPlayer.CurrentBet;
        //            currentPlayer.CurrentBet = 0.00m;
        //        }
        //        BlackJackDealer.CollectMoney(currentBlackJackPayout);
        //    }
        //    else
        //    {
        //        //Execute non-blackjack options in Else
        //        foreach (Player currentPlayer in PlayersWithoutBlackJack.List)
        //        {
        //            String currentBetInDollars = currentPlayer.CurrentBet.ToString("C", new CultureInfo("en-US"));
        //            //Check win conditions
        //            Console.WriteLine("{0}, you have {1} and the Dealer has {2}", currentPlayer.Name, currentPlayer.HandValue, BlackJackDealer.TotalCardValue);
        //            if (BlackJackDealer.TotalCardValue > 21 && currentPlayer.HandValue < 21)
        //            {
        //                currentBetInDollars = (currentPlayer.CurrentBet * 2).ToString("C", new CultureInfo("en-US"));
        //                Console.WriteLine("The dealer has score that exceeds the blackjack limit! You win {0}", currentBetInDollars);
        //                currentPlayer.CurrentMoney += (currentPlayer.CurrentBet * 2);
        //                BlackJackDealer.CasinoEarnings -= currentPlayer.CurrentBet;
        //            }
        //            else if (BlackJackDealer.TotalCardValue < 21 && BlackJackDealer.TotalCardValue < currentPlayer.HandValue && currentPlayer.HandValue < 21)
        //            {
        //                currentBetInDollars = (currentPlayer.CurrentBet * 2).ToString("C", new CultureInfo("en-US"));
        //                Console.WriteLine("You have a higher score then the dealer! You win {0}", currentBetInDollars);
        //                currentPlayer.CurrentMoney += (currentPlayer.CurrentBet * 2);
        //                BlackJackDealer.CasinoEarnings -= currentPlayer.CurrentBet;
        //            }
        //            else if (BlackJackDealer.TotalCardValue > 21 && currentPlayer.HandValue > 21)
        //            {
        //                currentBetInDollars = currentPlayer.CurrentBet.ToString("C", new CultureInfo("en-US"));
        //                Console.WriteLine("Both you and the dealer have a higher score then 21. You get your bet of {0} back.", currentBetInDollars);
        //                currentPlayer.CurrentMoney += currentPlayer.CurrentBet;
        //                BlackJackDealer.CasinoEarnings -= currentPlayer.CurrentBet;
        //            }
        //            else if (BlackJackDealer.TotalCardValue == currentPlayer.HandValue)
        //            {
        //                currentBetInDollars = currentPlayer.CurrentBet.ToString("C", new CultureInfo("en-US"));
        //                Console.WriteLine("You and the Dealer have similar scores! You get your bet of {0} back.", currentBetInDollars);
        //                currentPlayer.CurrentMoney += currentPlayer.CurrentBet;
        //                currentPlayer.CurrentMoney += currentPlayer.CurrentBet;
        //                BlackJackDealer.CasinoEarnings -= currentPlayer.CurrentBet;
        //            }
        //            else
        //            {
        //                currentBetInDollars = currentPlayer.CurrentBet.ToString("C", new CultureInfo("en-US"));
        //                Console.WriteLine("The Dealer has a higher score then you! You lose {0}", currentBetInDollars);
        //                BlackJackDealer.CollectMoney(currentPlayer.CurrentBet);
        //            }
        //            currentPlayer.CurrentBet = 0.00m;
        //        }
        //    }
        //    String casinoEarningsInDollars = BlackJackDealer.CasinoEarnings.ToString("C", new CultureInfo("en-US"));
        //    Console.WriteLine("The casino currently has {0}", casinoEarningsInDollars);
        //    foreach (Player currentPlayer in PlayerCollectionCurrentGame.List)
        //    {
        //        Console.WriteLine("Wanna play again? (y/n)");
        //        if (!Regex.IsMatch(Console.ReadLine().ToLower(), yesPattern))
        //        {
        //            playerStopList.Add(currentPlayer);
        //        }
        //        else
        //        {
        //            currentPlayer.EndOfRound = false;
        //        }
        //    }
        //    foreach (Player currentPlayer in playerStopList)
        //    {
        //        Console.WriteLine("{0} has left the game with {1}", currentPlayer.Name, currentPlayer.CurrentMoney);
        //        TotalPlayerCollection.List.Remove(currentPlayer);
        //    }
        //} 
        //#endregion

        private void WriteScoreToDatabase(Player player)
        {
            throw new NotImplementedException();
        }
    }
}
