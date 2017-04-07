using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using BlackJackLibrary;

namespace BlackJack
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Welcome to Thor's blackjack bar!");

            #region Game
            Game game = new Game();
            game.StartBlackJack();
            #endregion

            #region Simulator
            //int players = 8;
            //int rounds = 10000;
            //int repeats = 5;
            //int minimumStopScore = 16;
            //decimal betAmount = 500.00m;
            //bool sameScore = true;
            //for (int i = 0; i < repeats; i++) {
            //    SimulateGame simulator = new SimulateGame();
            //    simulator.RunBlackJackGame(players, rounds, minimumStopScore, betAmount, sameScore);
            //}
            #endregion
            Console.WriteLine("Thanks for comming! Please press a key to leave.");
            Console.ReadKey(); 
        }
    }
}
