using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BlackJack
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Welcome to Thor's blackjack bar!");
            #region FirstTest
            //String yesPattern = @"y|yes|yeah|ya|ye|hit me";
            //Player player = new Player(1);
            ////Player money

            //Dealer dealer = new Dealer();
            //Boolean wantToPlay = true;
            //while (wantToPlay && player.CurrentMoney > 0.00m)
            //{
            //    //Define starting values.
            //    player.ResetHand();
            //    dealer.ResetHand();
            //    Deck blackJackDeck = new Deck();
            //    String currentMoneyInDollars = player.CurrentMoney.ToString("C", new CultureInfo("en-US"));
            //    Console.WriteLine("You have {0} to spend!", currentMoneyInDollars);
            //    String currentBetInDollars = player.CurrentBet.ToString("C", new CultureInfo("en-US"));
            //    Console.WriteLine("How much do you wish to bet? Your current bet is set to {0}", currentBetInDollars);
            //    decimal output;
            //    while (!decimal.TryParse(Console.ReadLine(), out output))
            //    {
            //        Console.WriteLine("You can't bet with {0}! Please enter the amount of cash in numbers.");
            //    }
            //    player.CurrentBet = output;
            //    currentBetInDollars = player.CurrentBet.ToString("C", new CultureInfo("en-US"));
            //    Console.WriteLine("You are betting {0}!", currentBetInDollars);
            //    dealer.DrawStartingHands(player, blackJackDeck);
            //    Boolean draw = true;
            //    //If deck isn't empty or draw has been cancelled.
            //    while (draw)
            //    {
            //        Console.WriteLine("Your current hand is worth {0}. Do you want to draw a card? (y/n)", player.HandValue);
            //        String input = Console.ReadLine();
            //        Card card = blackJackDeck.DrawCard();
            //        blackJackDeck.CardDeck.Remove(card);
            //        player.Hit(blackJackDeck);
            //        if (Regex.IsMatch(input.ToLower(), yesPattern))
            //        {
            //            if (card.ValueName.Equals("Ace"))
            //            {
            //                Console.WriteLine("You drawed {0}! Do you want the value to be 1 or 10?", card.CardName);
            //                card.CardValue = int.Parse(Console.ReadLine());
            //                player.HandValue += card.CardValue;
            //            }
            //            else
            //            {
            //                Console.WriteLine("You drawed {0}!", card.CardName);
            //                player.HandValue += card.CardValue;
            //            }
            //        } else
            //        {
            //            draw = false;
            //        }
            //        if (player.HandValue >= 21)
            //        {
            //            draw = false;
            //            break;
            //        }
            //        card = blackJackDeck.DrawCard();
            //        blackJackDeck.CardDeck.Remove(card);
            //        dealer.HandValue += card.CardValue;
            //        if (dealer.HandValue >= 21)
            //        {
            //            draw = false;
            //            break;
            //        }
            //    }
            //    //Check win conditions
            //    Console.WriteLine("You have {0} and the Dealer has {1}", player.HandValue, dealer.HandValue);
            //    if (player.HandValue == 21)
            //    {
            //        currentBetInDollars = (player.CurrentBet * 3).ToString("C", new CultureInfo("en-US"));
            //        Console.WriteLine("You have blackjack! You win {0}", currentBetInDollars);
            //        player.CurrentMoney = (player.CurrentMoney + (player.CurrentBet * 3));
            //    } else if (dealer.HandValue == 21)
            //    {
            //        currentBetInDollars = (player.CurrentBet * 3).ToString("C", new CultureInfo("en-US"));
            //        Console.WriteLine("The Dealer has blackjack! You lose {0}", currentBetInDollars);
            //        player.CurrentMoney = (player.CurrentMoney - (player.CurrentBet * 3));
            //    } else if (dealer.HandValue < 21 && dealer.HandValue < player.HandValue && player.HandValue < 21) 
            //    {
            //        currentBetInDollars = player.CurrentBet.ToString("C", new CultureInfo("en-US"));
            //        Console.WriteLine("You have a higher score then the dealer! You win {0}", currentBetInDollars);
            //        player.CurrentMoney = (player.CurrentMoney + player.CurrentBet);
            //    } else if (dealer.HandValue > 21 && player.HandValue < 21)
            //    {
            //        currentBetInDollars = player.CurrentBet.ToString("C", new CultureInfo("en-US"));
            //        Console.WriteLine("The dealer has score that exceeds the blackjack limit! You win {0}", currentBetInDollars);
            //        player.CurrentMoney = (player.CurrentMoney + player.CurrentBet);
            //    } else if (dealer.HandValue == player.HandValue)
            //    {
            //        currentBetInDollars = player.CurrentBet.ToString("C", new CultureInfo("en-US"));
            //        Console.WriteLine("You and the Dealer have similar scores! You get your bet of {0} back.", currentBetInDollars);
            //    }
            //    {
            //        currentBetInDollars = player.CurrentBet.ToString("C", new CultureInfo("en-US"));
            //        Console.WriteLine("The Dealer has a higher score then you! You lose {0}", currentBetInDollars);
            //        player.CurrentMoney = (player.CurrentMoney - player.CurrentBet);
            //    }
            //    //Replay
            //    Console.WriteLine("Wanna play again? (y/n)");
            //    if (!Regex.IsMatch(Console.ReadLine().ToLower(), yesPattern))
            //    {
            //        wantToPlay = false;
            //    }
            //}
            //if (player.CurrentMoney <= 0.00m)
            //{
            //    Console.WriteLine("You can't spend more money!");
            //} 
            #endregion
            Game game = new Game();
            game.RunBlackJackGame();
            Console.WriteLine("Thanks for comming! Please press a key to leave.");
            Console.ReadKey();
        }
    }
}
