using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using BlackJackLibrary;
using System.Data.SqlClient;
using System.IO;

namespace BlackJack
{
    public class Game
    {
        private PlayerCollection TotalPlayerCollection { get; set; }
        private PlayerCollection PlayerCollectionCurrentGame { get; set; }
        private Deck BlackJackDeck { get; set; }
        private Dealer BlackJackDealer { get; set; }
        private Int32 AmountOfDecks { get; set; }
        private PlayerCollection PlayersWithBlackJack { get; set; }
        private PlayerCollection PlayersWithoutBlackJack { get; set; }
        private Table BlackjackTable { get; set; }
        private String yesPattern = @"y|yes|yeah|ya|ye|hit me";

        public Game()
        {
            this.TotalPlayerCollection = new PlayerCollection();
            this.PlayerCollectionCurrentGame = new PlayerCollection();
        }

        #region Start
        public void StartBlackJack()
        {
            AssignPlayersToTable();
            DefineDeckSize();
            Console.ReadKey();
            this.BlackJackDealer = BlackjackTable.Dealer;
            while (TotalPlayerCollection.List.Count != 0)
            {
                List<Player> playerStoppedList = new List<Player>();
                foreach (Player player in TotalPlayerCollection.List)
                {
                    if (player.CurrentMoney >= 100.0m)
                    {
                        player.BetMoney();
                    }
                    else
                    {
                        playerStoppedList.Add(player);
                    }
                }
                foreach (Player player in playerStoppedList)
                {
                    Console.WriteLine("{0} has left the game with {1}.", player.UserName, player.CurrentMoney);
                    UpdatePlayerData(player);
                    TotalPlayerCollection.List.Remove(player);
                    Console.WriteLine(TotalPlayerCollection.List);
                }
                Console.WriteLine("Players that left the game can reconnect in the next session with their username!");
                this.PlayerCollectionCurrentGame = this.TotalPlayerCollection;
                this.StartBlackJackRound();
            }
        }
        #endregion

