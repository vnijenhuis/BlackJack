using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BlackJack
{
    public class SimulateGame
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

        public SimulateGame()
        {
            this.TotalPlayerCollection = new PlayerCollection();
            this.PlayerCollectionCurrentGame = new PlayerCollection();
            this.BlackJackDealer = new Dealer();
        }

        public void RunBlackJackGame(Int32 players, Int32 rounds, Int32 stopScore, Decimal betAmount, Boolean sameScore)
        {
            this.CreatePlayerList(players, rounds, stopScore, betAmount, sameScore);
            this.DefineDeckSize();
            for (int i = 0; i < rounds; i++)
            {
                if (i % 1000 == 0)
                {
                    Trace.WriteLine("Simulated {0} rounds of blackjack!", i.ToString());
                }
                this.CurrentBlackJackPayout = 0.00m;
                List<Player> playerStoppedList = new List<Player>();
                foreach (SimulatedPlayer player in TotalPlayerCollection.List)
                {
                    if (player.CurrentMoney >= player.CurrentBet)
                    {
                        player.BetMoney();
                    }
                    else
                    {
                        playerStoppedList.Add(player);
                    }
                }
                String payoutInDollers = this.CurrentBlackJackPayout.ToString("C", new CultureInfo("en-US"));
                this.PlayerCollectionCurrentGame = this.TotalPlayerCollection;
                this.StartBlackJackRound();
            }
            Int32 totalDealer = BlackJackDealer.WinCounter + BlackJackDealer.LossCounter + BlackJackDealer.DrawCounter;
            String casinoEarnings = this.BlackJackDealer.CasinoEarnings.ToString("C", new CultureInfo("en-US"));
            Console.WriteLine("Dealer, Wins: {0}, Losses: {1} ,Draws: {2}! Total: {3}, Total BlackJack: {4}, Total Earnings: {5}", BlackJackDealer.WinCounter, BlackJackDealer.LossCounter, BlackJackDealer.DrawCounter, totalDealer, BlackJackDealer.BlackJackCounter, casinoEarnings);
            foreach (SimulatedPlayer player in TotalPlayerCollection.List)
            {
                Int32 totalPlayer = player.WinCounter + player.LossCounter + player.DrawCounter;
                String currentMoneyInDollars = player.CurrentMoney.ToString("C", new CultureInfo("en-US"));
                Console.WriteLine("{0}, RiskScore: {1}, Wins: {2}, Losses: {3} ,Draws: {4}, Total: {5}, Total BlackJack: {6}!, Total money: {7}", player.Name, player.StopScore, player.WinCounter, player.LossCounter, player.DrawCounter, totalPlayer, player.BlackJackCounter, currentMoneyInDollars);
            }
        }

        private void CreatePlayerList(Int32 players, Int32 rounds, Int32 stopScore, Decimal betAmount, Boolean sameScore)
        {
            Int32 currentStopScore = stopScore;
            for (int i = 0; i < players; i++)
            {
                String inputName = "Player " + i;
                if (sameScore)
                {
                    currentStopScore = stopScore;
                } else
                {
                    currentStopScore = stopScore + i;
                }
                SimulatedPlayer player = new SimulatedPlayer(inputName, currentStopScore);
                player.CurrentMoney = (player.CurrentMoney * rounds);
                player.CurrentBet = betAmount;
                String currentMoneyInDollars = player.CurrentMoney.ToString("C", new CultureInfo("en-US"));
                Console.WriteLine("{0} starts with {1}.", player.Name, currentMoneyInDollars);
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
                this.BlackJackDealer.SimulateOpenCard(this.BlackJackDeck);
                //if (BlackJackDealer.OpenCardValue == 0)
                //{
                //    this.BlackJackDealer.SimulateOpenCard(this.BlackJackDeck);
                //}
                //else if (BlackJackDealer.DealerCards.Count == 1)
                //{
                //    this.BlackJackDealer.Hit(this.BlackJackDeck);
                //}
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
                foreach (SimulatedPlayer player in PlayerCollectionCurrentGame.List)
                {
                    if (!player.EndOfRound)
                    {
                        if (player.HandValue < 21 && BlackJackDealer.TotalCardValue < 21)
                        {
                            Boolean doTurn = true;
                            while (doTurn)
                            {
                                if (player.StopScore >= 16 && player.HandValue >= 9 && player.HandValue <= 11 && BlackJackDealer.OpenCardValue < 10 && BlackJackDealer.OpenCardValue > 17)
                                {
                                    player.DoubleDown(this.BlackJackDeck, this.CurrentBlackJackPayout);
                                    doTurn = false;
                                }
                                else if (player.HandValue >= player.StopScore)
                                {
                                    player.Stand();
                                    doTurn = false;
                                }
                                else if (player.HandValue < player.StopScore)
                                {
                                    player.Hit(this.BlackJackDeck);
                                }
                                else
                                {
                                    player.Surrender();
                                    doTurn = false;
                                }
                            }
                        }
                        else
                        {
                            continueRound = false;
                        }
                    }
                    else
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
                    BlackJackDealer.SimulateOpenCard(this.BlackJackDeck);
                }
                if (playerStoppedWithRound.Count == PlayerCollectionCurrentGame.List.Count)
                {
                    while (BlackJackDealer.TotalCardValue < 17)
                    {
                        BlackJackDealer.SimulateOpenCard(this.BlackJackDeck);
                    }
                    continueRound = false;
                }
            }
            CheckWinConditions();
        }

        private void CheckWinConditions()
        {
            PlayersWithBlackJack = new PlayerCollection();
            PlayersWithoutBlackJack = new PlayerCollection();
            PlayerCollectionCurrentGame.LinqSort();
            foreach (SimulatedPlayer currentPlayer in PlayerCollectionCurrentGame.List)
            {
                if (currentPlayer.HandValue == 21)
                {
                    PlayersWithBlackJack.AddPlayer(currentPlayer);
                }
                else
                {
                    PlayersWithoutBlackJack.AddPlayer(currentPlayer);
                }
            }
            //Check blackjack options.
            if (PlayersWithBlackJack.List.Count > 1 && BlackJackDealer.TotalCardValue != 21)
            {
                Decimal moneyPerPlayer = (CurrentBlackJackPayout / PlayersWithBlackJack.List.Count);
                foreach (SimulatedPlayer currentPlayer in PlayersWithBlackJack.List)
                {
                    currentPlayer.BlackJackCounter += 1;
                    currentPlayer.DrawCounter += 1;
                    //Deze
                    //currentPlayer.CurrentMoney += moneyPerPlayer + currentPlayer.CurrentBet;
                    this.BlackJackDealer.LossCounter += 1;
                    //Deze
                    this.BlackJackDealer.CasinoEarnings -= currentPlayer.CurrentBet * 2;
                }
                foreach (SimulatedPlayer currentPlayer in PlayersWithoutBlackJack.List)
                {
                    currentPlayer.LossCounter += 1;
                    BlackJackDealer.LossCounter += 1;
                }
            }
            else if (PlayersWithBlackJack.List.Count > 1 && BlackJackDealer.TotalCardValue == 21)
            {
                foreach (SimulatedPlayer currentPlayer in PlayersWithBlackJack.List)
                {
                    currentPlayer.BlackJackCounter += 1;
                    //Deze
                    //currentPlayer.CurrentMoney += currentPlayer.CurrentBet;
                    this.BlackJackDealer.DrawCounter += 1;
                    currentPlayer.DrawCounter += 1;
                }
                foreach (SimulatedPlayer currentPlayer in PlayersWithoutBlackJack.List)
                {
                    //Deze
                    //this.BlackJackDealer.CasinoEarnings += currentPlayer.CurrentBet;
                    //this.BlackJackDealer.WinCounter += 1;
                    currentPlayer.LossCounter += 1;
                }
                this.BlackJackDealer.BlackJackCounter += 1;
            }
            else if (PlayersWithBlackJack.List.Count == 1)
            {
                SimulatedPlayer player = (SimulatedPlayer) PlayersWithBlackJack.List[0];
                player.BlackJackCounter += 1;
                //Deze
                //player.CurrentMoney += CurrentBlackJackPayout + player.CurrentBet;
                player.WinCounter += 1;
                BlackJackDealer.LossCounter += 1;
                //Deze
                this.BlackJackDealer.CasinoEarnings -= player.CurrentBet * 2;
                foreach (SimulatedPlayer currentPlayer in this.PlayersWithoutBlackJack.List)
                {
                    currentPlayer.LossCounter += 1;
                    BlackJackDealer.LossCounter += 1;
                }
            } else if (PlayersWithBlackJack.List.Count == 0 && this.BlackJackDealer.TotalCardValue == 21)
            {
                foreach (SimulatedPlayer currentPlayer in this.PlayersWithoutBlackJack.List)
                {
                    currentPlayer.LossCounter += 1;
                    BlackJackDealer.WinCounter += 1;
                    //Deze
                    //CurrentBlackJackPayout += currentPlayer.CurrentBet;
                    //currentPlayer.CurrentMoney -= currentPlayer.CurrentBet;
                }
                this.BlackJackDealer.BlackJackCounter += 1;
                //Deze
                //this.BlackJackDealer.CasinoEarnings += CurrentBlackJackPayout;
            }
            else
            {
                //Execute non-blackjack options in Else
                foreach (SimulatedPlayer currentPlayer in PlayersWithoutBlackJack.List)
                {
                    //Check win conditions
                    if (BlackJackDealer.TotalCardValue == currentPlayer.HandValue)
                    {
                        //Deze
                        //currentPlayer.CurrentMoney += currentPlayer.CurrentBet;
                        this.BlackJackDealer.DrawCounter += 1;
                        currentPlayer.DrawCounter += 1;
                    }
                    else if (BlackJackDealer.TotalCardValue < 21 && BlackJackDealer.TotalCardValue < currentPlayer.HandValue && currentPlayer.HandValue < 21)
                    {
                        //Deze
                        //currentPlayer.CurrentMoney += (currentPlayer.CurrentBet * 2);
                        //this.BlackJackDealer.CasinoEarnings -= currentPlayer.CurrentBet;
                        this.BlackJackDealer.LossCounter += 1;
                        currentPlayer.WinCounter += 1;
                    }
                    else if (BlackJackDealer.TotalCardValue > 21 && currentPlayer.HandValue < 21)
                    {
                        //Deze
                        //currentPlayer.CurrentMoney += (currentPlayer.CurrentBet * 2);
                        //this.BlackJackDealer.CasinoEarnings -= currentPlayer.CurrentBet;
                        this.BlackJackDealer.LossCounter += 1;
                        currentPlayer.WinCounter += 1;
                    }
                    else if (BlackJackDealer.TotalCardValue > 21 && currentPlayer.HandValue > 21)
                    {
                        //Deze
                        //currentPlayer.CurrentMoney += currentPlayer.CurrentBet;
                        //this.BlackJackDealer.CasinoEarnings -= currentPlayer.CurrentBet;
                        this.BlackJackDealer.LossCounter += 1;
                        currentPlayer.WinCounter += 1;
                    }
                    else
                    {
                        //Deze
                        //currentPlayer.CurrentMoney -= currentPlayer.CurrentBet;
                        //this.BlackJackDealer.CasinoEarnings += currentPlayer.CurrentBet;
                        this.BlackJackDealer.WinCounter += 1;
                        currentPlayer.LossCounter += 1;
                    }
                }
            }
        }
    }
}