        #region AssignPlayers
        private void AssignPlayersToTable()
        {
            Console.WriteLine("Which table do you wish to play at? (Table 1 to  8)");
            int tableId = 0;
            while (!int.TryParse(Console.ReadLine(), out tableId))
            {
                Console.WriteLine("Please enter a table number. (Table 1 to  8)");
            }
            if (tableId > 8 || tableId < 1)
            {
                Console.WriteLine("Ohhh, interesting...");
            }
            Console.WriteLine("With how many players do you wish to play at table {0}? (Maximum of 8)", tableId);
            int players = 0;
            while (!int.TryParse(Console.ReadLine(), out players))
            {
                Console.WriteLine("Please enter a number of players.");
            }
            if (players > 8)
            {
                players = 8;
                Console.WriteLine("Amount of players exceeded 8. The current amount of players will be set to 8.");
            }
            ShowAllPlayers();
            for (int i = 0; i < players; i++)
            {
                Console.SetIn(new StreamReader(Console.OpenStandardInput(1)));
                Console.WriteLine("Please enter a player name.");
                
                String inputName = Console.ReadLine();
                Player player = this.DefinePlayerData(inputName);
                TotalPlayerCollection.List.Add(player);
            }
            BlackjackTable = new Table(tableId, TotalPlayerCollection);
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
                foreach (Player player in PlayerCollectionCurrentGame.List)
                {
                    if (player.HandValue < 21 && BlackJackDealer.TotalCardValue < 21 && !player.EndOfRound)
                    {
                        while (!player.EndOfRound)
                        {
                            if (player.HandValue > 21)
                            {
                                player.TryChangeAceScore();
                                if (player.HandValue > 21)
                                {
                                    player.EndOfRound = true;
                                }
                            }
                            if (player.HandValue < 21)
                            {
                                Console.WriteLine("{0}, your current hand has a total score of {1}. Hit, Stand, Double-Down or Surrender?", player.UserName, player.HandValue);
                                String playerMove = Console.ReadLine().ToLower();
                                if (Regex.IsMatch(playerMove, hitPattern))
                                {
                                    player.Hit(this.BlackJackDeck);
                                }
                                else if (Regex.IsMatch(playerMove, standPattern))
                                {
                                    player.Stand();
                                    player.EndOfRound = true;
                                }
                                else if (Regex.IsMatch(playerMove, doubleDownPattern) && player.CurrentMoney >= player.CurrentBet && player.HandValue >= 9 && player.HandValue <= 11)
                                {
                                    player.DoubleDown(this.BlackJackDeck);
                                    player.EndOfRound = true;
                                }
                                else if (Regex.IsMatch(playerMove, surrenderPattern))
                                {
                                    player.Surrender();
                                    player.EndOfRound = true;
                                }
                                else
                                {
                                    Console.WriteLine("Please use hit, stand, doubledown or surrender as input.");
                                }
                            }
                            else
                            {
                                player.EndOfRound = true;
                                Console.WriteLine("Your score exceeded the BlackJack limit!");
                            }
                        }
                        playerStoppedWithRound.Add(player);
                    } else
                    {
                        player.EndOfRound = true;
                        playerStoppedWithRound.Add(player);
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
            CheckWinConditions();
        }
        #endregion

        #region WinConditions
        private void CheckWinConditions()
        {
            List<Player> playerStopList = new List<Player>();
            PlayersWithBlackJack = new PlayerCollection();
            PlayersWithoutBlackJack = new PlayerCollection();
            PlayerCollectionCurrentGame.LinqSort();
            BlackJackDealer.RevealAllCards();
            foreach (Player currentPlayer in PlayerCollectionCurrentGame.List)
            {
                Console.WriteLine("{0} has a Hand value of {1}", currentPlayer.UserName, currentPlayer.HandValue);
                if (currentPlayer.HandValue == 21)
                {
                    PlayersWithBlackJack.AddPlayer(currentPlayer);
                }
                else
                {
                    PlayersWithoutBlackJack.AddPlayer(currentPlayer);
                }
            }
            //This one Works
            if (PlayersWithBlackJack.List.Count == 0 && BlackJackDealer.TotalCardValue == 21)
            {
                foreach (Player currentPlayer in PlayersWithoutBlackJack.List)
                {
                    BlackJackDealer.CollectPlayerBet(currentPlayer);
                }
            }
            else if (PlayersWithBlackJack.List.Count >= 1 && BlackJackDealer.TotalCardValue == 21 && BlackJackDealer.DealerOpenCards.Count == 2)
            {
                PlayerCollection playersWithNatural = new PlayerCollection();
                foreach (Player currentPlayer in PlayersWithBlackJack.List)
                {
                    if (currentPlayer.PlayerCards.Count == 2)
                    {
                        playersWithNatural.AddPlayer(currentPlayer);
                        BlackJackDealer.NaturalBlackJackPayout(currentPlayer);
                    }
                }
                if (playersWithNatural.List.Count == 0)
                {
                    Console.WriteLine("The dealer has a natural blackjack! Everyone loses their bet.");
                } else
                {
                    Decimal currentBlackJackPayout = BlackJackDealer.CreateBlackjackPayout(PlayerCollectionCurrentGame);
                    Decimal payoutPerPlayer = currentBlackJackPayout / playersWithNatural.List.Count;
                    foreach (Player player in playersWithNatural.List)
                    {
                        PlayerCollectionCurrentGame.List.Remove(player);
                        Console.WriteLine("{0}, you have a natural Black Jack! Your share of the payout is {1}!", player.UserName, payoutPerPlayer);
                        BlackJackDealer.PayBlackjackPlayerBet(player, payoutPerPlayer);
                    }

                }
                foreach (Player player in PlayerCollectionCurrentGame.List)
                {
                    BlackJackDealer.CollectPlayerBet(player);
                }
            }
            else if (PlayersWithBlackJack.List.Count == 1)
            {
                Decimal currentBlackJackPayout = BlackJackDealer.CreateBlackjackPayout(PlayersWithoutBlackJack);
                Player currentPlayer = PlayersWithBlackJack.List[0];
                String blackJackPayout = currentBlackJackPayout.ToString("C", new CultureInfo("en-US"));
                Console.WriteLine("{0}, you have blackjack! You win {1}!", currentPlayer.UserName, blackJackPayout);
                BlackJackDealer.PayBlackjackPlayerBet(currentPlayer, currentBlackJackPayout);
                
            }
            else if (PlayersWithBlackJack.List.Count > 1 && BlackJackDealer.TotalCardValue != 21)
            {
                Decimal currentBlackJackPayout = BlackJackDealer.CreateBlackjackPayout(PlayersWithoutBlackJack);
                Decimal moneyPerPlayer = (currentBlackJackPayout / PlayersWithBlackJack.List.Count);
                String moneyPerPlayerInDollars = moneyPerPlayer.ToString("C", new CultureInfo("en-US"));
                foreach (Player currentPlayer in PlayersWithBlackJack.List)
                {
                    Console.WriteLine("{0}, you have blackjack! You share {1}!", currentPlayer.UserName, moneyPerPlayerInDollars);
                    BlackJackDealer.PayBlackjackPlayerBet(currentPlayer, moneyPerPlayer);
                }
            }
            else if (PlayersWithBlackJack.List.Count > 1 && BlackJackDealer.TotalCardValue == 21)
            {
                foreach (Player currentPlayer in PlayersWithBlackJack.List)
                {
                    String currentBetInDollars = currentPlayer.CurrentBet.ToString("C", new CultureInfo("en-US"));
                    Console.WriteLine("{0}, you and the dealer have blackjack! You get your bet of {1} back.", currentPlayer.UserName, currentBetInDollars);
                    BlackJackDealer.ReturnPlayerBet(currentPlayer);
                }
                foreach (Player player in PlayersWithoutBlackJack.List)
                {
                    BlackJackDealer.CollectPlayerBet(player);
                }
            }
            else
            {
                //Execute non-blackjack options in Else
                foreach (Player currentPlayer in PlayersWithoutBlackJack.List)
                {
                    String currentBetInDollars = currentPlayer.CurrentBet.ToString("C", new CultureInfo("en-US"));
                    //Check win conditions
                    Console.WriteLine("{0}, you have {1} and the Dealer has {2}", currentPlayer.UserName, currentPlayer.HandValue, BlackJackDealer.TotalCardValue);
                    if (BlackJackDealer.TotalCardValue > 21 && currentPlayer.HandValue < 21)
                    {
                        currentBetInDollars = (currentPlayer.CurrentBet * 2).ToString("C", new CultureInfo("en-US"));
                        Console.WriteLine("The dealer has score that exceeds the blackjack limit! You win {0}", currentBetInDollars);
                        BlackJackDealer.PayPlayerBet(currentPlayer);
                    }
                    else if (BlackJackDealer.TotalCardValue < 21 && BlackJackDealer.TotalCardValue < currentPlayer.HandValue && currentPlayer.HandValue < 21)
                    {
                        currentBetInDollars = (currentPlayer.CurrentBet * 2).ToString("C", new CultureInfo("en-US"));
                        Console.WriteLine("You have a higher score then the dealer! You win {0}", currentBetInDollars);
                        BlackJackDealer.PayPlayerBet(currentPlayer);
                    }
                    else if (BlackJackDealer.TotalCardValue > 21 && currentPlayer.HandValue > 21)
                    {
                        currentBetInDollars = currentPlayer.CurrentBet.ToString("C", new CultureInfo("en-US"));
                        Console.WriteLine("Both you and the dealer have a higher score then 21. You get your bet of {0} back.", currentBetInDollars);
                        BlackJackDealer.ReturnPlayerBet(currentPlayer);
                    }
                    else if (BlackJackDealer.TotalCardValue == currentPlayer.HandValue)
                    {
                        currentBetInDollars = currentPlayer.CurrentBet.ToString("C", new CultureInfo("en-US"));
                        Console.WriteLine("You and the Dealer have similar scores! You get your bet of {0} back.", currentBetInDollars);
                        BlackJackDealer.ReturnPlayerBet(currentPlayer);
                    }
                    else
                    {
                        currentBetInDollars = currentPlayer.CurrentBet.ToString("C", new CultureInfo("en-US"));
                        Console.WriteLine("The Dealer has a higher score then you! You lose {0}", currentBetInDollars);
                        BlackJackDealer.CollectPlayerBet(currentPlayer);
                    }
                    currentPlayer.CurrentBet = 0.00m;
                }
            }
            String casinoEarningsInDollars = BlackJackDealer.CasinoEarnings.ToString("C", new CultureInfo("en-US"));
            Console.WriteLine("The casino currently has {0}", casinoEarningsInDollars);
            foreach (Player currentPlayer in PlayerCollectionCurrentGame.List)
            {
                Console.WriteLine("{0}, wanna play again? (y/n)", currentPlayer.UserName);
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
                Console.WriteLine("{0} has left the game with {1}", currentPlayer.UserName, currentPlayer.CurrentMoney);
                UpdatePlayerData(currentPlayer);
                TotalPlayerCollection.List.Remove(currentPlayer);
            }
        }
        #endregion

        #region DatabaseConnections
        private void GenerateNewPlayer(Player player)
        {
            Console.WriteLine("Adding {0} to the player database.", player.UserName);
            SqlConnection currentConnection = new SqlConnection("Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=G:\\CSharpMaart2k17\\Projects\\BlackJack\\BlackJackTest\\BlackJackPlayerDB.mdf;Integrated Security=True");
            currentConnection.Open();
            SqlCommand command = new SqlCommand("INSERT INTO Players (UserName, Money, Wins, Losses, Draws) VALUES (@name, @money, @wins, @losses, @draws)", currentConnection);
            command.Parameters.Add(new SqlParameter("name", player.UserName));
            command.Parameters.Add(new SqlParameter("money", player.CurrentMoney));
            command.Parameters.Add(new SqlParameter("wins", player.WinCounter));
            command.Parameters.Add(new SqlParameter("losses", player.LossCounter));
            command.Parameters.Add(new SqlParameter("draws", player.DrawCounter));
            command.ExecuteNonQuery();
            currentConnection.Close();
            currentConnection.Dispose();
            Console.WriteLine("Finished adding {0} to the player database!", player.UserName);
        }

        private void UpdatePlayerData(Player player)
        {
            Console.WriteLine("Updating player stats...");
            SqlConnection currentConnection = new SqlConnection("Data Source = (LocalDB)\\MSSQLLocalDB; AttachDbFilename = G:\\CSharpMaart2k17\\Projects\\BlackJack\\BlackJackTest\\BlackJackPlayerDB.mdf; Integrated Security = True");
            currentConnection.Open();
            String query = "UPDATE Players SET Money = @money, Wins = @wins, Losses = @losses, Draws = @draws WHERE UserName='" + player.UserName + "'";
            SqlCommand command = new SqlCommand(query, currentConnection);
            command.Parameters.Add(new SqlParameter("money", player.CurrentMoney));
            command.Parameters.Add(new SqlParameter("wins", player.WinCounter));
            command.Parameters.Add(new SqlParameter("losses", player.LossCounter));
            command.Parameters.Add(new SqlParameter("draws", player.DrawCounter));
            command.ExecuteNonQuery();
            currentConnection.Close();
            currentConnection.Dispose();
            Console.WriteLine("Player data was saved in the database.");
        }

        private Player DefinePlayerData(String userName)
        {
            SqlConnection currentConnection = new SqlConnection("Data Source = (LocalDB)\\MSSQLLocalDB; AttachDbFilename = G:\\CSharpMaart2k17\\Projects\\BlackJack\\BlackJackTest\\BlackJackPlayerDB.mdf; Integrated Security = True");
            currentConnection.Open();
            String mainQuery = "SELECT * FROM Players WHERE UserName = '" + userName + "'";
            SqlCommand command = new SqlCommand(mainQuery, currentConnection);
            SqlDataReader dataReader = command.ExecuteReader();
            Player player = new Player(userName);
            if (dataReader.Read())
            {
                Decimal currentMoney = dataReader.GetDecimal(2);
                Int32 wins = dataReader.GetInt32(3);
                Int32 losses = dataReader.GetInt32(4);
                Int32 draws = dataReader.GetInt32(5);
                player = new Player(userName, currentMoney, wins, losses, draws);
                String moneyAsCurrency = currentMoney.ToString("C", new CultureInfo("en-US"));
                Console.WriteLine("Welcome back {0}! Your current money is {1}. Wins {2}, Losses {3}, Draws {4}", userName, moneyAsCurrency, wins, losses, draws);
            }
            else
            {
                Console.WriteLine("New Player");
                GenerateNewPlayer(player);
            }
            //  Maybe needed to insert first player.
            //dataReader.Close();
            //String countQuery = "SELECT Count(*) FROM Players";
            //SqlCommand countCmd = new SqlCommand(countQuery, currentConnection);
            //int count = (int) countCmd.ExecuteScalar();
            //if (count == 0)
            //{
            //    GenerateNewPlayer(player);
            //}
            currentConnection.Close();
            currentConnection.Dispose();
            return player;
        }

        private void ShowAllPlayers()
        {
            SqlConnection currentConnection = new SqlConnection("Data Source = (LocalDB)\\MSSQLLocalDB; AttachDbFilename = G:\\CSharpMaart2k17\\Projects\\BlackJack\\BlackJackTest\\BlackJackPlayerDB.mdf; Integrated Security = True");
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine("************************Player List************************");
            string line1 = String.Format("{0,-10}", String.Format("{0," + (("\t".Length + "Player".Length) / 2).ToString() + "}", "Player"));
            Console.WriteLine("Rank \t{0}\tWins\tLosses\tDraws", line1);
            String newQuery = "SELECT * FROM Players ORDER BY Wins DESC";
            currentConnection.Open();
            SqlCommand command = new SqlCommand(newQuery, currentConnection);
            SqlDataReader dataReader = command.ExecuteReader();
            int i = 0;
            while (dataReader.Read())
            {
                i++;
                String dbUserName = dataReader.GetString(1);
                Int32 wins = dataReader.GetInt32(3);
                Int32 losses = dataReader.GetInt32(4);
                Int32 draws = dataReader.GetInt32(5);
                string playerName = String.Format("{0,-10}", String.Format("{0," + (("\t".Length + dbUserName.Length) / 2).ToString() + "}", dbUserName));
                Console.WriteLine("{0}. \t{1}\t{2}\t{3}\t{4}", i , playerName, wins, losses, draws);
            }
            Console.WriteLine("***********************************************************");
            Console.ResetColor();
            dataReader.Close();
            currentConnection.Close();
            currentConnection.Dispose();
        }
        #endregion
    }
}
